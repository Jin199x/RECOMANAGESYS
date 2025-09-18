using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class Homeowners : UserControl
    {

        public Homeowners()
        {
            InitializeComponent();
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
                    conn.Open();
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
                            -- Grab an ApprovedByUserID if present (top 1); otherwise 0
                            (SELECT TOP 1 ISNULL(hu.ApprovedByUserID, 0) FROM HomeownerUnits hu WHERE hu.HomeownerID = r.HomeownerID) AS ApprovedByUserID,
                            -- Count units per homeowner
                            (SELECT COUNT(*) FROM HomeownerUnits hu WHERE hu.HomeownerID = r.HomeownerID) AS UnitsAcquired
                        FROM Residents r
                        WHERE r.IsActive = 1
                        ORDER BY r.HomeownerID;
                    ";

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

                    DGVResidents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    DGVResidents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    DGVResidents.ReadOnly = true;
                    DGVResidents.AllowUserToAddRows = false;
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
                MessageBox.Show("Please select a homeowner to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    "WARNING: This will permanently delete this homeowner and their unit links.\n\n" +
                    "Are you sure you want to proceed?",
                    "CONFIRM PERMANENT DELETE",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                int homeownerId = Convert.ToInt32(DGVResidents.SelectedRows[0].Cells["HomeownerID"].Value);
                string homeownerName = $"{DGVResidents.SelectedRows[0].Cells["FirstName"].Value} {DGVResidents.SelectedRows[0].Cells["LastName"].Value}";

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {

                            List<int> unitIds = new List<int>();
                            Dictionary<int, string> unitTypes = new Dictionary<int, string>();

                            string getUnitsQuery = @"
                        SELECT hu.UnitID, u.UnitType
                        FROM HomeownerUnits hu
                        INNER JOIN TBL_Units u ON hu.UnitID = u.UnitID
                        WHERE hu.HomeownerID = @id";

                            using (SqlCommand getUnitsCmd = new SqlCommand(getUnitsQuery, conn, transaction))
                            {
                                getUnitsCmd.Parameters.AddWithValue("@id", homeownerId);
                                using (SqlDataReader reader = getUnitsCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int unitId = reader.GetInt32(0);
                                        string unitType = reader.GetString(1);
                                        unitIds.Add(unitId);
                                        unitTypes[unitId] = unitType;
                                    }
                                }
                            }
                            using (SqlCommand deleteHomeownerUnitsCmd =
                                   new SqlCommand("DELETE FROM HomeownerUnits WHERE HomeownerID = @id", conn, transaction))
                            {
                                deleteHomeownerUnitsCmd.Parameters.AddWithValue("@id", homeownerId);
                                deleteHomeownerUnitsCmd.ExecuteNonQuery();
                            }
                            foreach (int unitId in unitIds)
                            {
                                if (unitTypes[unitId] != "Apartment")
                                {
                                    using (SqlCommand updateUnitsCmd =
                                           new SqlCommand("UPDATE TBL_Units SET IsOccupied = 0 WHERE UnitID = @unitId", conn, transaction))
                                    {
                                        updateUnitsCmd.Parameters.AddWithValue("@unitId", unitId);
                                        updateUnitsCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            using (SqlCommand deleteResidentCmd =
                                   new SqlCommand("DELETE FROM Residents WHERE HomeownerID = @id", conn, transaction))
                            {
                                deleteResidentCmd.Parameters.AddWithValue("@id", homeownerId);
                                deleteResidentCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            MessageBox.Show(
                                $"PERMANENTLY DELETED:\n" +
                                $"- Homeowner: {homeownerName} (ID: {homeownerId})\n" +
                                $"- {unitIds.Count} unit link(s) removed\n",
                                "Deletion Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            LoadHomeowners();
                        }
                        catch (Exception ex2)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error during deletion: {ex2.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in delete process: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    CASE WHEN IsOccupied = 1 THEN 'Occupied' ELSE 'Available' END AS Status
               FROM TBL_Units
                ORDER BY 
                    TRY_CAST(UnitNumber AS INT),  
                    UnitNumber;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    Form unitsForm = new Form();
                    unitsForm.Text = "All Units Information";
                    unitsForm.Width = 900;
                    unitsForm.Height = 500;

                    DataGridView dgv = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        DataSource = dt,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

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
    }
}
