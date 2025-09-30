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
    public partial class forgotpassfrm : Form
    {
        private const int MinimumPasswordLength = 8;
        public forgotpassfrm()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void forgotpassfrm_Load(object sender, EventArgs e)
        {

        }

        private void save_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }
            string username = forgotusername.Text.Trim();
            if (!UsernameExists(username))
            {
                MessageBox.Show("Username does not exist.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                ResetUserPassword(forgotusername.Text.Trim(), newpass.Text);
                MessageBox.Show("Password has been reset successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool UsernameExists(string username)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int)cmd.ExecuteScalar() > 0;

                }
            }
        }
        private bool ValidateInputs()
        {

            if (string.IsNullOrWhiteSpace(forgotusername.Text))
            {
                MessageBox.Show("Please enter your username.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                forgotusername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(newpass.Text))
            {
                MessageBox.Show("Please enter a new password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                newpass.Focus();
                return false;
            }

            if (newpass.Text.Length < MinimumPasswordLength)
            {
                MessageBox.Show($"Password must be at least {MinimumPasswordLength} characters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                newpass.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(confirmpass.Text))
            {
                MessageBox.Show("Please confirm your password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmpass.Focus();
                return false;
            }

            if (newpass.Text != confirmpass.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmpass.Focus();
                return false;
            }

            return true;
        }
        private void ResetUserPassword(string username, string newPassword)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        UpdatePassword(conn, transaction, username, hashedPassword);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private void UpdatePassword(SqlConnection conn, SqlTransaction transaction, string username, string hashedPassword)
        {
            string updateQuery = "UPDATE Users SET PasswordHash = @password WHERE Username = @username";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@username", username);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("No user was updated. The username might not exist.");
                }
            }
        }
    }
}
