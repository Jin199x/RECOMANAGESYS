using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RECOMANAGESYS
{
    public partial class BackupRestoreManager : Form
    {
        private docurepo parentDocuRepo;

        private string logFilePath = Path.Combine(Application.StartupPath, "BackupRestoreLog.txt");

        private Dictionary<string, List<string>> tableDependencies = new Dictionary<string, List<string>>
        {
            { "MonthlyDues", new List<string> { "Residents", "Units", "Homeowners" } },
            { "HomeownerUnits", new List<string> { "Units", "Homeowners" } },
            { "DesktopItems", new List<string> { "Homeowners" } }
        };

        public BackupRestoreManager(docurepo parent)
        {
            InitializeComponent();
            parentDocuRepo = parent;
            LoadTableList();
            LoadPersistentLog();
            btnRestore.MouseEnter += btnRestore_MouseEnter;
            btnRestore.MouseLeave += btnRestore_MouseLeave;
            btnBackup.MouseEnter += btnBackup_MouseEnter;
            btnBackup.MouseLeave += btnBackup_MouseLeave;
        }

        private void RefreshAllData()
        {
            parentDocuRepo?.RefreshAllData();
        }

        public class TableItem
        {
            public string DisplayName { get; set; }
            public string TableName { get; set; }
            public override string ToString() => DisplayName;
        }

        private void LoadTableList()
        {
            clbTables.Items.Clear();

            var tables = new List<TableItem>
            {
                new TableItem { DisplayName = "Documents Repository", TableName = "DesktopItems" },
                new TableItem { DisplayName = "Monthly Dues", TableName = "MonthlyDues" },
                new TableItem { DisplayName = "Homeowners", TableName = "Homeowners" },
                new TableItem { DisplayName = "Residents", TableName = "Residents" },
                new TableItem { DisplayName = "Units", TableName = "TBL_Units" },
                new TableItem { DisplayName = "Homeowner Units", TableName = "HomeownerUnits" },
                new TableItem { DisplayName = "Announcements", TableName = "Announcements" },
                new TableItem { DisplayName = "Visitors Log", TableName = "TBL_VisitorsLog" }
            };

            foreach (var table in tables)
                clbTables.Items.Add(table, table.TableName == "DesktopItems");
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = fbd.SelectedPath;
                }
            }
        }

        // Logs
        private void LoadPersistentLog()
        {
            if (File.Exists(logFilePath))
            {
                rtbLog.Text = File.ReadAllText(logFilePath);
                rtbLog.SelectionStart = rtbLog.Text.Length; 
                rtbLog.ScrollToCaret();
            }
        }

        private void AppendLog(string text)
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] {text}\n";
            rtbLog.AppendText(logLine);
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();

            File.AppendAllText(logFilePath, logLine);
        }


        // Backup 
        private async void btnBackup_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBackupPath.Text))
            {
                MessageBox.Show("Please select a folder to save the backup first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Stop the backup if no path is selected
            }

            string backupRoot = Path.Combine(txtBackupPath.Text, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(backupRoot);


            try
            {
                var tables = clbTables.CheckedItems.Cast<TableItem>().Select(t => t.TableName).ToList();
                if (!tables.Any())
                {
                    MessageBox.Show("No tables selected for backup.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                progressBar.Value = 0;
                progressBar.Maximum = tables.Count;

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    foreach (var table in tables)
                    {
                        string tableFolder = Path.Combine(backupRoot, table);
                        Directory.CreateDirectory(tableFolder);

                        var rows = new List<Dictionary<string, object>>();

                        using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {table}", conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    object val = reader[i];
                                    string colName = reader.GetName(i);

                                    if (val != DBNull.Value)
                                    {
                                        if (reader.GetFieldType(i) == typeof(byte[]))
                                        {
                                            row[colName] = Convert.ToBase64String((byte[])val);
                                        }
                                        else if (table == "DesktopItems" && colName == "FilePath")
                                        {
                                            string originalPath = val.ToString();
                                            row[colName] = originalPath;

                                            if (File.Exists(originalPath))
                                            {
                                                string destPath = Path.Combine(tableFolder, Path.GetFileName(originalPath));
                                                File.Copy(originalPath, destPath, true);
                                                row["FileBackupPath"] = destPath;
                                            }
                                            else
                                            {
                                                row["FileBackupPath"] = null;
                                            }
                                        }
                                        else
                                        {
                                            row[colName] = val;
                                        }
                                    }
                                    else
                                    {
                                        row[colName] = null;
                                    }
                                }
                                rows.Add(row);
                            }
                        }

                        File.WriteAllText(Path.Combine(tableFolder, "metadata.json"), JsonConvert.SerializeObject(rows, Formatting.Indented));
                        AppendLog($"Backup collected for {table} ({rows.Count} rows)");
                        progressBar.Value++;
                        await Task.Delay(50);
                    }
                }

                AppendLog($"Backup completed at {backupRoot}");
                MessageBox.Show("Backup completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"Backup failed: {ex.Message}");
                MessageBox.Show($"Backup failed!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Value = 0;
            }
        }

        // Confirm restore validation
        private bool ConfirmRestore(string table)
        {
            var result = MessageBox.Show(
                $"You are about to restore '{table}'. This may overwrite existing records.\nDo you want to continue?",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return result == DialogResult.Yes;
        }

        // Dependency validation
        private bool CheckDependencies(List<string> tablesToRestore)
        {
            foreach (var table in tablesToRestore)
            {
                if (tableDependencies.ContainsKey(table))
                {
                    var missing = tableDependencies[table].Where(t => !tablesToRestore.Contains(t)).ToList();
                    if (missing.Any())
                    {
                        string msg = $"It is recommended to restore [{string.Join(", ", missing)}] before restoring [{table}] due to dependencies.";
                        var result = MessageBox.Show(msg + "\nDo you want to continue?", "Dependency Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.No) return false;
                    }
                }
            }
            return true;
        }

        // Restore
        private async void btnRestore_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtBackupPath.Text))
            {
                MessageBox.Show("Backup folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string backupRoot = txtBackupPath.Text;
            var tablesToRestore = clbTables.CheckedItems.Cast<TableItem>().Select(t => t.TableName).ToList();
            if (!tablesToRestore.Any())
            {
                MessageBox.Show("No tables selected for restore.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!CheckDependencies(tablesToRestore)) return;

            progressBar.Value = 0;
            progressBar.Maximum = tablesToRestore.Count;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    foreach (var table in tablesToRestore)
                    {
                        string tableFolder = Path.Combine(backupRoot, table);
                        if (!Directory.Exists(tableFolder)) tableFolder = backupRoot;

                        string metadataFile = Path.Combine(tableFolder, "metadata.json");
                        if (!File.Exists(metadataFile)) throw new FileNotFoundException($"metadata.json not found for {table} backup.");

                        if (!ConfirmRestore(table)) continue;

                        var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(File.ReadAllText(metadataFile));
                        if (rows == null || !rows.Any()) throw new Exception("Backup data is invalid or empty.");

                        Dictionary<int, int> idMapping = new Dictionary<int, int>();
                        bool hasIdentity = table == "DesktopItems";

                        if (hasIdentity)
                            using (var cmd = new SqlCommand($"SET IDENTITY_INSERT {table} ON", conn)) cmd.ExecuteNonQuery();

                        foreach (var row in rows)
                        {
                            if (table == "DesktopItems" && row.ContainsKey("FileBackupPath"))
                            {
                                string backupFile = row["FileBackupPath"]?.ToString();
                                if (!string.IsNullOrEmpty(backupFile) && File.Exists(backupFile))
                                {
                                    string restoreFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesktopItemsRestore");
                                    Directory.CreateDirectory(restoreFolder);

                                    string fileName = Path.GetFileName(backupFile);
                                    string restoredPath = Path.Combine(restoreFolder, fileName);
                                    File.Copy(backupFile, restoredPath, true);

                                    row["FilePath"] = restoredPath;
                                }
                                else row["FilePath"] = DBNull.Value;

                                row.Remove("FileBackupPath");
                            }

                            // Handle ParentId mapping
                            int? oldParentId = null;
                            if (row.ContainsKey("ParentId") && row["ParentId"] != null && row["ParentId"] != DBNull.Value)
                                oldParentId = Convert.ToInt32(row["ParentId"]);

                            row["ParentId"] = oldParentId.HasValue && idMapping.ContainsKey(oldParentId.Value)
                                ? (object)idMapping[oldParentId.Value]
                                : DBNull.Value;

                            // Upsert logic (merge data)
                            string columns = string.Join(",", row.Keys);
                            string paramNames = string.Join(",", row.Keys.Select(k => "@" + k));
                            string updateSet = string.Join(",", row.Keys.Where(k => k != "ItemId").Select(k => $"{k}=@{k}"));

                            using (var cmd = new SqlCommand($@"
                            IF EXISTS (SELECT 1 FROM {table} WHERE ItemId=@ItemId)
                            UPDATE {table} SET {updateSet} WHERE ItemId=@ItemId ELSE
                            INSERT INTO {table} ({columns}) VALUES ({paramNames}) ", conn))
                            {
                                foreach (var kv in row)
                                    cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }

                            if (table == "DesktopItems")
                            {
                                int oldId = Convert.ToInt32(row["ItemId"]);
                                idMapping[oldId] = oldId;
                            }

                            AppendLog($"{table} row restored/merged successfully (ItemId={row["ItemId"]})");
                            await Task.Delay(20);
                        }

                        if (hasIdentity)
                            using (var cmd = new SqlCommand($"SET IDENTITY_INSERT {table} OFF", conn)) cmd.ExecuteNonQuery();

                        AppendLog($"{table} restored successfully ({rows.Count} rows)");
                        progressBar.Value++;
                    }
                }

                AppendLog("Restore process completed.");
                MessageBox.Show("Restore completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshAllData();
            }
            catch (Exception ex)
            {
                AppendLog($"Restore failed: {ex.Message}");
                MessageBox.Show("Restore failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Value = 0;
            }
        }
        private void btnRestore_MouseEnter(object sender, EventArgs e)
        {
            lblRestoreHint.Visible = true;
        }

        private void btnRestore_MouseLeave(object sender, EventArgs e)
        {
            lblRestoreHint.Visible = false;
        }
        private void btnBackup_MouseEnter(object sender, EventArgs e)
        {
            lblBackUpHint.Visible = true;
        }

        private void btnBackup_MouseLeave(object sender, EventArgs e)
        {
            lblBackUpHint.Visible = false;
        }
    }

}
