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

        private Dictionary<string, List<string>> tableDependencies = new Dictionary<string, List<string>>
        {
            { "MonthlyDues", new List<string> { "Residents", "Units", "Homeowners" } },
            { "HomeownerUnits", new List<string> { "Units", "Homeowners" } },
            { "DesktopItems", new List<string> { "Homeowners" } }
        };

        public BackupRestoreManager(docurepo parent)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            parentDocuRepo = parent;
            LoadTableList();
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

                new TableItem { DisplayName = "Units", TableName = "TBL_Units" },
                new TableItem { DisplayName = "Events", TableName = "Events" },
                new TableItem { DisplayName = "Residents", TableName = "Residents" },
                new TableItem { DisplayName = "Homeowners", TableName = "Homeowners" },
                new TableItem { DisplayName = "Visitors Log", TableName = "TBL_VisitorsLog" },
                new TableItem { DisplayName = "Monthly Dues", TableName = "MonthlyDues" },
                new TableItem { DisplayName = "Announcements", TableName = "Announcements" },
                new TableItem { DisplayName = "Homeowner Units", TableName = "HomeownerUnits" },
                new TableItem { DisplayName = "Documents Repository", TableName = "DesktopItems" },
                new TableItem { DisplayName = "Garbage Collection Schedules", TableName = "GarbageCollectionSchedules" }
            };

            foreach (var table in tables)
                clbTables.Items.Add(table, table.TableName == "DesktopItems");
        }

        // folder picker
        private string ChooseFolder(string title)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = title;
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                    return fbd.SelectedPath;
            }
            return null;
        }

        // backup button
        private async void btnBackup_Click_1(object sender, EventArgs e)
        {
            string folder = ChooseFolder("Select folder to save backup");
            if (string.IsNullOrEmpty(folder)) return;

            string backupRoot = Path.Combine(folder, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(backupRoot);

            var tables = clbTables.CheckedItems.Cast<TableItem>().Select(t => t.TableName).ToList();
            if (!tables.Any())
            {
                MessageBox.Show("No tables selected for backup.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            progressBar.Value = 0;
            progressBar.Maximum = tables.Count;

            try
            {
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
                                            row[colName] = Convert.ToBase64String((byte[])val);
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
                                        else row[colName] = val;
                                    }
                                    else row[colName] = null;
                                }
                                rows.Add(row);
                            }
                        }

                        File.WriteAllText(Path.Combine(tableFolder, "metadata.json"), JsonConvert.SerializeObject(rows, Formatting.Indented));
                        progressBar.Value++;
                        await Task.Delay(50);
                    }
                }

                MessageBox.Show("Backup completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Value = 0;
            }
        }

        // restore buttn
        private readonly HashSet<string> identityTables = new HashSet<string>
        {
            "DesktopItems",
            "MonthlyDues",
            "Homeowners",
            "Residents",
            "TBL_Units",
            "HomeownerUnits",
            "Announcements",
            "TBL_VisitorsLog"
        };

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            string folder = ChooseFolder("Select backup folder to restore");
            if (string.IsNullOrEmpty(folder)) return;

            if (MessageBox.Show(
                "Restoring will overwrite all current data, make sure you back-up first. Do you want to continue?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var tableFolders = Directory.GetDirectories(folder);
            if (!tableFolders.Any())
            {
                MessageBox.Show("No backup folders found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progressBar.Value = 0;
            progressBar.Maximum = tableFolders.Length;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    foreach (var tableFolder in tableFolders)
                    {
                        string table = Path.GetFileName(tableFolder);
                        string metadataFile = Path.Combine(tableFolder, "metadata.json");
                        if (!File.Exists(metadataFile)) continue;

                        var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(File.ReadAllText(metadataFile));
                        if (rows == null || !rows.Any()) continue;

                        bool hasIdentity = identityTables.Contains(table);

                        if (hasIdentity)
                            using (var cmd = new SqlCommand($"SET IDENTITY_INSERT {table} ON", conn)) cmd.ExecuteNonQuery();

                        using (var cmdClear = new SqlCommand($"DELETE FROM {table}", conn))
                            cmdClear.ExecuteNonQuery();

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

                            string columns = string.Join(",", row.Keys);
                            string paramNames = string.Join(",", row.Keys.Select(k => "@" + k));

                            using (var cmd = new SqlCommand($"INSERT INTO {table} ({columns}) VALUES ({paramNames})", conn))
                            {
                                foreach (var kv in row)
                                    cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }

                            await Task.Delay(20);
                        }

                        if (hasIdentity)
                            using (var cmd = new SqlCommand($"SET IDENTITY_INSERT {table} OFF", conn)) cmd.ExecuteNonQuery();

                        progressBar.Value++;
                    }
                }

                MessageBox.Show("Restore completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restore failed!\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Value = 0;
            }
        }

        private bool allSelected = false; // toggle 

        private void btnToggleSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTables.Items.Count; i++)
            {
                clbTables.SetItemChecked(i, !allSelected);
            }
            allSelected = !allSelected;

            btnToggleSelect.Text = allSelected ? "Deselect All" : "Select All";
        }
    }
}
