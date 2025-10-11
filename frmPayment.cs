using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class frmPayment : Form
    {
        private int currentOwnerResidentId;
        private int currentUnitId;
        private string ownerFullName;
        private string homeAddress;
        private List<Tuple<int, string, bool, string, string, int, int>> displayedUnits;

        private const decimal BaseDueRate = 100;

        public frmPayment()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            displayedUnits = new List<Tuple<int, string, bool, string, string, int, int>>();
            this.dtpPaymentDate.ValueChanged += new EventHandler(this.dtpPaymentDate_ValueChanged);
            this.dtpEndMonth.ValueChanged += new EventHandler(this.dtpEndMonth_ValueChanged);
            this.Load += new EventHandler(this.frmPayment_Load);
        }

        private void frmPayment_Load(object sender, EventArgs e)
        {
            cmbResidency.Items.AddRange(new object[] { "Owner", "Tenant", "Caretaker" });
            cmbResidency.SelectedIndex = 0;
            cmbResidency.SelectedIndexChanged += cmbResidency_SelectedIndexChanged;

            cmbNames.Visible = false;
            lblNames.Visible = false;
            cmbNames.SelectedIndexChanged += cmbNames_SelectedIndexChanged;

            cmbRemarks.Items.Clear();
            cmbRemarks.Items.Add("N/A");
            cmbRemarks.Items.Add("Others...");
            cmbRemarks.SelectedIndex = 0;
            cmbRemarks.SelectedIndexChanged += cmbRemarks_SelectedIndexChanged;
            lblResidentName.TextChanged += lblResidentName_TextChanged;

            PopulatePaymentComboBox(cmbPaid);
            PopulateChangeComboBox(cmbChange);

            this.cmbPaid.SelectedIndexChanged += new System.EventHandler(this.cmbPayment_HandleOther);
            this.cmbChange.SelectedIndexChanged += new System.EventHandler(this.cmbPayment_HandleOther);
        }

        private void btnSelectHomeowner_Click(object sender, EventArgs e)
        {
            using (frmSelectHomeowner selectForm = new frmSelectHomeowner())
            {
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    int selectedId = selectForm.SelectedHomeownerId;

                    txtHomeownerIDDisplay.Text = selectedId.ToString();
                    LoadHomeownerData(selectedId);
                }
            }
        }

        private void LoadHomeownerData(int homeownerId)
        {
            // Reset the entire form state
            lblResidentName.Text = "";
            lblResidentAddress.Text = "";
            lblUnitStatus.Text = "";
            cmbUnits.Items.Clear();
            displayedUnits.Clear();
            cmbResidency.SelectedIndex = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string residentQuery = @"SELECT ResidentID, FirstName, MiddleName, LastName, HomeAddress, IsActive 
                                         FROM Residents 
                                         WHERE HomeownerID = @homeownerId AND ResidencyType = 'Owner'";
                using (SqlCommand cmd = new SqlCommand(residentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Owner with that Homeowner ID not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (!Convert.ToBoolean(reader["IsActive"]))
                        {
                            MessageBox.Show("This owner is inactive and cannot process payments.", "Inactive Resident", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        currentOwnerResidentId = Convert.ToInt32(reader["ResidentID"]);
                        ownerFullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                        homeAddress = reader["HomeAddress"].ToString();
                        lblResidentName.Text = ownerFullName;
                        lblResidentAddress.Text = homeAddress;
                    }
                }
            }
            FilterAndDisplayUnits();
        }
        private void FilterAndDisplayUnits()
        {
            cmbUnits.Items.Clear();
            displayedUnits.Clear();
            lblUnitStatus.Text = "";
            currentUnitId = 0;

            if (currentOwnerResidentId <= 0) return;

            string payerType = cmbResidency.SelectedItem.ToString();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query;
                if (payerType == "Owner")
                {
                    query = @"SELECT u.UnitID, u.UnitNumber, u.IsOccupied, u.Block, u.UnitType, u.TotalRooms, u.AvailableRooms
                              FROM HomeownerUnits hu
                              INNER JOIN TBL_Units u ON hu.UnitID = u.UnitID
                              WHERE hu.ResidentID = @ownerResidentId AND hu.IsCurrent = 1";
                }
                else
                {
                    query = @"SELECT u.UnitID, u.UnitNumber, u.IsOccupied, u.Block, u.UnitType, u.TotalRooms, u.AvailableRooms
                              FROM TBL_Units u
                              JOIN HomeownerUnits hu_owner ON u.UnitID = hu_owner.UnitID
                              WHERE hu_owner.ResidentID = @ownerResidentId AND hu_owner.IsCurrent = 1
                              AND EXISTS (
                                  SELECT 1
                                  FROM HomeownerUnits hu_other
                                  JOIN Residents r_other ON hu_other.ResidentID = r_other.ResidentID
                                  WHERE hu_other.UnitID = u.UnitID
                                    AND r_other.ResidencyType = @payerType
                                    AND r_other.IsActive = 1
                              )";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ownerResidentId", currentOwnerResidentId);
                    if (payerType != "Owner")
                    {
                        cmd.Parameters.AddWithValue("@payerType", payerType);
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            displayedUnits.Add(new Tuple<int, string, bool, string, string, int, int>(
                                Convert.ToInt32(reader["UnitID"]),
                                reader["UnitNumber"].ToString(),
                                Convert.ToBoolean(reader["IsOccupied"]),
                                reader["Block"].ToString(),
                                reader["UnitType"].ToString(),
                                reader["TotalRooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalRooms"]),
                                reader["AvailableRooms"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AvailableRooms"])
                            ));
                            cmbUnits.Items.Add($"Unit {reader["UnitNumber"]} Block {reader["Block"]}");
                        }
                    }
                }
            }

            if (displayedUnits.Count == 1)
            {
                cmbUnits.SelectedIndex = 0;
            }
            else if (displayedUnits.Count > 1)
            {
                cmbUnits.Enabled = true;
            }
            else
            {
                MessageBox.Show($"This owner has no units with active {payerType}s.", "No Units Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1) return;
            var selectedUnit = displayedUnits[cmbUnits.SelectedIndex];

            currentUnitId = selectedUnit.Item1;
            string unitNumber = selectedUnit.Item2;
            string block = selectedUnit.Item4;
            string unitType = selectedUnit.Item5;
            int totalRooms = selectedUnit.Item6;
            int availableRooms = selectedUnit.Item7;

            lblResidentAddress.Text = $"Unit {unitNumber} Block {block}, {homeAddress}";

            if (unitType == "Apartment")
            {
                int occupiedRooms = totalRooms - availableRooms;
                lblUnitStatus.Text = $"{occupiedRooms}/{totalRooms} Occupied Rooms";
                lblUnitStatus.ForeColor = Color.Green;
            }
            else
            {
                lblUnitStatus.Text = "Occupied";
                lblUnitStatus.ForeColor = Color.Green;
            }

            lblDueRate.Text = BaseDueRate.ToString("N2");

            cmbResidency_SelectedIndexChanged(null, null); 
            UpdateAmountPaidLabel();
        }

        private void cmbResidency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                FilterAndDisplayUnits();
            }

            string selection = cmbResidency.SelectedItem.ToString();
            if (selection == "Owner")
            {
                lblNames.Visible = false;
                cmbNames.Visible = false;
                cmbNames.Items.Clear();
                lblResidentName.Text = ownerFullName;
            }
            else
            {
                if (currentUnitId > 0)
                {
                    cmbNames.Items.Clear();
                    using (SqlConnection conn = DatabaseHelper.GetConnection())
                    {
                        string query = @"SELECT r.FirstName, r.MiddleName, r.LastName FROM Residents r
                                         JOIN HomeownerUnits hu ON r.ResidentID = hu.ResidentID
                                         WHERE hu.UnitID = @unitId AND r.ResidencyType = @residencyType AND r.IsActive = 1";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@unitId", currentUnitId);
                            cmd.Parameters.AddWithValue("@residencyType", selection);
                            conn.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string name = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                                    cmbNames.Items.Add(name.Trim());
                                }
                            }
                        }
                    }

                    if (cmbNames.Items.Count > 0)
                    {
                        lblNames.Visible = true;
                        cmbNames.Visible = true;
                        cmbNames.Enabled = true;
                        cmbNames.Text = "";
                        lblResidentName.Text = "<- Select a name";
                    }
                    else
                    {
                        lblNames.Visible = false;
                        cmbNames.Visible = false;
                        lblResidentName.Text = ownerFullName;
                    }
                }
            }
        }

        private void cmbNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbNames.SelectedItem != null)
            {
                lblResidentName.Text = cmbNames.SelectedItem.ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (currentOwnerResidentId <= 0 || currentUnitId <= 0)
            {
                MessageBox.Show("Please select an owner and a unit first.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(lblAmountPaid.Text, out decimal totalAmount) || totalAmount <= 0)
            {
                MessageBox.Show("No payment to save. Please select a valid month range.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string payerType = cmbResidency.SelectedItem.ToString();
            if (payerType != "Owner" && (cmbNames.SelectedItem == null || !cmbNames.Enabled))
            {
                MessageBox.Show($"Please select a valid {payerType}'s name. If none are available, payment must be made by the Owner.", "Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(cmbPaid.Text, out decimal amountPaid))
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (amountPaid < totalAmount)
            {
                MessageBox.Show(
                    $"The payment amount (₱{amountPaid:N2}) is less than the total amount due (₱{totalAmount:N2}).\n\nPlease correct the payment amount or the months covered.",
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
                string reason;
                do
                {
                    reason = Microsoft.VisualBasic.Interaction.InputBox("Please specify the reason for the change discrepancy:", "Justification Required", "");
                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        var result = MessageBox.Show("You must provide a reason to continue. Try again?", "Reason Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (result == DialogResult.No) { return; }
                    }
                } while (string.IsNullOrWhiteSpace(reason));
                decimal discrepancyAmount = Math.Abs(expectedChange - actualChangeGiven);
                string discrepancyPrefix = actualChangeGiven < expectedChange ? $"Change Discrepancy (Less by {discrepancyAmount:N2})" : $"Change Discrepancy (More by {discrepancyAmount:N2})";
                string finalRemark = $"{discrepancyPrefix}: {reason}";
                if (!cmbRemarks.Items.Contains(finalRemark)) { cmbRemarks.Items.Add(finalRemark); }
                cmbRemarks.SelectedItem = finalRemark;
            }

            string payerName = null;
            if (payerType != "Owner")
            {
                payerName = cmbNames.SelectedItem.ToString();
            }

            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);
            int monthsToCover = ((endMonth.Year - startMonth.Year) * 12) + endMonth.Month - startMonth.Month + 1;
            List<string> duplicateMonths = new List<string>();
            int monthsSaved = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                for (int i = 0; i < monthsToCover; i++)
                {
                    DateTime month = startMonth.AddMonths(i);
                    string monthCovered = month.ToString("MMMM yyyy");
                    string checkQuery = "SELECT COUNT(*) FROM MonthlyDues WHERE ResidentID=@residentId AND UnitID=@unitId AND MonthCovered=@monthCovered";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                        checkCmd.Parameters.AddWithValue("@unitId", currentUnitId);
                        checkCmd.Parameters.AddWithValue("@monthCovered", monthCovered);
                        if ((int)checkCmd.ExecuteScalar() > 0) { duplicateMonths.Add(monthCovered); continue; }
                    }

                    string insertQuery = @"INSERT INTO MonthlyDues (ResidentID, UnitID, PaymentDate, AmountPaid, DueRate, MonthCovered, ProcessedByUserID, Remarks, PaidByResidencyType, PaidByResidentName)
                                           VALUES (@residentId, @unitId, @paymentDate, @amountPaid, @dueRate, @monthCovered, @processedByUserID, @remarks, @paidByResidencyType, @paidByResidentName)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                        cmd.Parameters.AddWithValue("@unitId", currentUnitId);
                        cmd.Parameters.AddWithValue("@paymentDate", dtpPaymentDate.Value.Date);
                        cmd.Parameters.AddWithValue("@amountPaid", BaseDueRate);
                        cmd.Parameters.AddWithValue("@dueRate", BaseDueRate);
                        cmd.Parameters.AddWithValue("@monthCovered", monthCovered);
                        cmd.Parameters.AddWithValue("@processedByUserID", CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@remarks", cmbRemarks.Text);
                        cmd.Parameters.AddWithValue("@paidByResidencyType", payerType);
                        cmd.Parameters.AddWithValue("@paidByResidentName", (object)payerName ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                        monthsSaved++;
                    }
                }
            }

            string finalMessage = "";
            if (monthsSaved > 0)
            {
                finalMessage = $"{monthsSaved} month(s) of payment saved successfully!";
                ShowReceipt();
            }
            if (duplicateMonths.Count > 0) { finalMessage += $"\n\nThe following month(s) were skipped as they were already paid:\n{string.Join(", ", duplicateMonths)}"; }
            if (!string.IsNullOrEmpty(finalMessage)) { MessageBox.Show(finalMessage, "Save Complete", MessageBoxButtons.OK, monthsSaved > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning); }
            if (monthsSaved > 0) { this.Close(); }
        }

        private void ShowReceipt()
        {
            using (var receipt = new Form())
            {
                receipt.Text = "Payment Receipt";
                receipt.StartPosition = FormStartPosition.CenterParent;
                receipt.Width = 800;
                receipt.Height = 600;
                var reportViewer = new ReportViewer { Dock = DockStyle.Fill, ProcessingMode = ProcessingMode.Local };
                receipt.Controls.Add(reportViewer);
                reportViewer.LocalReport.ReportEmbeddedResource = "RECOMANAGESYS.PaymentReceipt.rdlc";
                string changeAmountForReport = cmbChange.SelectedItem?.ToString() == "(None)" ? "" : cmbChange.Text;

                var parameters = new ReportParameter[]
                {
                    new ReportParameter("txtResidentName", lblResidentName.Text),
                    new ReportParameter("txtHomeownerID", txtHomeownerIDDisplay.Text),
                    new ReportParameter("txtPayment", cmbPaid.Text),
                    new ReportParameter("txtChange", changeAmountForReport),
                    new ReportParameter("txtAmountCovered", lblAmountPaid.Text),
                    new ReportParameter("txtMonthCovered", lblMonthCovered.Text),
                    new ReportParameter("txtRemarks", cmbRemarks.Text),
                    new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy, hh:mm tt")),
                    new ReportParameter("txtOfficerName", CurrentUser.FullName),
                    new ReportParameter("txtOfficerPosition", CurrentUser.Role)
                };
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.RefreshReport();
                receipt.ShowDialog();
            }
        }

        private void UpdateAmountPaidLabel()
        {
            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);
            if (endMonth < startMonth) { lblAmountPaid.Text = "0.00"; return; }
            int monthsToPay = ((endMonth.Year - startMonth.Year) * 12) + endMonth.Month - startMonth.Month + 1;
            lblAmountPaid.Text = (monthsToPay * BaseDueRate).ToString("F2");
        }

        private void UpdateMonthCoveredLabel()
        {
            DateTime startMonth = new DateTime(dtpPaymentDate.Value.Year, dtpPaymentDate.Value.Month, 1);
            DateTime endMonth = new DateTime(dtpEndMonth.Value.Year, dtpEndMonth.Value.Month, 1);
            if (endMonth < startMonth) lblMonthCovered.Text = "Invalid range";
            else if (startMonth == endMonth) lblMonthCovered.Text = startMonth.ToString("MMMM yyyy");
            else lblMonthCovered.Text = $"{startMonth:MMMM yyyy} - {endMonth:MMMM yyyy}";
        }

        private void dtpPaymentDate_ValueChanged(object sender, EventArgs e) { UpdateMonthCoveredLabel(); UpdateAmountPaidLabel(); }
        private void dtpEndMonth_ValueChanged(object sender, EventArgs e) { UpdateMonthCoveredLabel(); UpdateAmountPaidLabel(); }
        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }

        private void cmbRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRemarks.SelectedItem.ToString() == "Others...")
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Please specify your remark:", "Other Remark", "");
                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (!cmbRemarks.Items.Contains(input)) { cmbRemarks.Items.Insert(cmbRemarks.Items.Count - 1, input); }
                    cmbRemarks.SelectedItem = input;
                }
                else { cmbRemarks.SelectedIndex = 0; }
            }
        }

        private void PopulatePaymentComboBox(ComboBox cmb)
        {
            cmb.Items.Clear();
            for (int i = 100; i <= 1000; i += 100) { cmb.Items.Add(i.ToString("F2")); }
            cmb.Items.Add("Other...");
            cmb.SelectedIndex = 0;
        }

        private void PopulateChangeComboBox(ComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.Items.Add("(None)");
            cmb.Items.Add("0.00");
            for (int i = 100; i <= 500; i += 100) { cmb.Items.Add(i.ToString("F2")); }
            cmb.Items.Add("Other...");
            cmb.SelectedIndex = 1;
        }

        private void cmbPayment_HandleOther(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb != null && cmb.SelectedItem?.ToString() == "Other...")
            {
                string input;
                decimal value;
                while (true)
                {
                    input = Microsoft.VisualBasic.Interaction.InputBox("Please enter a specific amount:", "Enter Amount", "");
                    if (string.IsNullOrWhiteSpace(input)) { cmb.SelectedIndex = 0; return; }
                    if (decimal.TryParse(input, out value) && value >= 0)
                    {
                        string formattedValue = value.ToString("F2");
                        if (!cmb.Items.Contains(formattedValue))
                        {
                            int otherIndex = cmb.Items.IndexOf("Other...");
                            if (otherIndex > -1) { cmb.Items.Insert(otherIndex, formattedValue); }
                        }
                        cmb.SelectedItem = formattedValue;
                        break;
                    }
                    else { MessageBox.Show("Invalid input. Please enter only positive numbers and decimals.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
            
            private void lblResidentName_TextChanged(object sender, EventArgs e)
        {
            if (lblResidentName.Text == "<- Select a name")
            {
                lblResidentName.ForeColor = Color.Red;
            }
            else
            {
                lblResidentName.ForeColor = SystemColors.ControlText;
            }

        }
    }
 }


