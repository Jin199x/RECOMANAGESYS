using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Microsoft.VisualBasic;
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class UpdateMonthlyDues : Form
    {
        public Action OnPaymentSaved;
        private int currentOwnerResidentId = -1;
        private string ownerFullName = "";
        private List<Tuple<int, string, string, string>> displayedUnits;
        private decimal dueRate = 100;
        private bool _isUpdatingChecks = false;

        // <<< ADD THIS FIELD
        private int _initialHomeownerId = -1;

        public UpdateMonthlyDues()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            displayedUnits = new List<Tuple<int, string, string, string>>();
            // Leave the event handlers here
            this.Load += UpdateMonthlyDues_Load;
            btnSelectHomeowner.Click += btnSelectHomeowner_Click;
            cmbResidency.SelectedIndexChanged += cmbResidency_SelectedIndexChanged;
            cmbUnits.SelectedIndexChanged += cmbUnits_SelectedIndexChanged;
            clbMissedMonths.ItemCheck += clbMissedMonths_ItemCheck;
            btnToggleSelectAll.Click += btnToggleSelectAll_Click;
        }

        private void UpdateMonthlyDues_Load(object sender, EventArgs e)
        {
            cmbResidency.Items.AddRange(new object[] { "Owner", "Tenant", "Caretaker" });
            cmbResidency.SelectedIndex = 0;
            lblNames.Visible = false;
            cmbNames.Visible = false;

            PopulatePaymentComboBox(cmbPaid);
            PopulateChangeComboBox(cmbChange);
            cmbRemarks.Items.AddRange(new object[] { "N/A", "Others..." });
            cmbRemarks.SelectedIndex = 0;

            cmbPaid.SelectedIndexChanged += cmbPayment_HandleOther;
            cmbChange.SelectedIndexChanged += cmbPayment_HandleOther;
            cmbRemarks.SelectedIndexChanged += cmbRemarks_SelectedIndexChanged;
        }

        private void btnSelectHomeowner_Click(object sender, EventArgs e)
        {
            using (frmSelectHomeowner selectForm = new frmSelectHomeowner())
            {
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    txtHomeownerIDDisplay.Text = selectForm.SelectedHomeownerId.ToString();
                    LoadHomeownerData(selectForm.SelectedHomeownerId);
                }
            }
        }

        private void LoadHomeownerData(int homeownerId)
        {
            lblResidentName.Text = "";
            lblUnitAddress.Text = "";
            clbMissedMonths.Items.Clear();
            lblRemainingDebt.Text = "0.00";
            lblTotalAmount.Text = "0.00";
            cmbResidency.SelectedIndex = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string residentQuery = @"SELECT ResidentID, FirstName, MiddleName, LastName FROM Residents 
                                         WHERE HomeownerID = @homeownerId AND ResidencyType = 'Owner' AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(residentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@homeownerId", homeownerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Active owner with that Homeowner ID not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        currentOwnerResidentId = Convert.ToInt32(reader["ResidentID"]);
                        ownerFullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}".Trim();
                        lblResidentName.Text = ownerFullName;
                    }
                }
            }
            FilterAndDisplayUnits();
        }

        private void cmbResidency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentOwnerResidentId > 0)
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
                lblNames.Visible = true;
                cmbNames.Visible = true;
                cmbNames.Enabled = false;
                cmbNames.Text = "Select a unit first...";
            }
        }

        private void FilterAndDisplayUnits()
        {
            cmbUnits.Items.Clear();
            displayedUnits.Clear();
            ResetPaymentUI();

            if (currentOwnerResidentId <= 0) return;

            string payerType = cmbResidency.SelectedItem.ToString();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query;
                if (payerType == "Owner")
                {
                    query = @"SELECT u.UnitID, u.UnitNumber, u.Block, r.HomeAddress
                              FROM HomeownerUnits hu
                              JOIN Residents r ON hu.ResidentID = r.ResidentID
                              JOIN TBL_Units u ON hu.UnitID = u.UnitID
                              WHERE hu.ResidentID = @ownerResidentId AND hu.IsCurrent = 1 AND r.ResidencyType = 'Owner'";
                }
                else
                {
                    query = @"SELECT u.UnitID, u.UnitNumber, u.Block, r_owner.HomeAddress
                              FROM TBL_Units u
                              JOIN HomeownerUnits hu_owner ON u.UnitID = hu_owner.UnitID
                              JOIN Residents r_owner ON hu_owner.ResidentID = r_owner.ResidentID
                              WHERE hu_owner.ResidentID = @ownerResidentId AND hu_owner.IsCurrent = 1
                              AND EXISTS (
                                  SELECT 1 FROM HomeownerUnits hu_other
                                  JOIN Residents r_other ON hu_other.ResidentID = r_other.ResidentID
                                  WHERE hu_other.UnitID = u.UnitID AND r_other.ResidencyType = @payerType AND r_other.IsActive = 1
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
                            displayedUnits.Add(new Tuple<int, string, string, string>(
                                Convert.ToInt32(reader["UnitID"]),
                                reader["UnitNumber"].ToString(),
                                reader["Block"].ToString(),
                                reader["HomeAddress"].ToString()
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
            else if (payerType != "Owner")
            {
                MessageBox.Show($"This owner has no units with active {payerType}s.", "No Units Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1) return;

            var selectedUnit = displayedUnits[cmbUnits.SelectedIndex];
            int unitId = selectedUnit.Item1;
            string unitNumber = selectedUnit.Item2;
            string block = selectedUnit.Item3;
            string address = selectedUnit.Item4;

            lblUnitAddress.Text = $"Unit {unitNumber} Block {block}, {address}";

            PopulatePayerNames(unitId);
            LoadMissedMonths(unitId);
        }

        private void PopulatePayerNames(int unitId)
        {
            string payerType = cmbResidency.SelectedItem.ToString();
            if (payerType == "Owner") return;

            cmbNames.Items.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT r.FirstName, r.MiddleName, r.LastName FROM Residents r
                                 JOIN HomeownerUnits hu ON r.ResidentID = hu.ResidentID
                                 WHERE hu.UnitID = @unitId AND r.ResidencyType = @payerType AND r.IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    cmd.Parameters.AddWithValue("@payerType", payerType);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbNames.Items.Add($"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}".Trim());
                        }
                    }
                }
            }
            cmbNames.Enabled = cmbNames.Items.Count > 0;
            lblResidentName.Text = ownerFullName;
        }

        private void cmbNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbNames.SelectedItem != null)
            {
                lblResidentName.Text = cmbNames.SelectedItem.ToString();
            }
        }

        private void LoadMissedMonths(int unitId)
        {
            clbMissedMonths.Items.Clear();
            var paidMonths = new HashSet<string>();
            DateTime? dateOfOwnership = null; // start date 

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string ownershipQuery = "SELECT DateOfOwnership FROM HomeownerUnits WHERE ResidentID = @residentId AND UnitID = @unitId AND IsCurrent = 1";
                using (SqlCommand cmd = new SqlCommand(ownershipQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        dateOfOwnership = Convert.ToDateTime(result);
                    }
                }

                if (!dateOfOwnership.HasValue)
                {
                    MessageBox.Show("Could not determine the Date of Ownership for this unit. Cannot calculate missed months.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT MonthCovered FROM MonthlyDues WHERE ResidentID=@residentId AND UnitID=@unitId", conn))
                {
                    cmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paidMonths.Add(reader["MonthCovered"].ToString());
                        }
                    }
                }
            }

            DateTime now = DateTime.Now;
            for (DateTime monthIterator = dateOfOwnership.Value; monthIterator < now; monthIterator = monthIterator.AddMonths(1))
            {
                string monthName = monthIterator.ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName))
                {
                    clbMissedMonths.Items.Add(monthName);
                }
            }

            lblRemainingDebt.Text = (clbMissedMonths.Items.Count * dueRate).ToString("F2");
            UpdateTotalAmount();
            UpdateToggleSelectAllButtonText();
        }

        private void clbMissedMonths_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_isUpdatingChecks)
            {
                return;
            }

            if (e.NewValue == CheckState.Checked)
            {
                if (e.Index > 0)
                {
                    if (!clbMissedMonths.GetItemChecked(e.Index - 1))
                    {
                        MessageBox.Show("Please select the months in chronological order.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.NewValue = e.CurrentValue;
                        return;
                    }
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (e.Index < clbMissedMonths.Items.Count - 1)
                {
                    if (clbMissedMonths.GetItemChecked(e.Index + 1))
                    {
                        MessageBox.Show("Please deselect later months first before deselecting this one.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.NewValue = e.CurrentValue;
                        return;
                    }
                }
            }

            this.BeginInvoke((Action)(() =>
            {
                UpdateTotalAmount();
                UpdateToggleSelectAllButtonText();
            }));
        }

        private void UpdateTotalAmount()
        {
            decimal total = clbMissedMonths.CheckedItems.Count * dueRate;
            lblTotalAmount.Text = total.ToString("F2");
        }

        private void savevisitor_Click(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1 || clbMissedMonths.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select a unit and at least one missed month to pay.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(lblTotalAmount.Text, out decimal totalAmount) || totalAmount <= 0)
            {
                MessageBox.Show("No payment to save. Please check at least one month.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(cmbPaid.Text, out decimal amountPaid))
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (amountPaid < totalAmount)
            {
                MessageBox.Show($"The amount paid (₱{amountPaid:N2}) is less than the total amount due (₱{totalAmount:N2}).", "Underpayment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    reason = Interaction.InputBox("Please provide a reason for the change discrepancy:", "Justification Required", "");
                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        var result = MessageBox.Show("A reason is required to proceed. Try again?", "Reason Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (result == DialogResult.No) return;
                    }
                } while (string.IsNullOrWhiteSpace(reason));

                decimal discrepancyAmount = Math.Abs(expectedChange - actualChangeGiven);
                string discrepancyPrefix = actualChangeGiven < expectedChange ? $"Change Short by ₱{expectedChange - actualChangeGiven:N2}" : $"Change Over by ₱{actualChangeGiven - expectedChange:N2}";
                string finalRemark = $"{discrepancyPrefix}: {reason}";
                if (!cmbRemarks.Items.Contains(finalRemark)) { cmbRemarks.Items.Insert(cmbRemarks.Items.Count - 1, finalRemark); }
                cmbRemarks.SelectedItem = finalRemark;
            }

            var unitInfo = displayedUnits[cmbUnits.SelectedIndex];
            int unitId = unitInfo.Item1;
            string payerType = cmbResidency.SelectedItem.ToString();
            string payerName = (payerType != "Owner" && cmbNames.SelectedItem != null) ? cmbNames.SelectedItem.ToString() : null;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                foreach (var month in clbMissedMonths.CheckedItems)
                {
                    string monthCovered = month.ToString();

                    string insertQuery = @"INSERT INTO MonthlyDues (ResidentID, UnitID, PaymentDate, AmountPaid, DueRate, MonthCovered, ProcessedByUserID, Remarks, PaidByResidencyType, PaidByResidentName)
                                           VALUES (@residentId, @unitId, @paymentDate, @amountPaid, @dueRate, @monthCovered, @processedBy, @remarks, @paidByResidencyType, @paidByResidentName)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@residentId", currentOwnerResidentId);
                        cmd.Parameters.AddWithValue("@unitId", unitId);
                        cmd.Parameters.AddWithValue("@paymentDate", dtpPaymentDate.Value.Date);
                        cmd.Parameters.AddWithValue("@amountPaid", dueRate);
                        cmd.Parameters.AddWithValue("@dueRate", dueRate);
                        cmd.Parameters.AddWithValue("@monthCovered", monthCovered);
                        cmd.Parameters.AddWithValue("@processedBy", CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@remarks", cmbRemarks.Text);
                        cmd.Parameters.AddWithValue("@paidByResidencyType", payerType);
                        cmd.Parameters.AddWithValue("@paidByResidentName", (object)payerName ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Payment(s) for missed months saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowReceipt();
            OnPaymentSaved?.Invoke();
            this.Close();
        }

        private void ShowReceipt()
        {
            using (var receiptForm = new Form())
            {
                receiptForm.Text = "Missed Payment Receipt";
                receiptForm.StartPosition = FormStartPosition.CenterParent;
                receiptForm.Width = 800;
                receiptForm.Height = 600;

                var reportViewer = new ReportViewer { Dock = DockStyle.Fill, ProcessingMode = ProcessingMode.Local };
                receiptForm.Controls.Add(reportViewer);

                reportViewer.LocalReport.ReportEmbeddedResource = "RECOMANAGESYS.PaymentReceipt.rdlc";

                string monthsPaid = string.Join(", ", clbMissedMonths.CheckedItems.OfType<string>());

                var parameters = new ReportParameter[]
                {
                    new ReportParameter("txtResidentName", lblResidentName.Text),
                    new ReportParameter("txtHomeownerID", txtHomeownerIDDisplay.Text),
                    new ReportParameter("txtPayment", cmbPaid.Text),
                    new ReportParameter("txtChange", cmbChange.SelectedItem?.ToString() == "(None)" ? "" : cmbChange.Text),
                    new ReportParameter("txtAmountCovered", lblTotalAmount.Text),
                    new ReportParameter("txtMonthCovered", monthsPaid),
                    new ReportParameter("txtRemarks", cmbRemarks.Text),
                    new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy, hh:mm tt")),
                    new ReportParameter("txtOfficerName", CurrentUser.FullName),
                    new ReportParameter("txtOfficerPosition", CurrentUser.Role)
                };

                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.RefreshReport();
                receiptForm.ShowDialog();
            }
        }

        private void ResetPaymentUI()
        {
            clbMissedMonths.Items.Clear();
            lblRemainingDebt.Text = "0.00";
            lblTotalAmount.Text = "0.00";
            lblUnitAddress.Text = "";
            cmbNames.Items.Clear();
            lblNames.Visible = false;
            cmbNames.Visible = false;
            UpdateToggleSelectAllButtonText();
        }

        private void cancelvisitor_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbRemarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRemarks.SelectedItem.ToString() == "Others...")
            {
                string input = Interaction.InputBox("Please specify your remark:", "Other Remark", "");
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
            for (int i = 100; i <= 2000; i += 100) { cmb.Items.Add(i.ToString("F2")); }
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
                    input = Interaction.InputBox("Please enter a specific amount:", "Enter Amount", "");
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
        private void btnToggleSelectAll_Click(object sender, EventArgs e)
        {
            bool allAreChecked = (clbMissedMonths.CheckedItems.Count == clbMissedMonths.Items.Count);
            bool shouldBeChecked = !allAreChecked;

            try
            {
                _isUpdatingChecks = true; //chronological logic

                for (int i = 0; i < clbMissedMonths.Items.Count; i++)
                {
                    clbMissedMonths.SetItemChecked(i, shouldBeChecked);
                }
            }
            finally
            {
                _isUpdatingChecks = false; 
            }

            UpdateTotalAmount();
            UpdateToggleSelectAllButtonText();
        }

        private void UpdateToggleSelectAllButtonText()
        {
            if (clbMissedMonths.Items.Count == 0)
            {
                btnToggleSelectAll.Enabled = false;
                btnToggleSelectAll.Text = "Select All";
            }
            else 
            {
                btnToggleSelectAll.Enabled = true;

                if (clbMissedMonths.CheckedItems.Count == clbMissedMonths.Items.Count)
                {
                    btnToggleSelectAll.Text = "Deselect All";
                }
                else
                {
                    btnToggleSelectAll.Text = "Select All";
                }
            }
        }
    }
}