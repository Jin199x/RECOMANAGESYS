using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class Homeowners : UserControl
    {
        public monthdues MonthDuesControl { get; set; }

        private DataTable originalHomeownersTable;

        public Homeowners()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            LoadHomeowners();

            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (originalHomeownersTable == null) return;

            DataView dv = originalHomeownersTable.DefaultView;
            string searchText = txtSearch.Text.Trim();

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
                        "CONVERT(HomeownerID, 'System.String') LIKE '%{0}%' OR " +
                        "FirstName LIKE '%{0}%' OR " +
                        "LastName LIKE '%{0}%' OR " +
                        "HomeAddress LIKE '%{0}%' OR " +
                        "ContactNumber LIKE '%{0}%' OR " +
                        "EmailAddress LIKE '%{0}%' OR " +
                        "ResidencyType LIKE '%{0}%' OR " +
                        "Status LIKE '%{0}%' OR " +
                        "ResidentsList LIKE '%{0}%'",
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
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        public void RefreshData()
        {
            LoadHomeowners();
        }
        private void Homeowners_Load(object sender, EventArgs e)
        {
            DGVResidents.CellDoubleClick += DGVResidents_CellDoubleClick;
        }

        public void LoadHomeowners()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT
                            r.ResidentID,
                            r.HomeownerID,
                            ISNULL(r.FirstName, '') AS FirstName,
                            ISNULL(r.MiddleName, '') AS MiddleName,
                            ISNULL(r.LastName, '') AS LastName,
                            ISNULL(r.HomeAddress, '') AS HomeAddress,
                            ISNULL(r.ContactNumber, '') AS ContactNumber,
                            ISNULL(r.EmailAddress, '') AS EmailAddress,
                            ISNULL(r.EmergencyContactPerson, '') AS EmergencyContactPerson,
                            ISNULL(r.EmergencyContactNumber, '') AS EmergencyContactNumber,
                            ISNULL(r.ResidencyType, '') AS ResidencyType,
                            CASE 
                                WHEN r.IsActive = 1 THEN 'Active'
                                ELSE 'Inactive'
                            END AS Status,
                            r.InactiveDate,
                            (SELECT 
                                STRING_AGG(CONCAT(FirstName, ' ', LastName, ' (', ResidencyType, ')'), ', ')
                             FROM Residents r2
                             WHERE r2.HomeownerID = r.HomeownerID 
                               AND r2.ResidencyType IN ('Tenant', 'Caretaker')
                               AND r2.IsActive = 1) AS ResidentsList
                        FROM Residents r
                        WHERE r.ResidencyType = 'Owner'
                        ORDER BY 
                            CASE WHEN r.IsActive = 1 THEN 0 ELSE 1 END,
                            r.HomeownerID, 
                            CASE WHEN r.ResidencyType = 'Owner' THEN 1 
                                 WHEN r.ResidencyType = 'Tenant' THEN 2 
                                 ELSE 3 END,
                            r.ResidentID;";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // --- MODIFICATION: Store data and bind to the DefaultView ---
                    originalHomeownersTable = dt;
                    DGVResidents.DataSource = originalHomeownersTable.DefaultView;

                    SetupColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading homeowners: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadHomeownersSimple();
            }
        }

        private void LoadHomeownersSimple()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                            SELECT
                                ResidentID, HomeownerID, FirstName, LastName, HomeAddress,
                                ContactNumber, EmailAddress, EmergencyContactPerson,
                                EmergencyContactNumber, ResidencyType
                            FROM Residents
                            WHERE IsActive = 1
                            ORDER BY HomeownerID, ResidencyType DESC, LastName, FirstName";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // --- MODIFICATION: Apply same logic to the fallback method ---
                    originalHomeownersTable = dt;
                    DGVResidents.DataSource = originalHomeownersTable.DefaultView;
                    SetupBasicColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error: {ex.Message}\n\nPlease check your database connection and table structure.",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupColumns()
        {
            try
            {
                if (DGVResidents.Columns.Count > 0)
                {
                    if (DGVResidents.Columns["ResidentID"] != null) DGVResidents.Columns["ResidentID"].Visible = false;
                    if (DGVResidents.Columns["HomeownerID"] != null) { DGVResidents.Columns["HomeownerID"].HeaderText = "Homeowner ID"; DGVResidents.Columns["HomeownerID"].Width = 100; }
                    if (DGVResidents.Columns["FirstName"] != null) { DGVResidents.Columns["FirstName"].HeaderText = "First Name"; DGVResidents.Columns["FirstName"].Width = 100; }
                    if (DGVResidents.Columns["MiddleName"] != null) { DGVResidents.Columns["MiddleName"].HeaderText = "Middle Name"; DGVResidents.Columns["MiddleName"].Width = 100; }
                    if (DGVResidents.Columns["LastName"] != null) { DGVResidents.Columns["LastName"].HeaderText = "Last Name"; DGVResidents.Columns["LastName"].Width = 100; }
                    if (DGVResidents.Columns["HomeAddress"] != null) { DGVResidents.Columns["HomeAddress"].HeaderText = "Home Address"; DGVResidents.Columns["HomeAddress"].Width = 200; }
                    if (DGVResidents.Columns["ContactNumber"] != null) { DGVResidents.Columns["ContactNumber"].HeaderText = "Contact Number"; DGVResidents.Columns["ContactNumber"].Width = 120; }
                    if (DGVResidents.Columns["EmailAddress"] != null) { DGVResidents.Columns["EmailAddress"].HeaderText = "Email"; DGVResidents.Columns["EmailAddress"].Width = 150; }
                    if (DGVResidents.Columns["EmergencyContactPerson"] != null) { DGVResidents.Columns["EmergencyContactPerson"].HeaderText = "Emergency Contact"; DGVResidents.Columns["EmergencyContactPerson"].Width = 150; DGVResidents.Columns["EmergencyContactPerson"].Visible = true; }
                    if (DGVResidents.Columns["EmergencyContactNumber"] != null) { DGVResidents.Columns["EmergencyContactNumber"].HeaderText = "Emergency Number"; DGVResidents.Columns["EmergencyContactNumber"].Width = 120; DGVResidents.Columns["EmergencyContactNumber"].Visible = true; }
                    if (DGVResidents.Columns["ResidencyType"] != null) { DGVResidents.Columns["ResidencyType"].HeaderText = "Residency Type"; DGVResidents.Columns["ResidencyType"].Width = 100; }
                    if (DGVResidents.Columns["ApprovedByUserID"] != null) DGVResidents.Columns["ApprovedByUserID"].Visible = false;
                    if (DGVResidents.Columns["UnitsAcquired"] != null) { DGVResidents.Columns["UnitsAcquired"].HeaderText = "Units Owned"; DGVResidents.Columns["UnitsAcquired"].Width = 80; }
                    if (DGVResidents.Columns["ResidentsList"] != null) { DGVResidents.Columns["ResidentsList"].HeaderText = "Residents/Tenants"; DGVResidents.Columns["ResidentsList"].Width = 250; }
                    if (DGVResidents.Columns["InactiveDate"] != null) DGVResidents.Columns["InactiveDate"].Visible = false;

                    DGVResidents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    DGVResidents.ScrollBars = ScrollBars.Both;
                    DGVResidents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    DGVResidents.ReadOnly = true;
                    DGVResidents.AllowUserToAddRows = false;
                    DGVResidents.MultiSelect = false;
                    DGVResidents.EnableHeadersVisualStyles = false;
                    DGVResidents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    DGVResidents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    DGVResidents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                    DGVResidents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    DGVResidents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 149, 237);
                    DGVResidents.DefaultCellStyle.SelectionForeColor = Color.White;
                    DGVResidents.ColumnHeadersHeight = 35;
                    DGVResidents.RowTemplate.Height = 40;
                    DGVResidents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    DGVResidents.CellFormatting += DGVResidents_CellFormatting;
                    DGVResidents.Dock = DockStyle.None;
                    DGVResidents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up columns: {ex.Message}", "Column Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DGVResidents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = DGVResidents.Rows[e.RowIndex];
                if (row.Cells["Status"] != null && row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status == "Inactive")
                    {
                        e.CellStyle.ForeColor = Color.Gray;
                        e.CellStyle.BackColor = Color.FromArgb(245, 245, 245);
                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Italic);
                    }
                    else
                    {
                        if (e.RowIndex % 2 == 1) e.CellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    }
                }
            }
            catch { }
        }
        private void SetupBasicColumns()
        {
            try
            {
                if (DGVResidents.Columns.Count > 0)
                {
                    DGVResidents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    DGVResidents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    DGVResidents.ReadOnly = true;
                    DGVResidents.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up basic columns: {ex.Message}", "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddResidentsbtn_Click(object sender, EventArgs e)
        {
            try
            {
                ResidencyRegisterfrm registerForm = new ResidencyRegisterfrm();
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    txtSearch.Clear();
                    LoadHomeowners();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening registration form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadHomeowners();
            MessageBox.Show("Data refreshed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (DGVResidents.SelectedRows.Count > 0)
            {
                try
                {
                    string status = DGVResidents.SelectedRows[0].Cells["Status"]?.Value?.ToString() ?? "Active";

                    if (status == "Inactive")
                    {
                        var result = MessageBox.Show(
                            "This resident is currently inactive (no active units).\n\n" +
                            "Do you want to edit their information anyway?",
                            "Edit Inactive Resident",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result != DialogResult.Yes) return;
                    }

                    int residentId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["ResidentID"].Value);
                    ResidencyRegisterfrm editForm = new ResidencyRegisterfrm(residentId);

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        txtSearch.Clear();
                        LoadHomeowners();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening edit form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a resident to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (DGVResidents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a resident first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int residentId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["ResidentID"].Value);

            using (var unitsForm = new UnitsForm(residentId))
            {
                if (unitsForm.ShowDialog() == DialogResult.OK)
                {
                    foreach (int unitId in unitsForm.SelectedUnitIds)
                    {
                        UnregisterUnit(residentId, unitId);
                    }
                    MessageBox.Show("Selected unit(s) successfully unregistered.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSearch.Clear();
                    LoadHomeowners();
                }
            }
        }

        private void UnregisterUnit(int residentId, int unitId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            string unitType;
                            using (SqlCommand cmd = new SqlCommand("SELECT UnitType FROM TBL_Units WHERE UnitID = @unitId", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                object o = cmd.ExecuteScalar();
                                unitType = o == DBNull.Value || o == null ? "" : o.ToString();
                            }

                            string residencyType;
                            using (SqlCommand cmd = new SqlCommand("SELECT ResidencyType FROM Residents WHERE ResidentID = @residentId", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@residentId", residentId);
                                object o = cmd.ExecuteScalar();
                                residencyType = o == DBNull.Value || o == null ? "" : o.ToString();
                            }

                            using (SqlCommand cmd = new SqlCommand(@"UPDATE HomeownerUnits SET IsCurrent = 0, DateOfOwnershipEnd = ISNULL(DateOfOwnershipEnd, GETDATE()) WHERE ResidentID = @residentId AND UnitID = @unitId AND IsCurrent = 1", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@residentId", residentId);
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                cmd.ExecuteNonQuery();
                            }

                            if (unitType.Equals("Apartment", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE TBL_Units SET AvailableRooms = TotalRooms WHERE UnitID = @unitId", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@unitId", unitId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (string.Equals(unitType, "Apartment", StringComparison.OrdinalIgnoreCase) && string.Equals(residencyType, "Tenant", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE TBL_Units SET AvailableRooms = ISNULL(AvailableRooms,0) + 1 WHERE UnitID = @unitId", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@unitId", unitId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (!string.Equals(unitType, "Apartment", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM HomeownerUnits WHERE UnitID = @unitId AND IsCurrent = 1", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@unitId", unitId);
                                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                                    {
                                        using (SqlCommand upd = new SqlCommand("UPDATE TBL_Units SET IsOccupied = 0 WHERE UnitID = @unitId", conn, tran))
                                        {
                                            upd.Parameters.AddWithValue("@unitId", unitId);
                                            upd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM HomeownerUnits WHERE ResidentID = @residentId AND IsCurrent = 1", conn, tran))
                            {
                                checkCmd.Parameters.AddWithValue("@residentId", residentId);
                                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                                {
                                    using (SqlCommand deactivate = new SqlCommand("UPDATE Residents SET IsActive = 0, InactiveDate = GETDATE() WHERE ResidentID = @residentId", conn, tran))
                                    {
                                        deactivate.Parameters.AddWithValue("@residentId", residentId);
                                        deactivate.ExecuteNonQuery();
                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(@"UPDATE HomeownerUnits SET IsCurrent = 0, DateOfOwnershipEnd = ISNULL(DateOfOwnershipEnd, GETDATE()) WHERE UnitID = @unitId AND ResidentID IN (SELECT ResidentID FROM Residents WHERE ResidencyType IN ('Tenant', 'Caretaker')) AND IsCurrent = 1", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unregistering unit: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddUnitbtn_Click(object sender, EventArgs e)
        {
            if (DGVResidents.SelectedRows.Count > 0)
            {
                try
                {
                    string status = DGVResidents.SelectedRows[0].Cells["Status"]?.Value?.ToString() ?? "Active";
                    if (status == "Inactive")
                    {
                        MessageBox.Show("This resident is currently inactive.\n\nAdding a unit will automatically reactivate them.", "confirm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    int residentId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["ResidentID"].Value);
                    int homeownerId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["HomeownerID"].Value);
                    string residencyType = DGVResidents.SelectedRows[0].Cells["ResidencyType"].Value.ToString();

                    if (residencyType.Equals("Tenant", StringComparison.OrdinalIgnoreCase) || residencyType.Equals("Caretaker", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlConnection conn = DatabaseHelper.GetConnection())
                        {
                            conn.Open();
                            string q = "SELECT COUNT(*) FROM TBL_Units WHERE UnitType = 'Apartment' AND AvailableRooms > 0";
                            if (Convert.ToInt32(new SqlCommand(q, conn).ExecuteScalar()) == 0)
                            {
                                MessageBox.Show("No available apartment rooms for this residency type.", "No Rooms Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    AddUnits addUnitsForm = new AddUnits(residentId, homeownerId, residencyType);
                    if (addUnitsForm.ShowDialog() == DialogResult.OK)
                    {
                        if (status == "Inactive")
                        {
                            using (SqlConnection conn = DatabaseHelper.GetConnection())
                            {
                                conn.Open();
                                string reactivate = "UPDATE Residents SET IsActive = 1, InactiveDate = NULL WHERE ResidentID = @residentId";
                                using (SqlCommand cmd = new SqlCommand(reactivate, conn))
                                {
                                    cmd.Parameters.AddWithValue("@residentId", residentId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        txtSearch.Clear();
                        LoadHomeowners();
                        MessageBox.Show("Unit added successfully. Grid refreshed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening add units form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a resident first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DGVResidents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int residentId = Convert.ToInt32(DGVResidents.Rows[e.RowIndex].Cells["ResidentID"].Value);
                ShowResidentUnits(residentId);
            }
        }

        private void ShowResidentUnits(int residentId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string residentName = "";
                    int homeownerId = 0;

                    using (SqlCommand cmdInfo = new SqlCommand("SELECT HomeownerID, CONCAT(FirstName, ' ', LastName) AS FullName FROM Residents WHERE ResidentID = @id", conn))
                    {
                        cmdInfo.Parameters.AddWithValue("@id", residentId);
                        using (SqlDataReader reader = cmdInfo.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                homeownerId = Convert.ToInt32(reader["HomeownerID"]);
                                residentName = reader["FullName"].ToString();
                            }
                        }
                    }

                    string query = @"SELECT tu.UnitNumber, tu.Block, tu.UnitType, MIN(CASE WHEN r.ResidencyType = 'Owner' THEN hu.DateOfOwnership END) AS [Start Date], MAX(CASE WHEN r.ResidencyType = 'Owner' THEN hu.DateOfOwnershipEnd END) AS [End Date], MAX(CASE WHEN r.ResidencyType = 'Owner' THEN CONCAT(r.FirstName, ' ', r.LastName) END) AS [Owner Name], STRING_AGG(CASE WHEN r.ResidencyType IN ('Tenant', 'Caretaker') THEN CONCAT(r.FirstName, ' ', r.LastName, ' (', r.ResidencyType, ')') ELSE NULL END, CHAR(13) + CHAR(10)) AS [Residents (Tenants/Caretakers)], MAX(CASE WHEN r.ResidencyType = 'Owner' THEN ISNULL(u.Lastname, '') + ' ' + ISNULL(u.Firstname, '') END) AS [Approved By] FROM TBL_Units tu INNER JOIN HomeownerUnits hu ON tu.UnitID = hu.UnitID INNER JOIN Residents r ON hu.ResidentID = r.ResidentID LEFT JOIN Users u ON hu.ApprovedByUserID = u.UserID WHERE r.HomeownerID = @homeownerId GROUP BY tu.UnitID, tu.UnitNumber, tu.Block, tu.UnitType ORDER BY TRY_CAST(tu.UnitNumber AS INT), tu.UnitNumber;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@homeownerId", homeownerId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Form detailsForm = new Form { Text = $"Resident Units Information - {residentName}", Width = 1100, Height = 500, StartPosition = FormStartPosition.CenterParent };
                    DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, DataSource = dt, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing, ColumnHeadersHeight = 50, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, AllowUserToResizeRows = true, AllowUserToResizeColumns = true, ScrollBars = ScrollBars.Both, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Arial", 11F, FontStyle.Regular), WrapMode = DataGridViewTriState.True, Alignment = DataGridViewContentAlignment.TopLeft }, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, RowTemplate = { Height = 80 } };

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold);
                    dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 149, 237);
                    dgv.DefaultCellStyle.SelectionForeColor = Color.White;

                    detailsForm.Controls.Add(dgv);
                    detailsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching units: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowAllUnits()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT tu.UnitNumber, tu.Block, tu.UnitType, tu.TotalRooms, tu.AvailableRooms, (SELECT STRING_AGG(CONCAT(r.FirstName, ' ', r.LastName, ' [Start: ', FORMAT(hu.DateOfOwnership, 'yyyy-MM-dd'), ISNULL(CONCAT(', End: ', FORMAT(hu.DateOfOwnershipEnd, 'yyyy-MM-dd')), ''), CASE WHEN hu.IsCurrent = 1 THEN ' - CURRENT' ELSE '' END, ']'), CHAR(13) + CHAR(10)) FROM HomeownerUnits hu INNER JOIN Residents r ON hu.ResidentID = r.ResidentID WHERE hu.UnitID = tu.UnitID AND r.ResidencyType = 'Owner') AS [Owner(s) History], (SELECT STRING_AGG(CONCAT(r2.FirstName, ' ', r2.LastName, ' (', r2.ResidencyType, ')'), CHAR(13) + CHAR(10)) FROM HomeownerUnits hu2 INNER JOIN Residents r2 ON hu2.ResidentID = r2.ResidentID WHERE hu2.UnitID = tu.UnitID AND r2.ResidencyType IN ('Tenant', 'Caretaker') AND r2.IsActive = 1) AS [Residents (Tenants/Caretakers)], CASE WHEN tu.UnitType = 'Apartment' AND tu.AvailableRooms > 0 AND tu.AvailableRooms < tu.TotalRooms THEN 'Partially Occupied (' + CAST(tu.AvailableRooms AS NVARCHAR(10)) + ' Available)' WHEN tu.UnitType = 'Apartment' AND tu.AvailableRooms = 0 THEN 'Fully Occupied' WHEN tu.UnitType = 'Apartment' AND tu.AvailableRooms = tu.TotalRooms THEN 'Available' WHEN tu.IsOccupied = 1 THEN 'Occupied' ELSE 'Available' END AS [Status] FROM TBL_Units tu ORDER BY TRY_CAST(tu.UnitNumber AS INT), tu.UnitNumber;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Form unitsForm = new Form { Text = "All Units Information (with Ownership History)", Width = 1300, Height = 700, StartPosition = FormStartPosition.CenterScreen };
                    DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, DataSource = dt, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing, ColumnHeadersHeight = 50, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, AllowUserToResizeRows = true, AllowUserToResizeColumns = true, ScrollBars = ScrollBars.Both, DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Arial", 11F, FontStyle.Regular), WrapMode = DataGridViewTriState.True, Alignment = DataGridViewContentAlignment.TopLeft }, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, RowTemplate = { Height = 80 } };

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold);
                    dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 149, 237);
                    dgv.DefaultCellStyle.SelectionForeColor = Color.White;

                    unitsForm.Controls.Add(dgv);
                    unitsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching units: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewUnitbtn_Click(object sender, EventArgs e)
        {
            ShowAllUnits();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OfficerInfo officerInfo = new OfficerInfo();
            officerInfo.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void button3_Click(object sender, EventArgs e) { }
        private void searchbtn_Click(object sender, EventArgs e) { }
    }
}