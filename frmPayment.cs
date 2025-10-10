using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static RECOMANAGESYS.loginform;

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
            this.AutoScaleMode = AutoScaleMode.Dpi;
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
                    cmd.Parameters.AddWithValue("@ProcessedByUserID", CurrentUser.UserId);

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

                using (var receipt = new Form())
                {
                    receipt.Text = "Payment Receipt";
                    receipt.StartPosition = FormStartPosition.CenterParent; 
                    receipt.Width = 800;  
                    receipt.Height = 600;

                    var reportViewer = new Microsoft.Reporting.WinForms.ReportViewer
                    {
                        Dock = DockStyle.Fill,
                        ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
                    };

                    receipt.Controls.Add(reportViewer);

                    reportViewer.LocalReport.ReportEmbeddedResource = "RECOMANAGESYS.PaymentReceipt.rdlc";
                    decimal.TryParse(txtChange.Text, out decimal changeAmount);

                    var parameters = new Microsoft.Reporting.WinForms.ReportParameter[]
                    {
                      new Microsoft.Reporting.WinForms.ReportParameter("txtResidentID", txtResidentID.Text),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtResidentName", lblResidentName.Text),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtPayment", lblAmountPaid.Text),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtChange", changeAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtRemarks", cmbRemarks.Text),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy")),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtOfficerName", CurrentUser.FullName),
                      new Microsoft.Reporting.WinForms.ReportParameter("txtOfficerPosition", CurrentUser.Role)
                    };
                    reportViewer.LocalReport.SetParameters(parameters);
                    reportViewer.RefreshReport();

                    receipt.ShowDialog();
                }

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
            cmbRemarks.Items.Clear();
            cmbRemarks.Items.Add("N/A");
            cmbRemarks.Items.Add("Others");
            cmbRemarks.SelectedIndex = 0; 

            cmbRemarks.SelectedIndexChanged += cmbRemarks_SelectedIndexChanged;
            this.txtChange.KeyPress += new KeyPressEventHandler(this.txtChange_KeyPress);
            this.txtChange.Leave += new EventHandler(this.txtChange_Leave);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtResidentID_TextChanged(object sender, EventArgs e)
        {

        }
        private void cmbRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRemarks.SelectedItem.ToString() == "Others")
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Please specify your remark:", "Other Remark", "");

                if (!string.IsNullOrWhiteSpace(input))
                {
                    cmbRemarks.Items.Add(input);
                    cmbRemarks.SelectedItem = input;
                }
                else
                {
                    cmbRemarks.SelectedIndex = 0; 
                }
            }
        }
        private void txtChange_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; 
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true; 
            }
        }
        private void txtChange_Leave(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "0.00";
            }
            else
            {
                if (decimal.TryParse(tb.Text, out decimal value))
                {
                    tb.Text = value.ToString("F2");
                }
            }
        }

    }
}
