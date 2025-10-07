using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class UnitsForm : Form
    {
        private int _homeownerId;
        public List<int> SelectedUnitIds { get; private set; } = new List<int>();
        public UnitsForm(int homeownerId)
        {
            InitializeComponent();
            _homeownerId = homeownerId;
        }

        private void UnitsForm_Load(object sender, EventArgs e)
        {

            DGVUnits.Columns.Clear();

            DGVUnits.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "Select" });
            DGVUnits.Columns.Add("UnitID", "UnitID");
            DGVUnits.Columns.Add("UnitNumber", "Unit Number");
            DGVUnits.Columns.Add("Block", "Block");
            DGVUnits.Columns.Add("UnitType", "Unit Type");
            DGVUnits.Columns.Add("Status", "Status");

            DGVUnits.Columns["UnitID"].Visible = false;

            LoadUnitsForHomeowner();
        }
        private void LoadUnitsForHomeowner()
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
                        WHERE hu.HomeownerID = @hid AND hu.IsCurrent = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@hid", _homeownerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DGVUnits.Rows.Clear();
                        while (reader.Read())
                        {
                            DGVUnits.Rows.Add(false, reader["UnitID"], reader["UnitNumber"], reader["Block"], reader["UnitType"], reader["Status"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading units: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please select at least one unit to unregister.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
    }
