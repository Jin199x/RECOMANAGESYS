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
    public partial class UnlockAccountsForm : Form
    {
        public UnlockAccountsForm()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Setup columns
            dgvLockedAccounts.Columns.Clear();
            dgvLockedAccounts.Columns.Add("UserID", "UserID");
            dgvLockedAccounts.Columns.Add("Username", "Username");
            dgvLockedAccounts.Columns.Add("Firstname", "Firstname");
            dgvLockedAccounts.Columns.Add("Lastname", "Lastname");
            dgvLockedAccounts.Columns.Add("RoleName", "Role");
            // Make columns read-only
            foreach (DataGridViewColumn col in dgvLockedAccounts.Columns)
            {
                col.ReadOnly = true;
            }

            // Auto-size columns to fill the grid
            dgvLockedAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Fill the DataGridView immediately
            LoadLockedAccounts();

        }

        private void searchLock_TextChanged(object sender, EventArgs e)
        {
            LoadLockedAccounts(searchLock.Text.Trim());
        }
        private void LoadLockedAccounts(string search = "")
        {
            dgvLockedAccounts.Rows.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT u.UserID, u.Username, u.Firstname, u.Lastname, r.RoleName
                FROM Users u INNER JOIN TBL_Roles r ON u.RoleId = r.RoleId
                WHERE u.IsLocked = 1 
                AND (u.Username LIKE @search OR u.Firstname LIKE @search OR u.Lastname LIKE @search)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dgvLockedAccounts.Rows.Add(
                                reader["UserID"],
                                reader["Username"],
                                reader["Firstname"],
                                reader["Lastname"],
                                reader["RoleName"]
                            );
                        }
                    }
                }
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            if (dgvLockedAccounts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a user to unlock.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = Convert.ToInt32(dgvLockedAccounts.SelectedRows[0].Cells["UserID"].Value);

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Users 
                         SET IsLocked = 0, FailedLoginAttempts = 0 
                         WHERE UserID = @userId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("User account unlocked.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadLockedAccounts(); // Refresh the grid
        }

    }

}
