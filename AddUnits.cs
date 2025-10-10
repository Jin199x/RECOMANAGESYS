using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class AddUnits : Form
    {
        private int _residentId;      // ResidentID of the person we're adding a unit for
        private int _homeownerId;     // HomeownerID (group identifier)
        private string _residencyType;

        public AddUnits(int residentId, int homeownerId, string residencyType)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            _residentId = residentId;
            _homeownerId = homeownerId;
            _residencyType = residencyType;
            this.Text = "Add New Unit";

            cmbUnitType.SelectedIndexChanged += cmbUnitType_SelectedIndexChanged;
            cmbNumRooms.Enabled = false;

            if (HomeownerID != null)
            {
                HomeownerID.Text = homeownerId.ToString();
                HomeownerID.ReadOnly = true;
                LoadResidentInfo(residentId);
            }
        }

        private void AddUnits_Load(object sender, EventArgs e)
        {
            try
            {
                if (cmbUnitType != null)
                {
                    cmbUnitType.Items.Clear();
                    cmbUnitType.Items.AddRange(new string[] { "Town house", "Single Attach", "Single Detach", "Apartment" });
                    if (cmbUnitType.Items.Count > 0)
                        cmbUnitType.SelectedIndex = 0;
                }

                if (cmbNumRooms != null)
                {
                    cmbNumRooms.Items.Clear();
                    for (int i = 1; i <= 10; i++)
                    {
                        cmbNumRooms.Items.Add(i.ToString());
                    }
                    cmbNumRooms.Enabled = false;
                }

                LoadApprovedByUsers();

                if (_residencyType != "Owner")
                {
                    cmbApprovedBy.Enabled = false;
                    DTPOwnership.Enabled = false;
                }
                else
                {
                    cmbApprovedBy.Enabled = true;
                    DTPOwnership.Enabled = true;
                }

                HomeownerID.Text = _homeownerId.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbNumRooms.Enabled = (_residencyType == "Owner" && cmbUnitType.Text == "Apartment");
        }

        private void LoadResidentInfo(int residentId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT FirstName, LastName, ResidencyType,
                               (SELECT COUNT(*) FROM HomeownerUnits WHERE ResidentID = @id AND IsCurrent = 1) as CurrentUnits
                        FROM Residents WHERE ResidentID = @id AND IsActive = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", residentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string firstName = reader["FirstName"]?.ToString() ?? "";
                            string lastName = reader["LastName"]?.ToString() ?? "";
                            string residentName = $"{firstName} {lastName}".Trim();
                            string resType = reader["ResidencyType"]?.ToString() ?? "";
                            int currentUnits = Convert.ToInt32(reader["CurrentUnits"]);

                            if (lblHomeownerInfo != null)
                            {
                                lblHomeownerInfo.Text = $"{resType}: {residentName} | Current Units: {currentUnits}";
                                lblHomeownerInfo.ForeColor = System.Drawing.Color.Green;
                            }
                        }
                        else
                        {
                            if (lblHomeownerInfo != null)
                            {
                                lblHomeownerInfo.Text = "Resident not found";
                                lblHomeownerInfo.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (lblHomeownerInfo != null)
                {
                    lblHomeownerInfo.Text = $"Error loading resident info: {ex.Message}";
                    lblHomeownerInfo.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        public class ListItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ListItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void LoadApprovedByUsers()
        {
            try
            {
                if (cmbApprovedBy != null)
                {
                    cmbApprovedBy.Items.Clear();
                    cmbApprovedBy.Items.Add(new ListItem("-- Select Approver --", 0));

                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string query = @"
                            SELECT u.UserID, u.Username, r.RoleName 
                            FROM Users u 
                            INNER JOIN TBL_Roles r ON u.RoleID = r.RoleID 
                            WHERE u.IsActive = 1 
                            ORDER BY r.RoleName, u.Username";

                        SqlCommand cmd = new SqlCommand(query, conn);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["UserID"]);
                                string roleName = reader["RoleName"]?.ToString() ?? "No Role";
                                string username = reader["Username"]?.ToString() ?? "";
                                string displayText = $"{roleName}";

                                cmbApprovedBy.Items.Add(new ListItem(displayText, userId));
                            }
                        }
                    }

                    cmbApprovedBy.DisplayMember = "Text";
                    cmbApprovedBy.ValueMember = "Value";
                    cmbApprovedBy.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {

            if (string.IsNullOrWhiteSpace(unitNumbertxt.Text))
            {
                MessageBox.Show("Please enter a unit number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                unitNumbertxt.Focus();
                return false;
            }

            if (cmbUnitType == null || cmbUnitType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a unit type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbUnitType.Focus();
                return false;
            }

            string unitTypeText = cmbUnitType.SelectedItem.ToString();

            if (unitTypeText == "Apartment")
            {
                if (_residencyType == "Owner")
                {
                    if (cmbNumRooms == null || cmbNumRooms.SelectedIndex == -1 ||
                        !int.TryParse(cmbNumRooms.Text, out int nr) || nr <= 0)
                    {
                        MessageBox.Show("Please select valid number of rooms for the apartment.", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbNumRooms.Focus();
                        return false;
                    }
                }
            }

            if (_residencyType == "Owner")
            {
                if (cmbApprovedBy == null || cmbApprovedBy.SelectedItem == null)
                {
                    MessageBox.Show("Please select an approver.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbApprovedBy.Focus();
                    return false;
                }
                ListItem li = cmbApprovedBy.SelectedItem as ListItem;
                if (li == null || li.Value == 0)
                {
                    MessageBox.Show("Please select a valid approver (not default).", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbApprovedBy.Focus();
                    return false;
                }
            }

            return true;
        }
        private void Savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Verify resident exists
                            string checkQuery = "SELECT COUNT(*) FROM Residents WHERE ResidentID = @id AND IsActive = 1";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@id", _residentId);
                                int residentExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                                if (residentExists == 0)
                                {
                                    MessageBox.Show("Resident ID does not exist", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    transaction.Rollback();
                                    return;
                                }
                            }

                            int unitId = -1;
                            string unitNumber = unitNumbertxt.Text.Trim();
                            object blockVal = string.IsNullOrWhiteSpace(blocktxt?.Text) ? (object)DBNull.Value : blocktxt.Text.Trim();
                            string unitTypeText = cmbUnitType?.Text ?? "";

                            int numRooms = 0;
                            if (cmbNumRooms != null && cmbNumRooms.Enabled)
                            {
                                int.TryParse(cmbNumRooms.Text, out numRooms);
                            }

                            if (_residencyType == "Owner")
                            {
                                // Owner can create new units or use existing ones
                                string findUnitQuery = @"SELECT UnitID FROM TBL_Units WHERE UnitNumber = @unitNumber AND Block = @block";
                                using (SqlCommand findUnitCmd = new SqlCommand(findUnitQuery, conn, transaction))
                                {
                                    findUnitCmd.Parameters.AddWithValue("@unitNumber", unitNumber);
                                    findUnitCmd.Parameters.AddWithValue("@block", blockVal);
                                    object existingUnit = findUnitCmd.ExecuteScalar();

                                    if (existingUnit != null && existingUnit != DBNull.Value)
                                    {
                                        unitId = Convert.ToInt32(existingUnit);

                                        // ✅ FIXED: Check if THIS OWNER already owns this unit
                                        string checkOwnerQuery = @"
                                            SELECT COUNT(*) 
                                            FROM HomeownerUnits hu
                                            JOIN Residents r ON hu.ResidentID = r.ResidentID
                                            WHERE hu.UnitID = @unitId 
                                              AND r.HomeownerID = @homeownerId
                                              AND r.ResidencyType = 'Owner'
                                              AND hu.IsCurrent = 1";
                                        using (SqlCommand checkOwnerCmd = new SqlCommand(checkOwnerQuery, conn, transaction))
                                        {
                                            checkOwnerCmd.Parameters.AddWithValue("@unitId", unitId);
                                            checkOwnerCmd.Parameters.AddWithValue("@homeownerId", _homeownerId);
                                            int existingOwner = Convert.ToInt32(checkOwnerCmd.ExecuteScalar());
                                            if (existingOwner > 0)
                                            {
                                                MessageBox.Show("This owner already owns this unit.", "Duplicate Unit",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                transaction.Rollback();
                                                return;
                                            }
                                        }

                                        // ✅ ADDITIONAL CHECK: Prevent different owners from owning the same non-apartment unit
                                        if (unitTypeText != "Apartment")
                                        {
                                            string checkDifferentOwnerQuery = @"
                                                SELECT COUNT(*) 
                                                FROM HomeownerUnits hu
                                                JOIN Residents r ON hu.ResidentID = r.ResidentID
                                                WHERE hu.UnitID = @unitId 
                                                  AND r.HomeownerID <> @homeownerId
                                                  AND r.ResidencyType = 'Owner'
                                                  AND hu.IsCurrent = 1";
                                            using (SqlCommand checkDiffOwnerCmd = new SqlCommand(checkDifferentOwnerQuery, conn, transaction))
                                            {
                                                checkDiffOwnerCmd.Parameters.AddWithValue("@unitId", unitId);
                                                checkDiffOwnerCmd.Parameters.AddWithValue("@homeownerId", _homeownerId);
                                                int differentOwner = Convert.ToInt32(checkDiffOwnerCmd.ExecuteScalar());
                                                if (differentOwner > 0)
                                                {
                                                    MessageBox.Show("This unit is already owned by a different owner.", "Unit Already Owned",
                                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                    transaction.Rollback();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Create new unit
                                        string insertUnitQuery = @"
                                            INSERT INTO TBL_Units (UnitNumber, Block, UnitType, IsOccupied, TotalRooms, AvailableRooms)
                                            VALUES (@unitNumber, @block, @unitType, @isOccupied, @totalRooms, @availableRooms);
                                            SELECT SCOPE_IDENTITY();";
                                        using (SqlCommand insertUnitCmd = new SqlCommand(insertUnitQuery, conn, transaction))
                                        {
                                            insertUnitCmd.Parameters.AddWithValue("@unitNumber", unitNumber);
                                            insertUnitCmd.Parameters.AddWithValue("@block", blockVal);
                                            insertUnitCmd.Parameters.AddWithValue("@unitType", unitTypeText);
                                            insertUnitCmd.Parameters.AddWithValue("@isOccupied", 0);

                                            int totalRooms = 0;
                                            int availableRooms = 0;
                                            if (unitTypeText == "Apartment")
                                            {
                                                totalRooms = Math.Max(0, numRooms);
                                                availableRooms = totalRooms;
                                            }

                                            insertUnitCmd.Parameters.AddWithValue("@totalRooms",
                                                totalRooms > 0 ? (object)totalRooms : DBNull.Value);
                                            insertUnitCmd.Parameters.AddWithValue("@availableRooms",
                                                availableRooms > 0 ? (object)availableRooms : DBNull.Value);

                                            object sc = insertUnitCmd.ExecuteScalar();
                                            unitId = Convert.ToInt32(sc);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Tenant/Caretaker must use existing units
                                string findUnitQuery = @"SELECT UnitID, UnitType FROM TBL_Units WHERE UnitNumber = @unitNumber AND Block = @block";
                                using (SqlCommand findUnitCmd = new SqlCommand(findUnitQuery, conn, transaction))
                                {
                                    findUnitCmd.Parameters.AddWithValue("@unitNumber", unitNumber);
                                    findUnitCmd.Parameters.AddWithValue("@block", blockVal);
                                    using (SqlDataReader rdr = findUnitCmd.ExecuteReader())
                                    {
                                        if (!rdr.Read())
                                        {
                                            MessageBox.Show("This unit does not exist. Please create the unit first (Owner registration) or select existing unit.",
                                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            transaction.Rollback();
                                            return;
                                        }
                                        unitId = Convert.ToInt32(rdr["UnitID"]);
                                        unitTypeText = rdr["UnitType"]?.ToString() ?? unitTypeText;
                                    }
                                }

                                // Check apartment availability for Tenants
                                if (unitTypeText == "Apartment" && _residencyType == "Tenant")
                                {
                                    string checkAvailQuery = "SELECT ISNULL(AvailableRooms, 0) FROM TBL_Units WHERE UnitID = @unitId";
                                    using (SqlCommand checkAvail = new SqlCommand(checkAvailQuery, conn, transaction))
                                    {
                                        checkAvail.Parameters.AddWithValue("@unitId", unitId);
                                        int avail = Convert.ToInt32(checkAvail.ExecuteScalar());
                                        if (avail <= 0)
                                        {
                                            MessageBox.Show("No available rooms in this apartment.", "No Rooms Available",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            transaction.Rollback();
                                            return;
                                        }
                                    }
                                }

                                // ✅ FIXED: For Tenant/Caretaker, verify they're being assigned to their owner's unit
                                string verifyOwnershipQuery = @"
                                    SELECT COUNT(*)
                                    FROM HomeownerUnits hu
                                    INNER JOIN Residents r ON hu.ResidentID = r.ResidentID
                                    WHERE hu.UnitID = @unitId
                                      AND r.HomeownerID = @homeownerId
                                      AND r.ResidencyType = 'Owner'
                                      AND hu.IsCurrent = 1";
                                using (SqlCommand verifyCmd = new SqlCommand(verifyOwnershipQuery, conn, transaction))
                                {
                                    verifyCmd.Parameters.AddWithValue("@unitId", unitId);
                                    verifyCmd.Parameters.AddWithValue("@homeownerId", _homeownerId);
                                    int ownerHasUnit = Convert.ToInt32(verifyCmd.ExecuteScalar());

                                    if (ownerHasUnit == 0)
                                    {
                                        MessageBox.Show(
                                            $"This unit is not owned by the homeowner (ID: {_homeownerId}).\n\n" +
                                            "Tenants and Caretakers can only be assigned to units owned by their associated owner.",
                                            "Invalid Unit Assignment",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                                        transaction.Rollback();
                                        return;
                                    }
                                }
                            }

                            // Get unit type if not already set
                            if (string.IsNullOrEmpty(unitTypeText))
                            {
                                string getUnitTypeQuery = "SELECT UnitType FROM TBL_Units WHERE UnitID = @unitId";
                                using (SqlCommand typeCmd = new SqlCommand(getUnitTypeQuery, conn, transaction))
                                {
                                    typeCmd.Parameters.AddWithValue("@unitId", unitId);
                                    unitTypeText = (string)typeCmd.ExecuteScalar();
                                }
                            }

                            // ✅ FIXED: Check for duplicate registration (same resident, same unit)
                            string dupCheck = @"SELECT COUNT(*) 
                                                FROM HomeownerUnits 
                                                WHERE ResidentID = @residentId 
                                                  AND UnitID = @unitId 
                                                  AND IsCurrent = 1";
                            using (SqlCommand dupCmd = new SqlCommand(dupCheck, conn, transaction))
                            {
                                dupCmd.Parameters.AddWithValue("@residentId", _residentId);
                                dupCmd.Parameters.AddWithValue("@unitId", unitId);
                                int exists = Convert.ToInt32(dupCmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    MessageBox.Show("This resident is already registered for this unit.",
                                        "Duplicate Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    transaction.Rollback();
                                    return;
                                }
                            }

                            // Insert into HomeownerUnits - USE RESIDENTID
                            string insertJunctionQuery;
                            if (_residencyType == "Owner")
                            {
                                insertJunctionQuery = @"INSERT INTO HomeownerUnits (ResidentID, UnitID, DateOfOwnership, ApprovedByUserID, IsCurrent)
                                                        VALUES (@residentId, @unitId, @dateOfOwnership, @ApprovedByUserID, 1)";
                            }
                            else
                            {
                                insertJunctionQuery = @"INSERT INTO HomeownerUnits (ResidentID, UnitID, IsCurrent)
                                                        VALUES (@residentId, @unitId, 1)";
                            }

                            using (SqlCommand insertJunctionCmd = new SqlCommand(insertJunctionQuery, conn, transaction))
                            {
                                insertJunctionCmd.Parameters.AddWithValue("@residentId", _residentId);
                                insertJunctionCmd.Parameters.AddWithValue("@unitId", unitId);

                                if (_residencyType == "Owner")
                                {
                                    insertJunctionCmd.Parameters.AddWithValue("@dateOfOwnership", DTPOwnership.Value);

                                    object approvedByValue = DBNull.Value;
                                    if (cmbApprovedBy != null && cmbApprovedBy.SelectedIndex > 0)
                                    {
                                        ListItem selectedItem = (ListItem)cmbApprovedBy.SelectedItem;
                                        approvedByValue = selectedItem.Value;
                                    }
                                    insertJunctionCmd.Parameters.AddWithValue("@ApprovedByUserID", approvedByValue);
                                }

                                insertJunctionCmd.ExecuteNonQuery();
                            }

                            // Update apartment rooms for Tenants
                            if (unitTypeText == "Apartment" && _residencyType == "Tenant")
                            {
                                string updateRoomsQuery = "UPDATE TBL_Units SET AvailableRooms = AvailableRooms - 1 WHERE UnitID = @unitId";
                                using (SqlCommand updateRoomsCmd = new SqlCommand(updateRoomsQuery, conn, transaction))
                                {
                                    updateRoomsCmd.Parameters.AddWithValue("@unitId", unitId);
                                    updateRoomsCmd.ExecuteNonQuery();
                                }

                                string updateStatusQuery = @"
                                    UPDATE TBL_Units
                                    SET IsOccupied = 
                                        CASE 
                                            WHEN AvailableRooms = 0 THEN 1 
                                            ELSE 0 
                                        END
                                    WHERE UnitID = @unitId";
                                using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, conn, transaction))
                                {
                                    updateStatusCmd.Parameters.AddWithValue("@unitId", unitId);
                                    updateStatusCmd.ExecuteNonQuery();
                                }
                            }
                            else if (unitTypeText != "Apartment")
                            {
                                // Mark non-apartment units as occupied
                                string updateUnitStatusQuery = "UPDATE TBL_Units SET IsOccupied = 1 WHERE UnitID = @unitId";
                                using (SqlCommand updateUnitStatusCmd = new SqlCommand(updateUnitStatusQuery, conn, transaction))
                                {
                                    updateUnitStatusCmd.Parameters.AddWithValue("@unitId", unitId);
                                    updateUnitStatusCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Unit added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception innerEx)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error saving unit: {innerEx.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving unit: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbUnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cmbUnitType.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrEmpty(selectedType))
            {
                cmbNumRooms.Enabled = false;
                cmbNumRooms.SelectedIndex = -1;
                return;
            }

            if (selectedType == "Apartment")
            {
                if (_residencyType == "Owner")
                {
                    cmbNumRooms.Enabled = true;
                }
                else
                {
                    cmbNumRooms.Enabled = false;
                    cmbNumRooms.SelectedIndex = -1;
                }
            }
            else
            {
                cmbNumRooms.Enabled = false;
                cmbNumRooms.SelectedIndex = -1;
            }
        }

        // Event handlers
        private void HomeownerID_TextChanged(object sender, EventArgs e) { }
        private void DTPOwnership_ValueChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void Approvedbytxt_TextChanged(object sender, EventArgs e) { }
    }
}