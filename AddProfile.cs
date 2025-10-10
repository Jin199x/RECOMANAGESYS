using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class ResidencyRegisterfrm : Form
    {
        private const int ContactNumberLength = 11;
        private int? editResidentId = null;
        private bool isEditMode = false;

        public ResidencyRegisterfrm()
        {
            InitializeComponent();

            cmbType.SelectedIndexChanged += cmbPosition_SelectedIndexChanged;
            cmbUnitNum.DropDown += cmbUnitNum_DropDown;
            cmbUnitNum.SelectedIndexChanged += cmbUnitNum_SelectedIndexChanged;

            this.AutoScaleMode = AutoScaleMode.Dpi;
            LoadResidencyTypes();
            SetupFormForAddMode();
        }

        public ResidencyRegisterfrm(int editResidentId)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            LoadResidencyTypes();
            this.editResidentId = editResidentId;
            isEditMode = true;

            this.Text = "Edit Resident";
            SetupFormForEditMode();
            LoadResidentData();
        }

        private void SetupFormForAddMode()
        {
            ResidentIDtxt.Visible = true;
            residentlbl.Visible = true;
            ResidentIDtxt.ReadOnly = false;
            UpdateResidentIDLabel();
        }

        private void SetupFormForEditMode()
        {
            ResidentIDtxt.Visible = true;
            residentlbl.Visible = true;
            ResidentIDtxt.ReadOnly = true;

            // 🔒 PREVENT EDITING RESIDENCY TYPE IN EDIT MODE
            cmbType.Enabled = false;
        }

        private void LoadResidencyTypes()
        {
            cmbType.Items.Clear();
            cmbType.Items.AddRange(new string[] { "Owner", "Tenant", "Caretaker" });

            if (!isEditMode && cmbType.Items.Count > 0)
                cmbType.SelectedIndex = 0;
        }

        private void UpdateResidentIDLabel()
        {
            if (isEditMode) return;

            if (cmbType.Text == "Owner")
            {
                residentlbl.Text = "Homeowner ID (New):";
                ResidentIDtxt.Text = "";
                ResidentIDtxt.Text = "Enter unique ID (e.g., 1001)";
            }
            else
            {
                residentlbl.Text = "Owner's Homeowner ID:";
                ResidentIDtxt.Text = "";
                ResidentIDtxt.Text = "Enter existing Owner's ID";
            }
        }

        private void LoadResidentData()
        {
            if (!editResidentId.HasValue) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Residents WHERE ResidentID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", editResidentId.Value);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ResidentIDtxt.Text = reader["HomeownerID"].ToString();
                        FirstNametxt.Text = reader["FirstName"].ToString();
                        MiddleNametxt.Text = reader["MiddleName"] == DBNull.Value ? "" : reader["MiddleName"].ToString();
                        lastNametxt.Text = reader["LastName"].ToString();
                        addresstxt.Text = reader["HomeAddress"].ToString();
                        contactnumtxt.Text = reader["ContactNumber"].ToString();
                        Emailtxt.Text = reader["EmailAddress"] == DBNull.Value ? "" : reader["EmailAddress"].ToString();
                        emergencyPersontxt.Text = reader["EmergencyContactPerson"] == DBNull.Value ? "" : reader["EmergencyContactPerson"].ToString();
                        emergencyNumtxt.Text = reader["EmergencyContactNumber"] == DBNull.Value ? "" : reader["EmergencyContactNumber"].ToString();
                        cmbType.Text = reader["ResidencyType"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading resident data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateRegistrationInputs(out int homeownerId)
        {
            homeownerId = 0;

            if (cmbType.SelectedItem == null)
            {
                MessageBox.Show("Please select a residency type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string residencyType = cmbType.Text.Trim();

            // Validate Homeowner ID
            if (string.IsNullOrWhiteSpace(ResidentIDtxt.Text) || !int.TryParse(ResidentIDtxt.Text.Trim(), out homeownerId))
            {
                MessageBox.Show("Please enter a valid Homeowner ID (numbers only).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate based on residency type
            if (residencyType.Equals("Owner", StringComparison.OrdinalIgnoreCase))
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM Residents
                        WHERE HomeownerID = @homeownerId
                          AND ResidencyType = 'Owner'
                          AND IsActive = 1
                          AND ResidentID <> @editResidentId;";

                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                        cmd.Parameters.AddWithValue("@editResidentId", isEditMode ? editResidentId.Value : -1);

                        int existingOwnerCount = Convert.ToInt32(cmd.ExecuteScalar());

                        // 🩵 Only show warning if adding (not editing)
                        if (!isEditMode && existingOwnerCount > 0)
                        {
                            MessageBox.Show(
                                "This Homeowner ID is already registered to another Owner.\n\nPlease use a different unique ID.",
                                "Duplicate Homeowner ID",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            else // Tenant or Caretaker
            {
                // Verify Owner exists with this HomeownerID
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string checkOwnerQuery = @"
                        SELECT COUNT(*) 
                        FROM Residents 
                        WHERE HomeownerID = @OwnerID 
                        AND ResidencyType = 'Owner' 
                        AND IsActive = 1";

                    SqlCommand checkCmd = new SqlCommand(checkOwnerQuery, conn);
                    checkCmd.Parameters.AddWithValue("@OwnerID", homeownerId);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists == 0)
                    {
                        MessageBox.Show(
                            $"No active Owner found with Homeowner ID: {homeownerId}\n\n" +
                            "Tenants and Caretakers must be registered under an existing Owner's ID.\n" +
                            "Please verify the Owner's Homeowner ID and try again.",
                            "Invalid Owner ID",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return false;
                    }

                    // Show Owner info for confirmation
                    string getOwnerQuery = "SELECT FirstName, LastName FROM Residents WHERE HomeownerID = @id AND ResidencyType = 'Owner'";
                    SqlCommand ownerCmd = new SqlCommand(getOwnerQuery, conn);
                    ownerCmd.Parameters.AddWithValue("@id", homeownerId);
                    using (SqlDataReader reader = ownerCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string ownerName = $"{reader["FirstName"]} {reader["LastName"]}";
                            var result = MessageBox.Show(
                                $"You are registering this {residencyType} under:\n\n" +
                                $"Owner: {ownerName}\n" +
                                $"Homeowner ID: {homeownerId}\n\n" +
                                "Is this correct?",
                                "Confirm Owner",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result != DialogResult.Yes)
                                return false;
                        }
                    }
                }
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstNametxt.Text) ||
                string.IsNullOrWhiteSpace(lastNametxt.Text) ||
                string.IsNullOrWhiteSpace(addresstxt.Text))
            {
                MessageBox.Show("First name, Last name, and Address are required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate contact numbers
            string contact = contactnumtxt.Text.Trim();
            if (!Regex.IsMatch(contact, @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Contact number must be exactly {ContactNumberLength} digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string emergency = emergencyNumtxt.Text.Trim();
            if (!string.IsNullOrWhiteSpace(emergency) && !Regex.IsMatch(emergency, @"^\d{" + ContactNumberLength + "}$"))
            {
                MessageBox.Show($"Emergency contact number must be exactly {ContactNumberLength} digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            if (!ValidateRegistrationInputs(out int homeownerId))
                return;

            try
            {
                string residencyType = cmbType.Text.Trim();

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // ✅ NEW: For Tenant/Caretaker, validate apartment availability BEFORE saving
                    if (!isEditMode && (residencyType == "Tenant" || residencyType == "Caretaker"))
                    {
                        if (!(cmbUnitNum.SelectedItem is ComboBoxItem selectedUnit))
                        {
                            MessageBox.Show("Please select a unit before saving.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int selectedUnitId = selectedUnit.Value;

                        // ✅ CRITICAL: Check available rooms BEFORE inserting resident
                        if (residencyType == "Tenant")
                        {
                            string checkRoomsQuery = @"
                        SELECT UnitType, AvailableRooms, TotalRooms 
                        FROM TBL_Units 
                        WHERE UnitID = @unitId";

                            using (SqlCommand checkCmd = new SqlCommand(checkRoomsQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@unitId", selectedUnitId);

                                using (SqlDataReader reader = checkCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string unitType = reader["UnitType"]?.ToString() ?? "";

                                        if (unitType.Equals("Apartment", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int availableRooms = reader["AvailableRooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AvailableRooms"]);
                                            int totalRooms = reader["TotalRooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalRooms"]);

                                            if (availableRooms <= 0)
                                            {
                                                reader.Close();
                                                MessageBox.Show(
                                                    $"This apartment is already full.\n\n" +
                                                    $"Total Rooms: {totalRooms}\n" +
                                                    $"Available Rooms: {availableRooms}\n\n" +
                                                    $"Cannot register new tenant. Please select a different unit.",
                                                    "Apartment Fully Occupied",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Warning

                                                );
                                                return;
                                            }


                                        }
                                    }
                                }
                            }
                        }

                        }

                        SqlCommand cmd;

                            if (isEditMode)
                            {
                                // Edit mode - only update personal info
                                cmd = new SqlCommand(@"
                    UPDATE Residents
                    SET FirstName = @FirstName,
                        MiddleName = @MiddleName,
                        LastName = @LastName,
                        HomeAddress = @Address,
                        ContactNumber = @ContactNumber,
                        EmailAddress = @Email,
                        EmergencyContactPerson = @EmergencyContactPerson,
                        EmergencyContactNumber = @EmergencyContactNumber
                    WHERE ResidentID = @ResidentID", conn);

                                cmd.Parameters.AddWithValue("@ResidentID", editResidentId.Value);
                            }
                            else
                            {
                                // Add mode - insert new resident
                                cmd = new SqlCommand(@"
                    INSERT INTO Residents
                        (HomeownerID, FirstName, MiddleName, LastName, HomeAddress,
                         ContactNumber, EmailAddress, EmergencyContactPerson,
                         EmergencyContactNumber, ResidencyType)
                    VALUES
                        (@HomeownerID, @FirstName, @MiddleName, @LastName, @Address,
                         @ContactNumber, @Email, @EmergencyContactPerson,
                         @EmergencyContactNumber, @ResidencyType)", conn);

                                cmd.Parameters.AddWithValue("@HomeownerID", homeownerId);
                                cmd.Parameters.AddWithValue("@ResidencyType", residencyType);
                            }

                            cmd.Parameters.AddWithValue("@FirstName", FirstNametxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrWhiteSpace(MiddleNametxt.Text) ? (object)DBNull.Value : MiddleNametxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", lastNametxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@Address", addresstxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@ContactNumber", contactnumtxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(Emailtxt.Text) ? (object)DBNull.Value : Emailtxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmergencyContactPerson", string.IsNullOrWhiteSpace(emergencyPersontxt.Text) ? (object)DBNull.Value : emergencyPersontxt.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmergencyContactNumber", string.IsNullOrWhiteSpace(emergencyNumtxt.Text) ? (object)DBNull.Value : emergencyNumtxt.Text.Trim());

                            cmd.ExecuteNonQuery();

                            int newResidentId = 0;

                            if (!isEditMode)
                            {
                                using (SqlCommand getNewId = new SqlCommand("SELECT IDENT_CURRENT('Residents')", conn))
                                {
                                    newResidentId = Convert.ToInt32(getNewId.ExecuteScalar());
                                }
                            }
                            else
                            {
                                newResidentId = editResidentId.Value;
                            }

                            // Only link unit for new Tenant/Caretaker registrations
                            if (!isEditMode && (residencyType == "Tenant" || residencyType == "Caretaker"))
                            {
                                if (!(cmbUnitNum.SelectedItem is ComboBoxItem selectedUnit))
                                {
                                    MessageBox.Show("Please select a unit before saving.", "Validation",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                int selectedUnitId = selectedUnit.Value;

                                // Link new resident to the selected unit
                                string linkQuery = @"
                    INSERT INTO HomeownerUnits (ResidentID, UnitID, IsCurrent, DateOfOwnership) 
                    VALUES (@newResidentId, @unitId, 1, GETDATE());";
                                using (SqlCommand linkCmd = new SqlCommand(linkQuery, conn))
                                {
                                    linkCmd.Parameters.AddWithValue("@newResidentId", newResidentId);
                                    linkCmd.Parameters.AddWithValue("@unitId", selectedUnitId);
                                    linkCmd.ExecuteNonQuery();
                                }

                                // Only deduct apartment room if residency type is Tenant
                                if (residencyType.Equals("Tenant", StringComparison.OrdinalIgnoreCase))
                                {
                                    string updateRoomsQuery = @"
                        UPDATE TBL_Units
                        SET AvailableRooms = CASE 
                            WHEN AvailableRooms > 0 THEN AvailableRooms - 1 
                            ELSE 0 
                        END
                        WHERE UnitID = @unitId AND UnitType = 'Apartment';";
                                    using (SqlCommand updateCmd = new SqlCommand(updateRoomsQuery, conn))
                                    {
                                        updateCmd.Parameters.AddWithValue("@unitId", selectedUnitId);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }

                                // Update IsOccupied based on available rooms
                                string updateStatusQuery = @"
                    UPDATE TBL_Units
                    SET IsOccupied = CASE 
                        WHEN UnitType = 'Apartment' AND ISNULL(AvailableRooms,0) = 0 THEN 1 
                        WHEN UnitType != 'Apartment' THEN 1
                        ELSE IsOccupied 
                    END
                    WHERE UnitID = @unitId;";
                                using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, conn))
                                {
                                    updateStatusCmd.Parameters.AddWithValue("@unitId", selectedUnitId);
                                    updateStatusCmd.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show(
                                isEditMode ? "Resident updated successfully!" : "Resident registered successfully!",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving resident: {ex.Message}", "Error",
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
            if (!isEditMode)
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

        private void cmbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cmbType.SelectedItem?.ToString() ?? "";

            bool isTenantOrCaretaker = selectedType.Equals("Tenant", StringComparison.OrdinalIgnoreCase)
                                      || selectedType.Equals("Caretaker", StringComparison.OrdinalIgnoreCase);

            // Enable unit selection only for tenant/caretaker
            cmbUnitNum.Enabled = isTenantOrCaretaker;
            cmbBlock.Enabled = isTenantOrCaretaker;

            // Reset validation text
            if (lblValidation != null) lblValidation.Text = "";

            if (!isTenantOrCaretaker)
            {
                cmbUnitNum.Items.Clear();
                cmbBlock.Items.Clear();
                cmbUnitNum.SelectedIndex = -1;
                cmbBlock.SelectedIndex = -1;
            }

            // Update the HomeownerID label hint text
            UpdateResidentIDLabel();
        }

        private void AddProfile_Load(object sender, EventArgs e)
        {
            cmbUnitNum.Enabled = false;
            cmbBlock.Enabled = false;
            if (lblValidation != null) lblValidation.Text = "";
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;
        }

        private void cmbUnitNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnitNum.SelectedItem is ComboBoxItem item)
            {
                try
                {
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string q = "SELECT Block FROM TBL_Units WHERE UnitID = @unitId";
                        using (SqlCommand cmd = new SqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("@unitId", item.Value);
                            object o = cmd.ExecuteScalar();
                            if (o != null && o != DBNull.Value)
                            {
                                string block = o.ToString();
                                cmbBlock.Items.Clear();
                                cmbBlock.Items.Add(block);
                                cmbBlock.SelectedIndex = 0;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void cmbUnitNum_DropDown(object sender, EventArgs e)
        {
            lblValidation.Text = "";
            if (string.IsNullOrWhiteSpace(ResidentIDtxt.Text) || !int.TryParse(ResidentIDtxt.Text.Trim(), out int homeownerId))
            {
                lblValidation.ForeColor = System.Drawing.Color.Red;
                lblValidation.Text = "Please input homeowner ID before picking unit number.";
                ((ComboBox)sender).DroppedDown = false;
                return;
            }

            LoadAvailableUnitsForHomeowner(homeownerId);
        }

        private void LoadAvailableUnitsForHomeowner(int homeownerId)
        {
            try
            {
                cmbUnitNum.Items.Clear();
                cmbBlock.Items.Clear();

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // ✅ FIXED: Removed SELECT DISTINCT and added TRY_CAST to SELECT list
                    string query = @"
                        SELECT 
                            tu.UnitID, 
                            tu.UnitNumber, 
                            tu.Block, 
                            tu.UnitType, 
                            tu.AvailableRooms,
                            TRY_CAST(tu.UnitNumber AS INT) AS UnitNumberInt
                        FROM HomeownerUnits hu
                        INNER JOIN Residents r ON hu.ResidentID = r.ResidentID
                        INNER JOIN TBL_Units tu ON hu.UnitID = tu.UnitID
                        WHERE r.HomeownerID = @homeownerId
                          AND r.ResidencyType = 'Owner'
                          AND hu.IsCurrent = 1
                        GROUP BY tu.UnitID, tu.UnitNumber, tu.Block, tu.UnitType, tu.AvailableRooms
                        ORDER BY UnitNumberInt, tu.UnitNumber;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int unitId = Convert.ToInt32(reader["UnitID"]);
                                string unitNumber = reader["UnitNumber"]?.ToString() ?? "";
                                string block = reader["Block"]?.ToString() ?? "";
                                string unitType = reader["UnitType"]?.ToString() ?? "";
                                int availableRooms = reader["AvailableRooms"] == DBNull.Value ? -1 : Convert.ToInt32(reader["AvailableRooms"]);

                                string label = unitNumber + (string.IsNullOrWhiteSpace(block) ? "" : $" (Block {block})");
                                if (unitType.Equals("Apartment", StringComparison.OrdinalIgnoreCase) && availableRooms >= 0)
                                {
                                    label += $" - {availableRooms} rooms left";
                                }

                                cmbUnitNum.Items.Add(new ComboBoxItem { Text = label, Value = unitId });
                            }
                        }
                    }
                }

                if (cmbUnitNum.Items.Count == 0)
                {
                    lblValidation.ForeColor = System.Drawing.Color.Red;
                    lblValidation.Text = "No active units found for this homeowner.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading units: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResidentIDtxt_TextChanged(object sender, EventArgs e)
        {
            if (ResidentIDtxt.Text.Contains("Enter"))
                ResidentIDtxt.Clear();
        }

        // Placeholder event handlers
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void ProfilePic_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void savevisitor_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void cmbBlock_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblValidation_Click(object sender, EventArgs e) { }
    }
}