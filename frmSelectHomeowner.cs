using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class frmSelectHomeowner : Form
    {
        public int SelectedHomeownerId { get; private set; } = -1;

        // A class to hold our owner data, which is better for DataGridView
        private class OwnerDisplay
        {
            public int HomeownerId { get; set; }
            public string FullName { get; set; }
        }

        // A list to hold all owners, making filtering very fast
        private List<OwnerDisplay> allOwners = new List<OwnerDisplay>();

        public frmSelectHomeowner()
        {
            InitializeComponent();
        }

        private void frmSelectHomeowner_Load(object sender, EventArgs e)
        {
            // This prevents columns from being automatically generated
            dgvOwners.AutoGenerateColumns = false;

            // Define the columns manually
            dgvOwners.Columns.Clear();
            dgvOwners.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HomeownerId",
                HeaderText = "Homeowner ID",
                DataPropertyName = "HomeownerId",
                Width = 150 // This is the fixed width
            });

            dgvOwners.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Owner's Full Name",
                DataPropertyName = "FullName"
                // This column will be set to stretch
            });

            PopulateOwnersList();

            // --- THIS IS THE KEY FIX FOR STRETCHING using DataGridView ---
            // The "Fill" mode makes this column automatically take up all remaining space.
            // This is the standard, reliable way to do it.
            dgvOwners.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // --- END OF FIX ---
        }

        private void PopulateOwnersList()
        {
            allOwners.Clear();

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT HomeownerID, FirstName, MiddleName, LastName 
                                     FROM Residents 
                                     WHERE IsActive = 1 AND ResidencyType = 'Owner' 
                                     ORDER BY LastName, FirstName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allOwners.Add(new OwnerDisplay
                                {
                                    HomeownerId = Convert.ToInt32(reader["HomeownerID"]),
                                    FullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}".Trim()
                                });
                            }
                        }
                    }
                }
                // Bind the complete list of owners to the DataGridView
                dgvOwners.DataSource = allOwners;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading homeowner list: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // If search is empty, show all owners
                dgvOwners.DataSource = allOwners;
            }
            else
            {
                // Use LINQ to filter the list in memory - very fast
                var filteredList = allOwners.Where(owner =>
                    owner.HomeownerId.ToString().Contains(keyword) ||
                    owner.FullName.ToLower().Contains(keyword)
                ).ToList();

                // Show only the filtered results
                dgvOwners.DataSource = filteredList;
            }
        }

        private void SelectAndClose()
        {
            if (dgvOwners.SelectedRows.Count > 0)
            {
                // Get the value from the selected row's "HomeownerId" cell
                this.SelectedHomeownerId = (int)dgvOwners.SelectedRows[0].Cells["HomeownerId"].Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a homeowner from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectAndClose();
        }

        private void dgvOwners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-clicking anywhere on a row will select it
            SelectAndClose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}