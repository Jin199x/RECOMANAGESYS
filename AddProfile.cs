
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class ResidencyRegisterfrm : Form
    {
        private const int ContactNumberLength = 11;

        private int? homeownerId = null;

        public ResidencyRegisterfrm()
        {
            InitializeComponent();
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
                 string.IsNullOrWhiteSpace(FirstNametxt.Text) ||
                 string.IsNullOrWhiteSpace(lastNametxt.Text) ||
                 string.IsNullOrWhiteSpace(addresstxt.Text) ||
                 string.IsNullOrWhiteSpace(emergencyNumtxt.Text) ||
                 string.IsNullOrWhiteSpace(emergencyPersontxt.Text) ||
                 string.IsNullOrWhiteSpace(Emailtxt.Text) ||
                 string.IsNullOrWhiteSpace(contactnumtxt.Text) ||
                 cmbType.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(Emailtxt.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(contactnumtxt.Text.Trim(), @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Contact number must be exactly {ContactNumberLength} digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Regex.IsMatch(emergencyNumtxt.Text.Trim(), @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Emergency contact number must be exactly {ContactNumberLength} digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (!homeownerId.HasValue)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Residents WHERE HomeownerID = @id";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@id", inputHomeownerId);

                        int idExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (idExists > 0)
                        {
                            MessageBox.Show("This Resident ID already exists. Please use a different ID.", "Duplicate ID",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string query;
                    SqlCommand cmd;

                    if (homeownerId.HasValue)
                    {
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
                          EmailAddress, EmergencyContactPerson,
                          EmergencyContactNumber, ResidencyType, IsActive)
                         VALUES
                         (@HomeownerID, @FirstName, @MiddleName, @LastName, @HomeAddress, @ContactNumber,
                          @EmailAddress, @EmergencyContactPerson,
                          @EmergencyContactNumber, @ResidencyType, 1)";

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
                        string message = homeownerId.HasValue ?
                            "Homeowner updated successfully!" :
                            $"Homeowner registered successfully with ID: {inputHomeownerId}!";

                        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.", "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving homeowner: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void cmbPosition_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}