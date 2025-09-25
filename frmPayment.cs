using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class frmPayment : Form
    {
        private int currentResidentId;
        private List<Tuple<int, string, bool>> residentUnits; // UnitID, UnitNumber, IsOccupied
        private const decimal BaseDueRate = 100; // fixed due rate per house per month

        public frmPayment()
        {
            InitializeComponent();
            residentUnits = new List<Tuple<int, string, bool>>();
            this.dtpPaymentDate.ValueChanged += new EventHandler(this.dtpPaymentDate_ValueChanged);
            this.dtpEndMonth.ValueChanged += new EventHandler(this.dtpEndMonth_ValueChanged);
            this.Load += new EventHandler(this.frmPayment_Load);
        }

        private string GetUnitAddress(int unitId, string defaultAddress)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT r.HomeAddress 
                  FROM HomeownerUnits hu
                  INNER JOIN Residents r ON hu.HomeownerID = r.HomeownerID
                  WHERE hu.UnitID = @unitId", conn))
            {
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : defaultAddress;
            }
        }

        private int GetSelectedUnitID()
        {
            if (cmbUnits.SelectedIndex == -1) return -1;
            return residentUnits[cmbUnits.SelectedIndex].Item1;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtResidentID.Text, out int residentId))
            {
                MessageBox.Show("Please enter a valid Resident ID.");
                return;
            }

            currentResidentId = residentId;

            // Fetch resident info including IsActive
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT FirstName, MiddleName, LastName, HomeAddress, IsActive 
          FROM Residents 
          WHERE HomeownerID = @residentId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        MessageBox.Show("Resident not found.");
                        return;
                    }

                    bool isActive = Convert.ToBoolean(reader["IsActive"]);
                    if (!isActive)
                    {
                        MessageBox.Show("This resident is inactive.", "Inactive Resident", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close(); // Close the form if inactive
                        return;
                    }

                    lblResidentName.Text = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                    lblResidentAddress.Text = reader["HomeAddress"].ToString();
                }
            }

            // Fetch units
            cmbUnits.Items.Clear();
            residentUnits.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT u.UnitID, u.UnitNumber, u.IsOccupied
          FROM HomeownerUnits hu
          INNER JOIN TBL_Units u ON hu.UnitID = u.UnitID
          WHERE hu.HomeownerID = @residentId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int unitId = Convert.ToInt32(reader["UnitID"]);
                        string unitNumber = reader["UnitNumber"].ToString();
                        bool isOccupied = Convert.ToBoolean(reader["IsOccupied"]);

                        residentUnits.Add(new Tuple<int, string, bool>(unitId, unitNumber, isOccupied));
                        cmbUnits.Items.Add(unitNumber);
                    }
                }
            }

            if (residentUnits.Count == 1)
            {
                cmbUnits.SelectedIndex = 0;
                cmbUnits.Enabled = false;
                var selectedUnit = residentUnits[0];
                lblUnitStatus.Text = selectedUnit.Item3 ? "Active" : "Inactive";
                lblUnitStatus.ForeColor = selectedUnit.Item3 ? Color.Green : Color.Red;
                lblResidentAddress.Text = GetUnitAddress(selectedUnit.Item1, lblResidentAddress.Text);
                lblDueRate.Text = BaseDueRate.ToString();
            }
            else if (residentUnits.Count > 1)
            {
                cmbUnits.Enabled = true;
                lblDueRate.Text = ""; // blank until unit selected
            }
            else
            {
                MessageBox.Show("No units found for this resident.");
            }

            UpdateAmountPaidLabel();
        }



        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1) return;

            var selectedUnit = residentUnits[cmbUnits.SelectedIndex];
            lblUnitStatus.Text = selectedUnit.Item3 ? "Active" : "Inactive";
            lblUnitStatus.ForeColor = selectedUnit.Item3 ? Color.Green : Color.Red;
            lblResidentAddress.Text = GetUnitAddress(selectedUnit.Item1, lblResidentAddress.Text);
            lblDueRate.Text = BaseDueRate.ToString(); // fixed due rate

            UpdateAmountPaidLabel();
        }

        private void UpdateAmountPaidLabel()
        {
            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);

            if (endMonth < startMonth)
            {
                lblAmountPaid.Text = "0.00";
                return;
            }

            int monthsToPay = ((endMonth.Year - startMonth.Year) * 12) + endMonth.Month - startMonth.Month + 1;
            lblAmountPaid.Text = (monthsToPay * BaseDueRate).ToString("F2");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a unit.");
                return;
            }

            int unitId = GetSelectedUnitID();

            if (!decimal.TryParse(lblAmountPaid.Text, out decimal totalAmount) || totalAmount <= 0)
            {
                MessageBox.Show("No payment to save.");
                return;
            }

            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);

            if (endMonth < startMonth)
            {
                MessageBox.Show("End month cannot be before start month.");
                return;
            }

            int monthsToCover = ((endMonth.Year - startMonth.Year) * 12) + endMonth.Month - startMonth.Month + 1;

            List<string> duplicateMonths = new List<string>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("", conn))
            {
                conn.Open();

                for (int i = 0; i < monthsToCover; i++)
                {
                    DateTime month = startMonth.AddMonths(i);
                    string monthCovered = month.ToString("MMMM yyyy");

                    // Check if month already exists
                    using (SqlCommand checkCmd = new SqlCommand(
                        @"SELECT COUNT(*) FROM MonthlyDues 
                          WHERE HomeownerId=@residentId AND UnitID=@unitId AND MonthCovered=@monthCovered", conn))
                    {
                        checkCmd.Parameters.Clear();
                        checkCmd.Parameters.AddWithValue("@residentId", currentResidentId);
                        checkCmd.Parameters.AddWithValue("@unitId", unitId);
                        checkCmd.Parameters.AddWithValue("@monthCovered", monthCovered);

                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            duplicateMonths.Add(monthCovered);
                            continue; // skip duplicates
                        }
                    }

                    // Insert record
                    cmd.CommandText = @"INSERT INTO MonthlyDues 
                        (HomeownerId, UnitID, PaymentDate, AmountPaid, DueRate, MonthCovered)
                        VALUES (@residentId, @unitId, @paymentDate, @amountPaid, @dueRate, @monthCovered)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@residentId", currentResidentId);
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    cmd.Parameters.AddWithValue("@paymentDate", dtpPaymentDate.Value);
                    cmd.Parameters.AddWithValue("@amountPaid", BaseDueRate); // per month
                    cmd.Parameters.AddWithValue("@dueRate", BaseDueRate);
                    cmd.Parameters.AddWithValue("@monthCovered", monthCovered);

                    cmd.ExecuteNonQuery();
                }
            }

            if (duplicateMonths.Count > 0)
            {
                MessageBox.Show($"The following month(s) have already been paid for this year:\n{string.Join(", ", duplicateMonths)}", "Duplicate Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Payment saved successfully!");
                this.Close();
            }
        }

        private void UpdateMonthCoveredLabel()
        {
            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);

            if (endMonth < startMonth)
                lblMonthCovered.Text = "Invalid range";
            else if (startMonth == endMonth)
                lblMonthCovered.Text = startMonth.ToString("MMMM yyyy");
            else
                lblMonthCovered.Text = $"{startMonth:MMMM yyyy} - {endMonth:MMMM yyyy}";
        }

        private void dtpPaymentDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateMonthCoveredLabel();
            UpdateAmountPaidLabel();
        }

        private void dtpEndMonth_ValueChanged(object sender, EventArgs e)
        {
            UpdateMonthCoveredLabel();
            UpdateAmountPaidLabel();
        }

        private void frmPayment_Load(object sender, EventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtResidentID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
