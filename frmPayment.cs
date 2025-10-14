using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
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
        private bool _isUpdatingChecks = false;
        private const decimal BaseDueRate = 100;

        public frmPayment()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            displayedUnits = new List<Tuple<int, string, bool, string, string, int, int>>();
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

            this.dtpPaymentDate.ValueChanged += new System.EventHandler(this.dtpPaymentDate_ValueChanged);
            this.clbAdvanceMonths.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbAdvanceMonths_ItemCheck);
            this.btnToggleSelectAll.Click += new System.EventHandler(this.btnToggleSelectAll_Click);
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
                    query = @"SELECT u.UnitID, u.UnitNumber, u.IsOccupied, u.Block, u.UnitType, u.TotalRooms, 
                            (SELECT COUNT(*) FROM HomeownerUnits hu_c JOIN Residents r_c ON hu_c.ResidentID = r_c.ResidentID WHERE hu_c.UnitID = u.UnitID AND hu_c.IsCurrent = 1 AND r_c.IsActive = 1 AND r_c.ResidencyType != 'Owner') AS OccupantCount
                      FROM HomeownerUnits hu
                      INNER JOIN TBL_Units u ON hu.UnitID = u.UnitID
                      WHERE hu.ResidentID = @ownerResidentId AND hu.IsCurrent = 1";
                }
                else
                {
                    query = @"SELECT u.UnitID, u.UnitNumber, u.IsOccupied, u.Block, u.UnitType, u.TotalRooms,
                            (SELECT COUNT(*) FROM HomeownerUnits hu_c JOIN Residents r_c ON hu_c.ResidentID = r_c.ResidentID WHERE hu_c.UnitID = u.UnitID AND hu_c.IsCurrent = 1 AND r_c.IsActive = 1 AND r_c.ResidencyType != 'Owner') AS OccupantCount
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
                                reader["OccupantCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["OccupantCount"])
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

            lblResidentAddress.Text = $"Unit {unitNumber} Block {block}, {homeAddress}";
            UpdateUnitStatusLabel();

            lblDueRate.Text = BaseDueRate.ToString("N2");
            if (cmbResidency.SelectedIndex != 0)
            {
                cmbResidency.SelectedIndex = 0;
            }
            else
            {
                cmbResidency_SelectedIndexChanged(null, null);
            }

            SetValidStartDate();
        }

        private void UpdateUnitStatusLabel()
        {
            if (cmbUnits.SelectedIndex == -1)
            {
                lblUnitStatus.Text = "";
                return;
            }

            var selectedUnit = displayedUnits[cmbUnits.SelectedIndex];
            string unitType = selectedUnit.Item5;
            int totalRooms = selectedUnit.Item6;
            int occupantCount = selectedUnit.Item7;
            string residencyType = cmbResidency.SelectedItem.ToString();

            if (unitType == "Apartment")
            {
                lblUnitStatus.Text = $"{occupantCount}/{totalRooms} Occupied Rooms";
                lblUnitStatus.ForeColor = Color.Green;
            }
            else 
            {
                if (residencyType == "Owner")
                {
                    lblUnitStatus.Text = "Occupied";
                    lblUnitStatus.ForeColor = Color.Green;
                }
                else 
                {
                    lblUnitStatus.Text = occupantCount > 0 ? "Occupied" : "Vacant";
                    lblUnitStatus.ForeColor = occupantCount > 0 ? Color.Green : Color.Red;
                }
            }
        }

        private void SetValidStartDate()
        {
            if (currentOwnerResidentId <= 0 || currentUnitId <= 0) return;

            DateTime lastPaidMonth = DateTime.MinValue;
            DateTime nextDueMonth;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT TOP 1 MonthCovered 
                         FROM MonthlyDues 
                         WHERE ResidentID = @residentId AND UnitID = @unitId 
                         ORDER BY CONVERT(DATETIME, '01 ' + MonthCovered) DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                    cmd.Parameters.AddWithValue("@unitId", currentUnitId);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lastPaidMonth = DateTime.ParseExact(result.ToString(), "MMMM yyyy", CultureInfo.InvariantCulture);
                    }
                }
            }

            if (lastPaidMonth != DateTime.MinValue)
            {
                nextDueMonth = lastPaidMonth.AddMonths(1);
            }
            else
            {
                nextDueMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            clbAdvanceMonths.Items.Clear();
            for (int i = 0; i < 12; i++)
            {
                clbAdvanceMonths.Items.Add(nextDueMonth.AddMonths(i).ToString("MMMM yyyy"));
            }
            UpdateAmountPaidLabel();
            UpdateMonthCoveredLabel();
            UpdateToggleSelectAllButtonText();
        }

        private void dtpPaymentDate_ValueChanged(object sender, EventArgs e)
        {
            PopulateAdvanceMonths();
        }

        private void PopulateAdvanceMonths()
        {
            clbAdvanceMonths.Items.Clear();
            DateTime startDate = dtpPaymentDate.Value;

            for (int i = 0; i < 12; i++)
            {
                clbAdvanceMonths.Items.Add(startDate.AddMonths(i).ToString("MMMM yyyy"));
            }

            UpdateAmountPaidLabel();
            UpdateMonthCoveredLabel();
            UpdateToggleSelectAllButtonText();
        }
        private void clbAdvanceMonths_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_isUpdatingChecks) return;
            if (e.NewValue == CheckState.Checked)
            {
                if (e.Index > 0 && !clbAdvanceMonths.GetItemChecked(e.Index - 1))
                {
                    MessageBox.Show("Please select the months in chronological order.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.NewValue = e.CurrentValue;
                    return;
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (e.Index < clbAdvanceMonths.Items.Count - 1 && clbAdvanceMonths.GetItemChecked(e.Index + 1))
                {
                    MessageBox.Show("Please deselect later months first before deselecting this one.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.NewValue = e.CurrentValue;
                    return;
                }
            }

            this.BeginInvoke((Action)(() =>
            {
                UpdateAmountPaidLabel();
                UpdateMonthCoveredLabel();
                UpdateToggleSelectAllButtonText();
            }));
        }

        private void cmbResidency_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUnitStatusLabel();

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
            if (clbAdvanceMonths.CheckedItems.Count == 0)
            {
                MessageBox.Show("No payment to save. Please select at least one month.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(lblAmountPaid.Text, out decimal totalAmount) || totalAmount <= 0)
            {
                MessageBox.Show("No payment to save. Please select a valid month.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    "Underpayment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            decimal actualChangeGiven;
            decimal.TryParse(cmbChange.Text, out actualChangeGiven);

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

            List<string> duplicateMonths = new List<string>();
            int monthsSaved = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                foreach (var item in clbAdvanceMonths.CheckedItems)
                {
                    string monthCovered = item.ToString();
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
                string changeAmountForReport = cmbChange.Text;

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
            lblAmountPaid.Text = (clbAdvanceMonths.CheckedItems.Count * BaseDueRate).ToString("F2");
        }

        private void UpdateMonthCoveredLabel()
        {
            if (clbAdvanceMonths.CheckedItems.Count == 0)
            {
                lblMonthCovered.Text = "No months selected";
            }
            else if (clbAdvanceMonths.CheckedItems.Count == 1)
            {
                lblMonthCovered.Text = clbAdvanceMonths.CheckedItems[0].ToString();
            }
            else
            {
                string firstMonth = clbAdvanceMonths.CheckedItems[0].ToString();
                string lastMonth = clbAdvanceMonths.CheckedItems[clbAdvanceMonths.CheckedItems.Count - 1].ToString();
                lblMonthCovered.Text = $"{firstMonth} - {lastMonth}";
            }
        }
        private void btnToggleSelectAll_Click(object sender, EventArgs e)
        {
            bool allAreChecked = (clbAdvanceMonths.CheckedItems.Count == clbAdvanceMonths.Items.Count);
            bool shouldBeChecked = !allAreChecked;

            try
            {
                _isUpdatingChecks = true;

                for (int i = 0; i < clbAdvanceMonths.Items.Count; i++)
                {
                    clbAdvanceMonths.SetItemChecked(i, shouldBeChecked);
                }
            }
            finally
            {
                _isUpdatingChecks = false;
            }

            UpdateAmountPaidLabel();
            UpdateMonthCoveredLabel();
            UpdateToggleSelectAllButtonText();
        }
        private void UpdateToggleSelectAllButtonText()
        {
            if (clbAdvanceMonths.Items.Count == 0)
            {
                btnToggleSelectAll.Enabled = false;
                btnToggleSelectAll.Text = "Select All";
            }
            else
            {
                btnToggleSelectAll.Enabled = true;
                if (clbAdvanceMonths.CheckedItems.Count == clbAdvanceMonths.Items.Count)
                {
                    btnToggleSelectAll.Text = "Deselect All";
                }
                else
                {
                    btnToggleSelectAll.Text = "Select All";
                }
            }
        }

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
            cmb.Items.Add("0.00");
            for (int i = 100; i <= 500; i += 100) { cmb.Items.Add(i.ToString("F2")); }
            cmb.Items.Add("Other...");
            cmb.SelectedIndex = 0; 
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
        private void clbAdvanceMonths_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}