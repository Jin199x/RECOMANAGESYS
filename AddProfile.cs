
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class ResidencyRegisterfrm : Form
    {
        private const int ContactNumberLength = 11;
       private int homeownerIdParsed = 0;
        private int? homeownerId = null;

        public ResidencyRegisterfrm()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            if (ResidentIDtxt != null && residentlbl != null)
            {
                ResidentIDtxt.Visible = true;
                residentlbl.Visible = true;
                ResidentIDtxt.ReadOnly = false;
            }

        }

        public ResidencyRegisterfrm(int editHomeownerId)
        {
            InitializeComponent();
            homeownerId = editHomeownerId;
            this.Text = "Edit Homeowner";
            LoadHomeownerData();

            if (ResidentIDtxt != null && residentlbl != null)
            {
                ResidentIDtxt.Visible = true;
                residentlbl.Visible = true;
                ResidentIDtxt.ReadOnly = true;
            }
        }

        private void AddProfile_Load(object sender, EventArgs e)
        {
            cmbType.Items.Clear();
            cmbType.Items.AddRange(new string[] { "Owner", "Tenant", "Caretaker" });
            cmbType.SelectedIndex = 0;
        }

        private void LoadHomeownerData()
        {
            if (!homeownerId.HasValue) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Residents WHERE HomeownerID = @id AND IsActive = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", homeownerId.Value);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ResidentIDtxt.Text = reader["HomeownerID"].ToString();
                        FirstNametxt.Text = reader["FirstName"].ToString();
                        MiddleNametxt.Text = reader["MiddleName"].ToString();
                        lastNametxt.Text = reader["LastName"].ToString();
                        addresstxt.Text = reader["HomeAddress"].ToString();
                        contactnumtxt.Text = reader["ContactNumber"].ToString();
                        Emailtxt.Text = reader["EmailAddress"].ToString();
                        emergencyPersontxt.Text = reader["EmergencyContactPerson"].ToString();
                        emergencyNumtxt.Text = reader["EmergencyContactNumber"].ToString();
                        cmbType.Text = reader["ResidencyType"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading homeowner data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateRegistrationInputs()
        {
            if (string.IsNullOrWhiteSpace(ResidentIDtxt.Text) ||
                !int.TryParse(ResidentIDtxt.Text.Trim(), out homeownerIdParsed))
            {
                MessageBox.Show("Please enter a valid numeric Homeowner ID.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResidentIDtxt.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(FirstNametxt.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FirstNametxt.Focus();
                return false;
            }


            if (string.IsNullOrWhiteSpace(lastNametxt.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNametxt.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(addresstxt.Text))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addresstxt.Focus();
                return false;
            }

            if (cmbType == null || string.IsNullOrWhiteSpace(cmbType.Text))
            {
                MessageBox.Show("Please select a residency type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbType.Focus();
                return false;
            }

            string email = Emailtxt.Text.Trim();
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Emailtxt.Focus();
                return false;
            }

            string contact = contactnumtxt.Text.Trim();
            if (!Regex.IsMatch(contact, @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Contact number must be exactly {ContactNumberLength} digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                contactnumtxt.Focus();
                return false;
            }

            string emergency = emergencyNumtxt.Text.Trim();
            if (!Regex.IsMatch(emergency, @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Emergency contact number must be exactly {ContactNumberLength} digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emergencyNumtxt.Focus();
                return false;
            }
          
            return true;
        }
        private void Addbtn_Click(object sender, EventArgs e)
        {

            if (!ValidateRegistrationInputs())
                return;

            try
            {
                int inputHomeownerId = Convert.ToInt32(ResidentIDtxt.Text.Trim());

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (homeownerId.HasValue)
                    {
                        if (!homeownerId.HasValue)
                        {
                            string checkDup = "SELECT COUNT(*) FROM Residents WHERE HomeownerID = @id";                      
                        }
                        query = @"UPDATE Residents SET
                            FirstName = @FirstName,
                            MiddleName = @MiddleName,
                            LastName = @LastName,
                            HomeAddress = @HomeAddress,
                            ContactNumber = @ContactNumber,
                            EmailAddress = @EmailAddress,
                            EmergencyContactPerson = @EmergencyContactPerson,
                            EmergencyContactNumber = @EmergencyContactNumber,
                            ResidencyType = @ResidencyType
                          WHERE HomeownerID = @HomeownerID";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@HomeownerID", homeownerId.Value);
                    }
                    else
                    {                      
                        query = @"INSERT INTO Residents
                            (HomeownerID, FirstName, MiddleName, LastName, HomeAddress, ContactNumber,
                             EmailAddress, EmergencyContactPerson, EmergencyContactNumber, ResidencyType, IsActive)
                          VALUES
                            (@HomeownerID, @FirstName, @MiddleName, @LastName, @HomeAddress, @ContactNumber,
                             @EmailAddress, @EmergencyContactPerson, @EmergencyContactNumber, @ResidencyType, 1)";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@HomeownerID", inputHomeownerId);
                    }

                    cmd.Parameters.AddWithValue("@FirstName", FirstNametxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(MiddleNametxt.Text) ? (object)DBNull.Value : MiddleNametxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", lastNametxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@HomeAddress", addresstxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@ContactNumber", contactnumtxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmailAddress", Emailtxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmergencyContactPerson", emergencyPersontxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmergencyContactNumber", emergencyNumtxt.Text.Trim());
                    cmd.Parameters.AddWithValue("@ResidencyType", cmbType.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string message = homeownerId.HasValue
                            ? "Homeowner updated successfully!"
                            : $"Homeowner registered successfully with ID: {inputHomeownerId}!";

                        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving homeowner: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearForm()
        {
            FirstNametxt.Clear();
            MiddleNametxt.Clear();
            lastNametxt.Clear();
            addresstxt.Clear();
            contactnumtxt.Clear();
            Emailtxt.Clear();
            emergencyPersontxt.Clear();
            emergencyNumtxt.Clear();
            ResidentIDtxt.Clear();

            cmbType.SelectedIndex = 0;
        }

        private void Clearbtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all fields?", "Confirm Clear",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private void cancelvisitor_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void ProfilePic_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void savevisitor_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }
        private void cmbPosition_SelectedIndexChanged(object sender, EventArgs e) {
            
                   if (cmbType.Text == "Caretaker")
            {
                cmbType.Enabled = false;
            }
            else
            {
                cmbType.Enabled = true;
            }
        }
    }
}