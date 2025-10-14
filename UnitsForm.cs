using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class UnitsForm : Form
    {
        private int _residentId;
        public List<int> SelectedUnitIds { get; private set; } = new List<int>();

        public UnitsForm(int residentId)
        {
            InitializeComponent();
            _residentId = residentId;
        }

        private void UnitsForm_Load(object sender, EventArgs e)
        {
            InitializeDataGridView();
            LoadUnitsForResident();
            DGVUnits.SelectionChanged += DGVUnits_SelectionChanged;
            DGVTCunregister.CellContentClick += DGVTCunregister_CellContentClick;
        }

        private void InitializeDataGridView()
        {
            DGVUnits.Columns.Clear();
            DGVUnits.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "Select",
                Width = 70
            });
            DGVUnits.Columns.Add("UnitID", "UnitID");
            DGVUnits.Columns.Add("UnitNumber", "Unit Number");
            DGVUnits.Columns.Add("Block", "Block");
            DGVUnits.Columns.Add("UnitType", "Unit Type");
            DGVUnits.Columns.Add("Status", "Status");

            DGVUnits.Columns["UnitID"].Visible = false;
            DGVUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVUnits.ColumnHeadersHeight = 45;
            DGVUnits.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            DGVUnits.EnableHeadersVisualStyles = false;
            DGVUnits.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Arial", 11F),
                SelectionBackColor = Color.CornflowerBlue,
                SelectionForeColor = Color.White
            };
            DGVUnits.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            DGVUnits.RowHeadersVisible = false;
            DGVUnits.GridColor = Color.LightGray;

            DGVTCunregister.Columns.Clear();
            DGVTCunregister.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "Select",
                Width = 70
            });
            DGVTCunregister.Columns.Add("ResidentID", "ResidentID");
            DGVTCunregister.Columns.Add("FullName", "Full Name");
            DGVTCunregister.Columns.Add("ResidencyType", "Residency Type");
            DGVTCunregister.Columns.Add("Status", "Status");

            DGVTCunregister.Columns["ResidentID"].Visible = false;
            DGVTCunregister.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVTCunregister.ColumnHeadersHeight = 45;
            DGVTCunregister.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            DGVTCunregister.EnableHeadersVisualStyles = false;
            DGVTCunregister.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Arial", 11F),
                SelectionBackColor = Color.CornflowerBlue,
                SelectionForeColor = Color.White
            };
            DGVTCunregister.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            DGVTCunregister.RowHeadersVisible = false;
            DGVTCunregister.GridColor = Color.LightGray;

            DGVTCunregister.EditMode = DataGridViewEditMode.EditOnEnter;
            DGVTCunregister.ReadOnly = false;
            foreach (DataGridViewColumn col in DGVTCunregister.Columns)
            {
                if (col.Name != "Select")
                    col.ReadOnly = true;
            }

            DGVTCunregister.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == DGVTCunregister.Columns["Select"].Index)
                {
                    DGVTCunregister.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        private void LoadUnitsForResident()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT u.UnitID, u.UnitNumber, u.Block, u.UnitType,
                               CASE WHEN u.IsOccupied = 1 THEN 'Occupied' ELSE 'Available' END AS Status
                        FROM TBL_Units u
                        INNER JOIN HomeownerUnits hu ON hu.UnitID = u.UnitID
                        WHERE hu.ResidentID = @residentId AND hu.IsCurrent = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@residentId", _residentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DGVUnits.Rows.Clear();
                        while (reader.Read())
                        {
                            DGVUnits.Rows.Add(false, reader["UnitID"], reader["UnitNumber"],
                                reader["Block"], reader["UnitType"], reader["Status"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading units: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (DGVUnits.SelectedRows.Count > 0)
            {
                int selectedUnitId = Convert.ToInt32(DGVUnits.SelectedRows[0].Cells["UnitID"].Value);
                LoadTenantsAndCaretaker(selectedUnitId);
            }
        }

        private void DGVUnits_SelectionChanged(object sender, EventArgs e)
        {
            if (DGVUnits.SelectedRows.Count == 0) return;
            int unitId = Convert.ToInt32(DGVUnits.SelectedRows[0].Cells["UnitID"].Value);
            LoadTenantsAndCaretaker(unitId);
        }

        private void LoadTenantsAndCaretaker(int unitId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            r.ResidentID,
                            CONCAT(r.FirstName, ' ', r.LastName) AS FullName,
                            r.ResidencyType,
                            CASE WHEN hu.IsCurrent = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                        FROM HomeownerUnits hu
                        INNER JOIN Residents r ON hu.ResidentID = r.ResidentID
                        WHERE hu.UnitID = @unitId
                          AND r.ResidencyType IN ('Tenant', 'Caretaker')
                        ORDER BY 
                            CASE WHEN hu.IsCurrent = 1 THEN 0 ELSE 1 END, 
                            r.ResidencyType,
                            r.LastName,
                            r.FirstName;";


                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@unitId", unitId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DGVTCunregister.Rows.Clear();
                        while (reader.Read())
                        {
                            DGVTCunregister.Rows.Add(false, reader["ResidentID"], reader["FullName"],
                                reader["ResidencyType"], reader["Status"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tenants/caretakers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DGVTCunregister_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (DGVTCunregister.Columns[e.ColumnIndex].Name == "Select")
            {
                DGVTCunregister.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }


      


        private void UnregisterResident(int residentId, int unitId)
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
                            string deactivateLink = @"
                                UPDATE HomeownerUnits 
                                SET IsCurrent = 0, DateOfOwnershipEnd = GETDATE()
                                WHERE ResidentID = @residentId AND UnitID = @unitId AND IsCurrent = 1";
                            using (SqlCommand cmd = new SqlCommand(deactivateLink, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@residentId", residentId);
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                cmd.ExecuteNonQuery();
                            }

                            string unitType = "";
                            string residencyType = "";

                            using (SqlCommand cmd = new SqlCommand("SELECT UnitType FROM TBL_Units WHERE UnitID=@id", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", unitId);
                                unitType = cmd.ExecuteScalar()?.ToString() ?? "";
                            }

                            using (SqlCommand cmd = new SqlCommand("SELECT ResidencyType FROM Residents WHERE ResidentID=@id", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", residentId);
                                residencyType = cmd.ExecuteScalar()?.ToString() ?? "";
                            }

                            if (unitType.Equals("Apartment", StringComparison.OrdinalIgnoreCase) &&
                                residencyType.Equals("Tenant", StringComparison.OrdinalIgnoreCase))
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "UPDATE TBL_Units SET AvailableRooms = AvailableRooms + 1 WHERE UnitID=@id", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@id", unitId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            string check = "SELECT COUNT(*) FROM HomeownerUnits WHERE ResidentID=@id AND IsCurrent=1";
                            using (SqlCommand cmd = new SqlCommand(check, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", residentId);
                                if ((int)cmd.ExecuteScalar() == 0)
                                {
                                    using (SqlCommand deactivate = new SqlCommand(
                                        "UPDATE Residents SET IsActive=0, InactiveDate=GETDATE() WHERE ResidentID=@id", conn, tran))
                                    {
                                        deactivate.Parameters.AddWithValue("@id", residentId);
                                        deactivate.ExecuteNonQuery();
                                    }
                                }
                            }

                            tran.Commit();
                            MessageBox.Show("Resident successfully unregistered.", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show($"Error unregistering resident: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUnregisterSelected_Click(object sender, EventArgs e)
        {
            SelectedUnitIds.Clear();

            foreach (DataGridViewRow row in DGVUnits.Rows)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["Select"].Value ?? false);
                if (isSelected)
                {
                    int unitId = Convert.ToInt32(row.Cells["UnitID"].Value);
                    SelectedUnitIds.Add(unitId);
                }
            }

            if (SelectedUnitIds.Count == 0)
            {
                MessageBox.Show("Please select at least one unit to unregister.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                foreach (int unitId in SelectedUnitIds)
                {
                    string checkActiveQuery = @"
                SELECT COUNT(*) 
                FROM HomeownerUnits hu
                INNER JOIN Residents r ON hu.ResidentID = r.ResidentID
                WHERE hu.UnitID = @unitId 
                  AND hu.IsCurrent = 1 
                  AND r.ResidencyType IN ('Tenant', 'Caretaker');";

                    using (SqlCommand checkCmd = new SqlCommand(checkActiveQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@unitId", unitId);
                        int activeCount = (int)checkCmd.ExecuteScalar();

                        if (activeCount > 0)
                        {
                            MessageBox.Show(
                                $"Unit ID {unitId} cannot be unregistered because there are still active tenants or caretakers.",
                                "Cannot Unregister Unit",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                    }
                }

                var result = MessageBox.Show(
                    "Are you sure you want to unregister the selected units from this owner?",
                    "Confirm Unregister",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (int unitId in SelectedUnitIds)
                        {
                            string deactivateLink = @"
                        UPDATE HomeownerUnits 
                        SET IsCurrent = 0, DateOfOwnershipEnd = GETDATE()
                        WHERE ResidentID = @residentId AND UnitID = @unitId AND IsCurrent = 1";
                            using (SqlCommand cmd = new SqlCommand(deactivateLink, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@residentId", _residentId);
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                cmd.ExecuteNonQuery();
                            }

                            string checkOccupants = @"
                        SELECT COUNT(*) FROM HomeownerUnits 
                        WHERE UnitID = @unitId AND IsCurrent = 1";
                            using (SqlCommand cmd = new SqlCommand(checkOccupants, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@unitId", unitId);
                                int count = (int)cmd.ExecuteScalar();
                                if (count == 0)
                                {
                                    using (SqlCommand cmd2 = new SqlCommand(
                                        "UPDATE TBL_Units SET IsOccupied = 0 WHERE UnitID = @unitId", conn, tran))
                                    {
                                        cmd2.Parameters.AddWithValue("@unitId", unitId);
                                        cmd2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        tran.Commit();
                        MessageBox.Show("Selected units successfully unregistered.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadUnitsForResident();
                        DGVTCunregister.Rows.Clear();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show($"Error unregistering units: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnUnregisterTC_Click(object sender, EventArgs e)
        {
            if (DGVTCunregister.Rows.Count == 0 || DGVUnits.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a unit and at least one tenant/caretaker to unregister.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int unitId = Convert.ToInt32(DGVUnits.SelectedRows[0].Cells["UnitID"].Value);
            List<int> selectedResidentIds = new List<int>();
            foreach (DataGridViewRow row in DGVTCunregister.Rows)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["Select"].Value ?? false);
                if (isSelected)
                {
                    selectedResidentIds.Add(Convert.ToInt32(row.Cells["ResidentID"].Value));
                }
            }

            if (selectedResidentIds.Count == 0)
            {
                MessageBox.Show("Please check at least one tenant or caretaker to unregister.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to unregister the selected tenants/caretakers?",
                "Confirm Unregister", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (int residentId in selectedResidentIds)
                {
                    UnregisterResident(residentId, unitId);
                }

                LoadTenantsAndCaretaker(unitId);
                LoadUnitsForResident();
            }
        }
    }
}
