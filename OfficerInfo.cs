using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class OfficerInfo : Form
    {
        private DataTable originalDataTable;
        private bool isInEditMode = false;
        private bool isLoadingData = false;
        private bool isSavingViaEnter = false;

        public OfficerInfo()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            SetupDataGridView();
            LoadOfficers();

            this.DGVOfficers.SelectionChanged += new System.EventHandler(this.DGVOfficers_SelectionChanged);

            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (originalDataTable == null) return;

            string searchText = txtSearch.Text.Trim();
            DataView dv = originalDataTable.DefaultView;

            if (string.IsNullOrEmpty(searchText))
            {
                dv.RowFilter = string.Empty;
            }
            else
            {
                string safeSearchText = searchText.Replace("'", "''");

                try
                {
                    dv.RowFilter = string.Format(
                        "FullName LIKE '%{0}%' OR " +
                        "CompleteAddress LIKE '%{0}%' OR " +
                        "ContactNumber LIKE '%{0}%' OR " +
                        "PositionInHOA LIKE '%{0}%' OR " +
                        "DisplayID LIKE '%{0}%'",
                        safeSearchText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Filter Error: " + ex.Message);
                    dv.RowFilter = string.Empty;
                }
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key was pressed
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (isInEditMode && keyData == Keys.Enter && this.DGVOfficers.IsCurrentCellInEditMode)
            {
                this.Editbtn.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetupDataGridView()
        {
            DGVOfficers.AutoGenerateColumns = false;
            DGVOfficers.AllowUserToAddRows = false;
            DGVOfficers.AllowUserToDeleteRows = false;
            DGVOfficers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVOfficers.MultiSelect = false;

            DGVOfficers.Columns.Clear();
            DGVOfficers.RowTemplate.Height = 35;

            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisplayID", HeaderText = "ID", Name = "DisplayID", Width = 50 });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Name", Name = "FullName", Width = 150 });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CompleteAddress", HeaderText = "Complete Address", Name = "CompleteAddress", Width = 230 });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ContactNumber", HeaderText = "Contact Number", Name = "ContactNumber", Width = 120 });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MemberSince", HeaderText = "Member Since", Name = "MemberSince", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" } });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PositionInHOA", HeaderText = "Position in HOA", Name = "PositionInHOA", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OfficerID", HeaderText = "Officer ID", Name = "OfficerID", Visible = false });

            foreach (DataGridViewColumn col in DGVOfficers.Columns)
            {
                col.ReadOnly = true;
            }
        }

        public void LoadOfficers()
        {
            isLoadingData = true;
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT
                                        u.UserID AS OfficerID,
                                        CONCAT_WS(' ', u.Firstname, u.MiddleName, u.Lastname) AS FullName,
                                        ISNULL(u.CompleteAddress, 'Not Specified') AS CompleteAddress,
                                        ISNULL(u.ContactNumber, 'Not Specified') AS ContactNumber,
                                        u.MemberSince,
                                        r.RoleName AS PositionInHOA
                                      FROM Users u
                                      LEFT JOIN TBL_Roles r ON u.RoleId = r.RoleId";

                    if (CurrentUser.Role != "Developer")
                    {
                        query += " WHERE r.RoleName <> 'Developer'";
                    }
                    query += " ORDER BY u.Lastname, u.Firstname";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dt.Columns.Add("DisplayID", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["DisplayID"] = row["OfficerID"].ToString().PadLeft(3, '0');
                        }

                        originalDataTable = dt;

                        DGVOfficers.DataSource = originalDataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingData = false;
            }
        }

        private void OfficerInfo_Load(object sender, EventArgs e)
        {
            var allowedRoles = new List<string> { "Developer", "President", "Vice President", "Secretary" };
            bool canEdit = allowedRoles.Contains(CurrentUser.Role);

            Editbtn.Visible = canEdit;
            viewLockAccounts.Visible = CurrentUser.Role == "President" || CurrentUser.Role == "Developer";
        }

        private void ToggleEditMode(bool editing)
        {
            isInEditMode = editing;
            Editbtn.Text = editing ? "Save" : "Edit";

            DGVOfficers.Columns["CompleteAddress"].ReadOnly = !editing;
            DGVOfficers.Columns["ContactNumber"].ReadOnly = !editing;

            Color cellColor = editing ? Color.LightYellow : SystemColors.Window;
            DGVOfficers.Columns["CompleteAddress"].DefaultCellStyle.BackColor = cellColor;
            DGVOfficers.Columns["ContactNumber"].DefaultCellStyle.BackColor = cellColor;

            DGVOfficers.Refresh();
        }

        private void SaveChanges()
        {
            DGVOfficers.EndEdit();

            DataTable changes = originalDataTable.GetChanges(DataRowState.Modified);

            if (changes == null || changes.Rows.Count == 0)
            {
                MessageBox.Show("No changes were made.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ToggleEditMode(false);
                return;
            }

            int updatedCount = 0;
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    foreach (DataRow row in changes.Rows)
                    {
                        string updateQuery = @"UPDATE Users SET 
                                                    CompleteAddress = @CompleteAddress, 
                                                    ContactNumber = @ContactNumber
                                                WHERE UserID = @OfficerID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CompleteAddress", row["CompleteAddress"]);
                            cmd.Parameters.AddWithValue("@ContactNumber", row["ContactNumber"]);
                            cmd.Parameters.AddWithValue("@OfficerID", row["OfficerID"]);
                            cmd.ExecuteNonQuery();
                            updatedCount++;
                        }
                    }
                }
                MessageBox.Show($"{updatedCount} officer record(s) updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving changes: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                LoadOfficers();
                txtSearch.Clear(); 
                ToggleEditMode(false);
            }
        }

        private void DGVOfficers_SelectionChanged(object sender, EventArgs e)
        {
            if (isSavingViaEnter)
            {
                isSavingViaEnter = false;
                return;
            }

            if (isLoadingData || !isInEditMode)
            {
                return;
            }

            this.DGVOfficers.SelectionChanged -= DGVOfficers_SelectionChanged;

            DGVOfficers.DataSource = originalDataTable.Copy();
            ToggleEditMode(false);

            this.DGVOfficers.SelectionChanged += DGVOfficers_SelectionChanged;
        }
        private void Editbtn_Click(object sender, EventArgs e)
        {
            if (isInEditMode)
            {
                SaveChanges();
            }
            else
            {
                ToggleEditMode(true);
            }
        }

        private void officerPanel_Paint(object sender, PaintEventArgs e)
        {
            // no code
        }

        private void registerbtn_Click(object sender, EventArgs e)
        {
            var regform = new registerform();
            regform.RegistrationSuccess += (s, args) =>
            {
                LoadOfficers();
                txtSearch.Clear(); // Also clear search after a new user is added
            };
            regform.Show();
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (DGVOfficers.SelectedRows.Count == 0 || DGVOfficers.SelectedRows[0].IsNewRow)
            {
                MessageBox.Show("Please select a profile to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = DGVOfficers.SelectedRows[0];
            string officerID = selectedRow.Cells["OfficerID"].Value?.ToString();

            if (string.IsNullOrEmpty(officerID))
            {
                MessageBox.Show("Could not find the profile ID. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete profile '{selectedRow.Cells["FullName"].Value}'?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM Users WHERE UserID = @OfficerID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@officerID", officerID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Officer account deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadOfficers();
                            txtSearch.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Officer account not found or already deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Refreshbtn_Click(object sender, EventArgs e)
        {
            if (isInEditMode)
            {
                var result = MessageBox.Show("You are currently in edit mode. Refreshing will discard any unsaved changes. Continue?", "Confirm Refresh", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            txtSearch.Clear();

            ToggleEditMode(false);
            LoadOfficers();
            MessageBox.Show("Officers list refreshed!", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void viewLockAccounts_Click(object sender, EventArgs e)
        {
            UnlockAccountsForm unlockAcct = new UnlockAccountsForm();
            unlockAcct.ShowDialog();
        }
    }
}