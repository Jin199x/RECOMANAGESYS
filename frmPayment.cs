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

            if (!decimal.TryParse(lblAmountPaid.Text, out decimal totalAmount) || totalAmount <= 0)
            {
                MessageBox.Show("No payment to save.");
                return;
            }
            decimal.TryParse(cmbPaid.Text, out decimal amountPaid);
            if (amountPaid < totalAmount)
            {
                MessageBox.Show(
                    $"The payment amount ({amountPaid:N2}) is less than the total amount due ({totalAmount:N2}).\n\nPlease correct the payment amount or the months covered.",
                    "Underpayment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return; 
            }

            decimal actualChangeGiven = 0;
            if (cmbChange.SelectedItem?.ToString() != "(None)")
            {
                decimal.TryParse(cmbChange.Text, out actualChangeGiven);
            }
            decimal expectedChange = amountPaid - totalAmount;
            if (expectedChange < 0)
            {
                expectedChange = 0;
            }
            if (Math.Abs(expectedChange - actualChangeGiven) > 0.01m)
            {
                string message = $"The change amount appears to be incorrect.\n\n" +
                                 $"Amount Paid: {amountPaid:N2}\n" +
                                 $"Total Due: {totalAmount:N2}\n" +
                                 $"----------------------------------\n" +
                                 $"Expected Change: {expectedChange:N2}\n" +
                                 $"Entered Change: {actualChangeGiven:N2}\n\n" +
                                 "A remark is required to proceed.";

                MessageBox.Show(message, "Change Discrepancy Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                string reason = "";
                do
                {
                    reason = Microsoft.VisualBasic.Interaction.InputBox(
                        "Please specify the reason for the change discrepancy:",
                        "Justification Required",
                        "");

                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        var result = MessageBox.Show(
                            "You must provide a reason to continue with this transaction. Do you want to try again?",
                            "Reason Required",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                } while (string.IsNullOrWhiteSpace(reason));

                // Calculate the exact amount of the discrepancy.
                decimal discrepancyAmount = Math.Abs(expectedChange - actualChangeGiven);
                string discrepancyPrefix;
                if (actualChangeGiven < expectedChange)
                {
                    discrepancyPrefix = $"Change Discrepancy (Less by {discrepancyAmount:N2})";
                }
                else
                {
                    discrepancyPrefix = $"Change Discrepancy (More by {discrepancyAmount:N2})";
                }
                string finalRemark = $"{discrepancyPrefix}: {reason}";

                if (!cmbRemarks.Items.Contains(finalRemark))
                {
                    cmbRemarks.Items.Add(finalRemark);
                }
                cmbRemarks.SelectedItem = finalRemark;
            }

            int unitId = GetSelectedUnitID();
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
                            continue;
                        }
                    }
                    cmd.CommandText = @"INSERT INTO MonthlyDues 
                (HomeownerId, UnitID, PaymentDate, AmountPaid, DueRate, MonthCovered, ProcessedByUserID)
                VALUES (@residentId, @unitId, @paymentDate, @amountPaid, @dueRate, @monthCovered, @ProcessedByUserID)";

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
                    string changeAmountForReport = cmbChange.Text;
                    if (cmbChange.SelectedItem?.ToString() == "(None)")
                    {
                        changeAmountForReport = "";
                    }

                    var parameters = new Microsoft.Reporting.WinForms.ReportParameter[]
                    {
                new Microsoft.Reporting.WinForms.ReportParameter("txtResidentID", txtResidentID.Text),
                new Microsoft.Reporting.WinForms.ReportParameter("txtResidentName", lblResidentName.Text),
                new Microsoft.Reporting.WinForms.ReportParameter("txtPayment", cmbPaid.Text),
                new Microsoft.Reporting.WinForms.ReportParameter("txtChange", changeAmountForReport), // Use the processed value
                new Microsoft.Reporting.WinForms.ReportParameter("txtAmountCovered", lblAmountPaid.Text),
                new Microsoft.Reporting.WinForms.ReportParameter("txtMonthCovered", lblMonthCovered.Text),
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
            PopulatePaymentComboBox(cmbPaid);
            PopulateChangeComboBox(cmbChange);
            this.cmbPaid.SelectedIndexChanged += new System.EventHandler(this.cmbPayment_HandleOther);
            this.cmbChange.SelectedIndexChanged += new System.EventHandler(this.cmbPayment_HandleOther);
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
        private void PopulatePaymentComboBox(ComboBox cmb)
        {
            cmb.Items.Clear();
            for (int i = 100; i <= 1000; i += 100)
            {
                cmb.Items.Add(i.ToString("F2"));
            }
            cmb.Items.Add("Other");
            cmb.SelectedIndex = 0; 
        }
        private void PopulateChangeComboBox(ComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.Items.Add("(None)"); 
            cmb.Items.Add("0.00");    

            for (int i = 100; i <= 1000; i += 100)
            {
                cmb.Items.Add(i.ToString("F2"));
            }

            cmb.Items.Add("Other");
            cmb.SelectedIndex = 1;
        }
        private void cmbPayment_HandleOther(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb != null && cmb.SelectedItem?.ToString() == "Other")
            {
                string input = "";
                decimal value;

                while (true)
                {
                    input = Microsoft.VisualBasic.Interaction.InputBox(
                        "Please enter a specific amount:", "Enter Amount", "");

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        cmb.SelectedIndex = 0; 
                        return;
                    }
                    if (decimal.TryParse(input, out value) && value >= 0)
                    {
                        string formattedValue = value.ToString("F2");
                        if (!cmb.Items.Contains(formattedValue))
                        {
                            int otherIndex = cmb.Items.IndexOf("Other");
                            if (otherIndex > -1)
                            {
                                cmb.Items.Insert(otherIndex, formattedValue);
                                cmb.SelectedItem = formattedValue;
                            }
                        }
                        else
                        {
                            cmb.SelectedItem = formattedValue;
                        }
                        break; 
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please enter only numbers and decimals.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    }

