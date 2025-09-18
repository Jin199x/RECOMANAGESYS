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

namespace RECOMANAGESYS
{
    public partial class OfficerInfo : Form
    {
        private DataTable originalDataTable;
        public OfficerInfo()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadOfficers();
        }
        private void SetupDataGridView()
        {
            DGVOfficers.AutoGenerateColumns = false;
            DGVOfficers.AllowUserToAddRows = false;
            DGVOfficers.AllowUserToDeleteRows = false;
            DGVOfficers.ReadOnly = false;
            DGVOfficers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVOfficers.MultiSelect = false;

            DGVOfficers.Columns.Clear();
            DGVOfficers.RowTemplate.Height = 35;

            // New column for the formatted ID
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DisplayID",
                HeaderText = "ID",
                Name = "DisplayID",
                Width = 50,
                ReadOnly = true
            });

            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FullName",
                HeaderText = "Name",
                Name = "FullName",
                Width = 150,
                ReadOnly = true
            });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CompleteAddress",
                HeaderText = "Complete Address",
                Name = "CompleteAddress",
                Width = 230,
                ReadOnly = false
            });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ContactNumber",
                HeaderText = "Contact Number",
                Name = "ContactNumber",
                Width = 120,
                ReadOnly = false
            });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MemberSince",
                HeaderText = "Member Since",
                Name = "MemberSince",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" },
                ReadOnly = false
            });
            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PositionInHOA",
                HeaderText = "Position in HOA",
                Name = "PositionInHOA",
                Width = 120,
                ReadOnly = false
            });

            DGVOfficers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OfficerID",
                HeaderText = "Officer ID",
                Name = "OfficerID",
                Visible = false,
                ReadOnly = true
            });
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Padding = new Padding(0, 5, 0, 5);
        }
        public void LoadOfficers()
        {
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
                             FROM
                                Users u
                             LEFT JOIN
                                TBL_Roles r ON u.RoleId = r.RoleId
                             ORDER BY u.Lastname, u.Firstname";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dt.Columns.Add("DisplayID", typeof(string));

                        foreach (DataRow row in dt.Rows)
                        {
                            // Changed from "ProfileID" to "OfficerID" to match your query
                            row["DisplayID"] = row["OfficerID"].ToString().PadLeft(3, '0'); // Also changed PadLeft to 3 for better formatting
                        }

                        DGVOfficers.ClearSelection();
                        originalDataTable = dt.Copy();
                        DGVOfficers.DataSource = dt;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void officerPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void OfficerInfo_Load(object sender, EventArgs e)
        {

        }

        private void registerbtn_Click(object sender, EventArgs e)
        {

            var regform = new registerform();


            regform.RegistrationSuccess += (s, args) =>
            {
                LoadOfficers();
            };

            regform.Show();


        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (DGVOfficers.SelectedRows.Count == 0 || DGVOfficers.SelectedRows[0].Index < 0)
            {
                MessageBox.Show("Please select a profile to delete.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = DGVOfficers.SelectedRows[0];
            string officerID = selectedRow.Cells["OfficerID"].Value?.ToString();

            if (string.IsNullOrEmpty(officerID))
            {
                MessageBox.Show("Could not find the profile ID. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete profile '{selectedRow.Cells["FullName"].Value}'?",
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

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
                            MessageBox.Show("Officer account deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadOfficers();
                        }
                        else
                        {
                            MessageBox.Show("Offficer account not found or already deleted.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting profile: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Refreshbtn_Click(object sender, EventArgs e)
        {
            LoadOfficers();
            MessageBox.Show("Officers list refreshed!", "Refresh",
                MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
    }
}
