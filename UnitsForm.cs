using System;
using System.Collections.Generic;
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
            DGVUnits.Size = new Size(850, 400);
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
            DGVUnits.ScrollBars = ScrollBars.Both;
            DGVUnits.RowHeadersVisible = false;
            DGVUnits.GridColor = Color.LightGray;
        }
        private void UnitsForm_Load(object sender, EventArgs e)
        {
            InitializeDataGridView();
            LoadUnitsForResident();
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

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lblInstruction_Click(object sender, EventArgs e)
        {

        }
    }
}