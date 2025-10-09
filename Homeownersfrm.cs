using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class Homeowners : UserControl
    {
        public monthdues MonthDuesControl { get; set; }
        public Homeowners()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            LoadHomeowners();
        }
        public void RefreshData()
        {
            LoadHomeowners();
        }
        private void Homeowners_Load(object sender, EventArgs e)
        {
            DGVResidents.CellDoubleClick += DGVResidents_CellDoubleClick;

            LoadHomeowners();
        }

        public void LoadHomeowners()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                            SELECT
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
                              
                                (SELECT TOP 1 ISNULL(hu.ApprovedByUserID, 0) 
                                 FROM HomeownerUnits hu 
                                 WHERE hu.HomeownerID = r.HomeownerID 
                                 ORDER BY hu.DateOfOwnership DESC) AS ApprovedByUserID,
                             
                                (SELECT COUNT(*) 
                                 FROM HomeownerUnits hu 
                                 WHERE hu.HomeownerID = r.HomeownerID 
                                   AND hu.IsCurrent = 1) AS UnitsAcquired
                            FROM Residents r
                            WHERE r.IsActive = 1
                            ORDER BY r.HomeownerID;";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    DGVResidents.DataSource = dt;
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
                            HomeownerID,
                            FirstName,
                            LastName,
                            HomeAddress,
                            ContactNumber,
                            EmailAddress,
                            EmergencyContactPerson,
                            EmergencyContactNumber,
                            ResidencyType
                        FROM Residents
                        WHERE IsActive = 1
                        ORDER BY LastName, FirstName";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    DGVResidents.DataSource = dt;
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
                    if (DGVResidents.Columns["HomeownerID"] != null)
                    {
                        DGVResidents.Columns["HomeownerID"].HeaderText = "Homeowner ID";
                        DGVResidents.Columns["HomeownerID"].Width = 50;
                    }

                    if (DGVResidents.Columns["FirstName"] != null)
                    {
                        DGVResidents.Columns["FirstName"].HeaderText = "First Name";
                        DGVResidents.Columns["FirstName"].Width = 100;
                    }

                    if (DGVResidents.Columns["MiddleName"] != null)
                    {
                        DGVResidents.Columns["MiddleName"].HeaderText = "Middle Name";
                        DGVResidents.Columns["MiddleName"].Width = 100;
                    }

                    if (DGVResidents.Columns["LastName"] != null)
                    {
                        DGVResidents.Columns["LastName"].HeaderText = "Last Name";
                        DGVResidents.Columns["LastName"].Width = 100;
                    }

                    if (DGVResidents.Columns["HomeAddress"] != null)
                    {
                        DGVResidents.Columns["HomeAddress"].HeaderText = "Home Address";
                        DGVResidents.Columns["HomeAddress"].Width = 200;
                    }

                    if (DGVResidents.Columns["ContactNumber"] != null)
                    {
                        DGVResidents.Columns["ContactNumber"].HeaderText = "Contact Number";
                        DGVResidents.Columns["ContactNumber"].Width = 120;
                    }

                    if (DGVResidents.Columns["EmailAddress"] != null)
                    {
                        DGVResidents.Columns["EmailAddress"].HeaderText = "Email";
                        DGVResidents.Columns["EmailAddress"].Width = 150;
                    }

                    if (DGVResidents.Columns["EmergencyContactPerson"] != null)
                    {
                        DGVResidents.Columns["EmergencyContactPerson"].HeaderText = "Emergency Contact";
                        DGVResidents.Columns["EmergencyContactPerson"].Width = 150;
                        DGVResidents.Columns["EmergencyContactPerson"].Visible = true;
                    }

                    if (DGVResidents.Columns["EmergencyContactNumber"] != null)
                    {
                        DGVResidents.Columns["EmergencyContactNumber"].HeaderText = "Emergency Number";
                        DGVResidents.Columns["EmergencyContactNumber"].Width = 120;
                        DGVResidents.Columns["EmergencyContactNumber"].Visible = true;
                    }

                    if (DGVResidents.Columns["ResidencyType"] != null)
                    {
                        DGVResidents.Columns["ResidencyType"].HeaderText = "Residency Type";
                        DGVResidents.Columns["ResidencyType"].Width = 80;
                    }

                    if (DGVResidents.Columns["ApprovedByUserID"] != null)
                    {
                        DGVResidents.Columns["ApprovedByUserID"].HeaderText = "Approved By UserID";
                        DGVResidents.Columns["ApprovedByUserID"].Width = 120;
                        DGVResidents.Columns["ApprovedByUserID"].Visible = false;
                    }

                    if (DGVResidents.Columns["UnitsAcquired"] != null)
                    {
                        DGVResidents.Columns["UnitsAcquired"].HeaderText = "Units Owned";
                        DGVResidents.Columns["UnitsAcquired"].Width = 80;
                    }



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


                    DGVResidents.Dock = DockStyle.None;
                    DGVResidents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up columns: {ex.Message}", "Column Setup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                MessageBox.Show($"Error setting up basic columns: {ex.Message}", "Setup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddResidentsbtn_Click(object sender, EventArgs e)
        {
            try
            {
                ResidencyRegisterfrm registerForm = new ResidencyRegisterfrm();
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    LoadHomeowners();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening registration form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            LoadHomeowners();
            MessageBox.Show("Data refreshed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (DGVResidents.SelectedRows.Count > 0)
            {
                try
                {
                    int homeownerId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["HomeownerID"].Value);

                    ResidencyRegisterfrm editForm = new ResidencyRegisterfrm(homeownerId);

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadHomeowners();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening edit form: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a homeowner to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (DGVResidents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a homeowner first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int homeownerId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["HomeownerID"].Value);

            using (var unitsForm = new UnitsForm(homeownerId))
            {
                if (unitsForm.ShowDialog() == DialogResult.OK)
                {
                    foreach (int unitId in unitsForm.SelectedUnitIds)
                    {
                        UnregisterUnit(homeownerId, unitId);
                    }

                    MessageBox.Show("Selected unit(s) successfully unregistered.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHomeowners(); 
                }
            }
        }
        private void UnregisterUnit(int homeownerId, int unitId)
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
                            using (SqlCommand cmd = new SqlCommand("SELECT ResidencyType FROM Residents WHERE HomeownerID = @homeownerId", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                                object o = cmd.ExecuteScalar();
                                residencyType = o == DBNull.Value || o == null ? "" : o.ToString();
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE HomeownerUnits
                          SET IsCurrent = 0, DateOfOwnershipEnd = ISNULL(DateOfOwnershipEnd, GETDATE())
                          WHERE HomeownerID = @homeownerId AND UnitID = @unitId AND IsCurrent = 1", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                cmd.ExecuteNonQuery();
                            }

                            if (string.Equals(unitType, "Apartment", StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(residencyType, "Tenant", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "UPDATE TBL_Units SET AvailableRooms = ISNULL(AvailableRooms,0) + 1 WHERE UnitID = @unitId", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@unitId", unitId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (!string.Equals(unitType, "Apartment", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "SELECT COUNT(*) FROM HomeownerUnits WHERE UnitID = @unitId AND IsCurrent = 1", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@unitId", unitId);
                                    int remaining = Convert.ToInt32(cmd.ExecuteScalar());
                                    if (remaining == 0)
                                    {
                                        using (SqlCommand upd = new SqlCommand(
                                            "UPDATE TBL_Units SET IsOccupied = 0 WHERE UnitID = @unitId", conn, tran))
                                        {
                                            upd.Parameters.AddWithValue("@unitId", unitId);
                                            upd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            using (SqlCommand checkCmd = new SqlCommand(
                                "SELECT COUNT(*) FROM HomeownerUnits WHERE HomeownerID = @homeownerId AND IsCurrent = 1", conn, tran))
                            {
                                checkCmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                                int activeLinks = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (activeLinks == 0)
                                {
                                    using (SqlCommand deactivate = new SqlCommand(
                                        "UPDATE Residents SET IsActive = 0, InactiveDate = GETDATE() WHERE HomeownerID = @homeownerId", conn, tran))
                                    {
                                        deactivate.Parameters.AddWithValue("@homeownerId", homeownerId);
                                        deactivate.ExecuteNonQuery();
                                    }
                                }
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
                    int homeownerId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["HomeownerID"].Value);
                    string residencyType = DGVResidents.SelectedRows[0].Cells["ResidencyType"].Value.ToString();

                    if (residencyType.Equals("Tenant", StringComparison.OrdinalIgnoreCase) ||
                        residencyType.Equals("Caretaker", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlConnection conn = DatabaseHelper.GetConnection())
                        {
                            conn.Open();
                            string q = "SELECT COUNT(*) FROM TBL_Units WHERE UnitType = 'Apartment' AND AvailableRooms > 0";
                            int available = Convert.ToInt32(new SqlCommand(q, conn).ExecuteScalar());
                            if (available == 0)
                            {
                                MessageBox.Show("No available apartment rooms for this residency type.",
                                    "No Rooms Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    AddUnits addUnitsForm = new AddUnits(homeownerId, residencyType);
                    if (addUnitsForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadHomeowners();
                        MessageBox.Show("Unit added successfully! Grid refreshed.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening add units form: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a homeowner first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OfficerInfo officerInfo = new OfficerInfo();
            officerInfo.Show();
        }
        private void DGVResidents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int homeownerId = Convert.ToInt32(DGVResidents.Rows[e.RowIndex].Cells["HomeownerID"].Value);
                ShowResidentUnits(homeownerId);
            }
        }
        private void ShowResidentUnits(int homeownerId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            hu.DateOfOwnership,
                            tu.UnitNumber,
                            tu.Block,
                            tu.UnitType,
                            us.Lastname +' '+ Firstname AS ApprovedBy
                        FROM HomeownerUnits hu
                        INNER JOIN TBL_Units tu ON hu.UnitID = tu.UnitID
                        LEFT JOIN Users us ON hu.ApprovedByUserID = us.UserID
                        WHERE hu.HomeownerID = @homeownerId
                        ORDER BY tu.UnitNumber;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@homeownerId", homeownerId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Form detailsForm = new Form();
                    detailsForm.Text = "Resident Units Information";
                    detailsForm.Width = 800;
                    detailsForm.Height = 400;

                    DataGridView dgv = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        DataSource = dt,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

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

                    string query = @"
                SELECT 
                    UnitID,
                    UnitNumber,
                    Block,
                    UnitType,
                    TotalRooms,
                    AvailableRooms,
                    CASE 
                        WHEN UnitType = 'Apartment' AND AvailableRooms > 0 AND AvailableRooms < TotalRooms 
                            THEN 'Partially Occupied (' + CAST(AvailableRooms AS NVARCHAR(10)) + ' Available)'
                        WHEN UnitType = 'Apartment' AND AvailableRooms = 0 
                            THEN 'Fully Occupied'
                        WHEN UnitType = 'Apartment' AND AvailableRooms = TotalRooms 
                            THEN 'Available'
                        WHEN IsOccupied = 1 THEN 'Occupied'
                        ELSE 'Available'
                    END AS Status
                FROM TBL_Units
                ORDER BY 
                    TRY_CAST(UnitNumber AS INT),  
                    UnitNumber;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Form unitsForm = new Form
                    {
                        Text = "All Units Information",
                        Width = 1000,
                        Height = 550,
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    DataGridView dgv = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        DataSource = dt,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        SelectionMode = DataGridViewSelectionMode.FullRowSelect
                    };

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.RowTemplate.Height = 35;

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

        private void searchbtn_Click(object sender, EventArgs e)
        {

        }
    }
}
