using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BCrypt.Net;
using System.Text.RegularExpressions;
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class loginform : Form
    {

        private const int MinimumPasswordLength = 8;

        public loginform()
        {
            InitializeComponent();
        }
        private void hide_Click(object sender, EventArgs e)
        {
            if (txtpassword.PasswordChar == '\0')
            {
                unhide.BringToFront();
                txtpassword.PasswordChar = '*';
            }
        }
        private void unhide_Click(object sender, EventArgs e)
        {
            if (txtpassword.PasswordChar == '*')
            {
                hide.BringToFront();
                txtpassword.PasswordChar = '\0';
            }
        }
        private void label5_Click(object sender, EventArgs e)
        {
            registerform registerform = new registerform();
            registerform.Show();
        }




        public static class CurrentUser
        {
            public static int UserId { get; set; }
            public static string Username { get; set; }
            public static string FullName { get; set; }
            public static string Role { get; set; }
            public static int RoleId { get; set; }
            public static List<string> Permissions { get; set; } = new List<string>();

            public static bool HasPermission(string permissionName)
            {
                return Permissions != null && Permissions.Contains(permissionName);
            }

            public static void Clear()
            {
                UserId = 0;
                Username = string.Empty;
                FullName = string.Empty;
                Role = string.Empty;
                RoleId = 0;
                Permissions.Clear();
            }
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string username = txtusername.Text.Trim();
            string password = txtpassword.Text;

            if (!ValidateInputs(username, password))
                return;

            try
            {
                if (AuthenticateUser(username, password))
                {
                    ShowDashboard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (password.Length < MinimumPasswordLength)
            {
                MessageBox.Show($"Password must be at least {MinimumPasswordLength} characters.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = @"SELECT u.UserID, u.Username, u.PasswordHash, u.Firstname, u.Lastname, 
                                u.RoleId, r.RoleName 
                         FROM Users u 
                         INNER JOIN TBL_Roles r ON u.RoleId = r.RoleId 
                         WHERE u.Username = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();

                            bool isValid = false;

                            // Special Dev account login (plain password)
                            if (reader["Username"].ToString() == "dev account" && password == "developer")
                            {
                                isValid = true;
                            }
                            else
                            {
                                // Normal users use hashed password
                                isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
                            }

                            if (!isValid)
                            {
                                MessageBox.Show("Invalid password.", "Login Failed",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            CurrentUser.UserId = Convert.ToInt32(reader["UserID"]);
                            CurrentUser.Username = reader["Username"].ToString();
                            CurrentUser.FullName = $"{reader["Firstname"]} {reader["Lastname"]}";
                            CurrentUser.RoleId = Convert.ToInt32(reader["RoleId"]);
                            CurrentUser.Role = reader["RoleName"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Username not found.", "Login Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }

                CurrentUser.Permissions = LoadUserPermissions(CurrentUser.RoleId);
            }

            return true;
        }

        private List<string> LoadUserPermissions(int roleId)
        {
            List<string> permissions = new List<string>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT p.PermissionName 
                                 FROM TBL_Permissions p
                                 INNER JOIN TBL_RolePermissions rp ON p.PermissionId = rp.PermissionId
                                 WHERE rp.RoleId = @roleId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@roleId", roleId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(reader["PermissionName"].ToString());
                        }
                    }
                }
            }
            return permissions;
        }

        private void ShowDashboard()
        {
            MessageBox.Show($"Welcome {CurrentUser.FullName} ({CurrentUser.Role})",
                "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dashboard dashboard = new dashboard();
            dashboard.Show();
            this.Hide();
        }

        private void loginform_Load(object sender, EventArgs e)
        {

        }

        private void lbForgotPass_Click(object sender, EventArgs e)
        {
            forgotpassfrm forgotpass = new forgotpassfrm();
            forgotpass.Show();
        }
    }
}
