using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class monthdues : UserControl
    {
        private int lastSelectedHomeownerId = -1;
        private int lastSelectedUnitId = -1;
        private string currentResidentFullName = "";
        private Microsoft.Reporting.WinForms.ReportViewer reportViewerForSaving;
        private Dictionary<int, int> homeownerToResidentIdMap = new Dictionary<int, int>();

        public class AccountDetail
        {
            public string Month { get; set; }
            public string Description { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal Balance { get; set; }
        }

        public class ReportData
        {
            public string FullName { get; set; }
            public string Address { get; set; }
            public string Contact { get; set; }
            public string HomeownerId { get; set; }
            public List<AccountDetail> AccountDetails { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal TotalCredit { get; set; }
            public decimal RunningBalance { get; set; }
            public string BalanceMessage { get; set; }
        }

        public monthdues()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            lvResidents.View = View.Details;
            lvResidents.FullRowSelect = true;
            lvResidents.CheckBoxes = true;
            lvResidents.Columns.Clear();

            lvResidents.Columns.Add("HomeownerID", 90);
            lvResidents.Columns.Add("Owner's Full Name", 200);
            lvResidents.Columns.Add("Address", 350);
            lvResidents.Columns.Add("Total Paid", 100);
            lvResidents.Columns.Add("Total Missed", 100);
            lvResidents.Columns.Add("Unit Type", 120);
            lvResidents.Columns.Add("Status", 100);

            lvMonths.View = View.Details;
            lvMonths.FullRowSelect = true;
            lvMonths.Columns.Clear();
            lvMonths.Columns.Add("Month", 150);
            lvMonths.Columns.Add("Status", 100);
            lvMonths.Columns.Add("Paid By", 200);
            lvMonths.Columns.Add("Payment Date", 150);

            LoadResidentsList();
            lvResidents.MouseClick += LvResidents_MouseClick;
            lvResidents.ItemChecked += lvResidents_ItemChecked;
            btnToggleSelect.Click += btnToggleSelect_Click;
        }

        public void RefreshData()
        {
            LoadResidentsList();
        }

        private void addvisitor_Click(object sender, EventArgs e)
        {
            frmPayment payform = new frmPayment();
            payform.ShowDialog();
            LoadResidentsList();
            if (lastSelectedHomeownerId != -1)
            {
                if (homeownerToResidentIdMap.TryGetValue(lastSelectedHomeownerId, out int residentId))
                {
                    LoadMonthlyDues(residentId, lastSelectedUnitId);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateMonthlyDues updateDues = new UpdateMonthlyDues();
            updateDues.OnPaymentSaved = () =>
            {
                if (lastSelectedHomeownerId != -1 && homeownerToResidentIdMap.TryGetValue(lastSelectedHomeownerId, out int residentId))
                {
                    LoadMonthlyDues(residentId, lastSelectedUnitId);
                }
            };
            updateDues.ShowDialog();
        }

        public void LoadResidentsList()
        {
            string currentFilter = cmbResidentFilter.SelectedItem?.ToString();
            int selectedHomeownerId = lastSelectedHomeownerId;
            bool? isActive = null;
            if (currentFilter == "Active Residents") isActive = true;
            else if (currentFilter == "Inactive Residents") isActive = false;

            LoadResidents(isActive, txtSearch.Text.Trim());

            if (selectedHomeownerId != -1)
            {
                foreach (ListViewItem item in lvResidents.Items)
                {
                    if (int.Parse(item.SubItems[0].Text) == selectedHomeownerId)
                    {
                        item.Selected = true;
                        item.Focused = true;
                        lvResidents.EnsureVisible(item.Index);
                        break;
                    }
                }
            }
        }

        private void LoadResidents(bool? isActive, string keyword)
        {
            lvResidents.BeginUpdate();
            lvResidents.Items.Clear();
            homeownerToResidentIdMap.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"
                    WITH PaymentSummary AS (
                        SELECT
                            ResidentID, UnitID, SUM(AmountPaid) AS TotalPaid, COUNT(DueId) AS PaidMonthsCount
                        FROM MonthlyDues
                        GROUP BY ResidentID, UnitID
                    )
                    SELECT
                        r.HomeownerID, r.ResidentID,
                        r.FirstName, r.MiddleName, r.LastName, r.HomeAddress, r.IsActive,
                        hu.UnitID,
                        u.UnitNumber, u.Block, u.UnitType,
                        ISNULL(ps.TotalPaid, 0) AS TotalPaid,
                        (DATEDIFF(month, r.DateRegistered, GETDATE()) + 1) - ISNULL(ps.PaidMonthsCount, 0) AS TotalMissed
                    FROM Residents r
                    JOIN HomeownerUnits hu ON r.ResidentID = hu.ResidentID AND hu.IsCurrent = 1
                    JOIN TBL_Units u ON hu.UnitID = u.UnitID
                    INNER JOIN PaymentSummary ps ON r.ResidentID = ps.ResidentID AND hu.UnitID = ps.UnitID
                    WHERE r.ResidencyType = 'Owner'
                      AND (r.IsActive = @isActive OR @isActive IS NULL)
                      AND (@keyword IS NULL OR (r.FirstName LIKE @keyword OR r.LastName LIKE @keyword OR r.HomeownerID LIKE @keyword))
                    ORDER BY r.LastName, r.FirstName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isActive", (object)isActive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : $"%{keyword}%");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int homeownerId = Convert.ToInt32(reader["HomeownerID"]);
                            int residentId = Convert.ToInt32(reader["ResidentID"]);
                            int unitId = Convert.ToInt32(reader["UnitID"]);
                            string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                            string homeAddress = reader["HomeAddress"].ToString();
                            bool status = Convert.ToBoolean(reader["IsActive"]);

                            string unitNumber = reader["UnitNumber"].ToString();
                            string block = reader["Block"].ToString();
                            string unitType = reader["UnitType"].ToString();

                            decimal totalPaid = Convert.ToDecimal(reader["TotalPaid"]);
                            int totalMissed = Convert.ToInt32(reader["TotalMissed"]);

                            homeownerToResidentIdMap[homeownerId] = residentId;

                            string formattedAddress = $"Unit {unitNumber} Block {block}, {homeAddress}";

                            ListViewItem item = new ListViewItem(homeownerId.ToString());
                            item.SubItems.Add(fullName);
                            item.SubItems.Add(formattedAddress);
                            item.SubItems.Add(totalPaid.ToString("F2"));
                            item.SubItems.Add(Math.Max(0, totalMissed).ToString());
                            item.SubItems.Add(unitType);
                            item.SubItems.Add(status ? "Active" : "Inactive");

                            item.Tag = (unitId, unitNumber);

                            lvResidents.Items.Add(item);
                        }
                    }
                }
            }
            lvResidents.EndUpdate();
        }

        private void LvResidents_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvResidents.SelectedItems.Count == 0) return;

            var selected = lvResidents.SelectedItems[0];
            int homeownerId = int.Parse(selected.SubItems[0].Text);
            var unitInfo = ((int unitId, string unitNumber))selected.Tag;

            lastSelectedHomeownerId = homeownerId;
            lastSelectedUnitId = unitInfo.unitId;

            if (homeownerToResidentIdMap.TryGetValue(homeownerId, out int residentId))
            {
                LoadMonthlyDues(residentId, lastSelectedUnitId);
            }
        }

        private void LoadMonthlyDues(int residentId, int unitId)
        {
            lvMonths.Items.Clear();
            var payments = new List<(string Month, DateTime PmtDate, decimal Amt, string PayerType, string PayerName)>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string paymentQuery = @"SELECT MonthCovered, PaymentDate, AmountPaid, PaidByResidencyType, PaidByResidentName 
                                        FROM MonthlyDues 
                                        WHERE ResidentID=@residentId AND UnitID=@unitId 
                                        ORDER BY CONVERT(DATETIME, '01 ' + MonthCovered)";
                using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@residentId", residentId);
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add((
                                reader["MonthCovered"].ToString(),
                                Convert.ToDateTime(reader["PaymentDate"]),
                                Convert.ToDecimal(reader["AmountPaid"]),
                                reader["PaidByResidencyType"] as string,
                                reader["PaidByResidentName"] as string
                            ));
                        }
                    }
                }
            }

            bool isActive = true;
            DateTime? inactiveDate = null;
            DateTime dateRegistered = DateTime.Now;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string residentInfoQuery = "SELECT IsActive, InactiveDate, DateRegistered FROM Residents WHERE ResidentID=@residentId";
                using (SqlCommand cmd = new SqlCommand(residentInfoQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@residentId", residentId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            dateRegistered = Convert.ToDateTime(reader["DateRegistered"]);
                            if (reader["InactiveDate"] != DBNull.Value)
                                inactiveDate = Convert.ToDateTime(reader["InactiveDate"]);
                        }
                    }
                }
            }

            HashSet<string> paidMonths = new HashSet<string>();
            foreach (var p in payments)
            {
                string paidByDisplay = p.PayerType ?? "Owner";
                if (!string.IsNullOrWhiteSpace(p.PayerName))
                {
                    paidByDisplay += $" ({p.PayerName})";
                }

                var item = new ListViewItem(p.Month);
                item.SubItems.Add("Paid");
                item.SubItems.Add(paidByDisplay);
                item.SubItems.Add(p.PmtDate.ToString("MMMM dd, yyyy"));
                item.BackColor = Color.FromArgb(220, 255, 220);
                lvMonths.Items.Add(item);
                paidMonths.Add(p.Month);
            }

            DateTime now = DateTime.Now;
            DateTime startDate = dateRegistered;
            DateTime endDate = isActive ? now : (inactiveDate ?? now);

            for (DateTime monthIterator = startDate; monthIterator <= endDate; monthIterator = monthIterator.AddMonths(1))
            {
                string monthName = monthIterator.ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName))
                {
                    var missedItem = new ListViewItem(monthName);
                    missedItem.SubItems.Add("Missed");
                    missedItem.SubItems.Add("");
                    missedItem.SubItems.Add("");
                    missedItem.BackColor = Color.FromArgb(255, 220, 220);
                    lvMonths.Items.Add(missedItem);
                }
            }

            lvMonths.BringToFront();
            lvMonths.Visible = true;
        }

        private void monthdues_Load(object sender, EventArgs e)
        {
            txtSearch.TextChanged += txtSearch_TextChanged;
            cmbResidentFilter.Items.Add("All Residents");
            cmbResidentFilter.Items.Add("Active Residents");
            cmbResidentFilter.Items.Add("Inactive Residents");
            cmbResidentFilter.SelectedIndex = 1;
            UpdateToggleSelectButton();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadResidentsList();
        }

        public void cmbResidentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadResidentsList();
        }

        private ReportData GenerateReportData(int residentId, int unitId)
        {
            string fullName = "", baseHomeAddress = "", contact = "", homeownerId = "";
            var accountDetails = new List<AccountDetail>();
            decimal totalCreditFromDb = 0m;
            var monthPaid = new Dictionary<DateTime, decimal>();

            // --- FIX: The formattedAddress variable is now declared here, outside the 'using' block ---
            string formattedAddress = "";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmdResident = new SqlCommand("SELECT HomeownerID, FirstName, MiddleName, LastName, HomeAddress, ContactNumber FROM Residents WHERE ResidentID=@residentId", conn))
                {
                    cmdResident.Parameters.AddWithValue("@residentId", residentId);
                    using (SqlDataReader reader = cmdResident.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            homeownerId = reader["HomeownerID"].ToString();
                            fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                            baseHomeAddress = reader["HomeAddress"].ToString();
                            contact = reader["ContactNumber"].ToString();
                        }
                    }
                }

                string unitNumber = "";
                string block = "";
                using (SqlCommand cmdUnit = new SqlCommand("SELECT UnitNumber, Block FROM TBL_Units WHERE UnitID = @unitId", conn))
                {
                    cmdUnit.Parameters.AddWithValue("@unitId", unitId);
                    using (SqlDataReader reader = cmdUnit.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            unitNumber = reader["UnitNumber"].ToString();
                            block = reader["Block"].ToString();
                        }
                    }
                }

                // --- FIX: The value is now assigned to the variable that exists outside the block ---
                formattedAddress = $"Unit {unitNumber} Block {block}, {baseHomeAddress}";

                string paymentQuery = @"SELECT MonthCovered, AmountPaid 
                                FROM MonthlyDues 
                                WHERE ResidentID=@residentId AND UnitID = @unitId
                                ORDER BY CONVERT(DATETIME, '01 ' + MonthCovered)";
                using (SqlCommand cmdPayments = new SqlCommand(paymentQuery, conn))
                {
                    cmdPayments.Parameters.AddWithValue("@residentId", residentId);
                    cmdPayments.Parameters.AddWithValue("@unitId", unitId);
                    using (SqlDataReader reader = cmdPayments.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (DateTime.TryParseExact(reader["MonthCovered"].ToString(), "MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime monthDate))
                            {
                                DateTime monthKey = new DateTime(monthDate.Year, monthDate.Month, 1);
                                decimal amount = Convert.ToDecimal(reader["AmountPaid"]);
                                totalCreditFromDb += amount;
                                if (monthPaid.ContainsKey(monthKey)) { monthPaid[monthKey] += amount; }
                                else { monthPaid[monthKey] = amount; }
                            }
                        }
                    }
                }
            }

            const decimal monthlyDue = 100m;
            decimal leftoverCredit = 0m, runningBalance = 0m, totalDebit = 0m;
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime earliestMonth = monthPaid.Keys.Count > 0 ? monthPaid.Keys.Min() : currentMonth;
            DateTime lastMonth = monthPaid.Keys.Count > 0 ? monthPaid.Keys.Max() : currentMonth;
            if (lastMonth < currentMonth) lastMonth = currentMonth;
            for (DateTime iter = earliestMonth; iter <= lastMonth; iter = iter.AddMonths(1))
            {
                decimal paidForThisMonth = monthPaid.ContainsKey(iter) ? monthPaid[iter] : 0m;
                decimal creditToApply = paidForThisMonth + leftoverCredit;
                string status;
                decimal balanceChange;
                if (creditToApply >= monthlyDue) { status = "Paid"; leftoverCredit = creditToApply - monthlyDue; balanceChange = monthlyDue - creditToApply; }
                else { status = "Missed"; leftoverCredit = 0; balanceChange = monthlyDue - creditToApply; }
                if (iter > currentMonth && creditToApply > 0) { status = "Advanced Paid"; }
                runningBalance += balanceChange;
                totalDebit += monthlyDue;
                accountDetails.Add(new AccountDetail
                {
                    Month = iter.ToString("MMMM yyyy"),
                    Description = status,
                    Debit = monthlyDue,
                    Credit = creditToApply > monthlyDue ? monthlyDue : creditToApply,
                    Balance = runningBalance
                });
            }

            var last6Months = accountDetails.Skip(Math.Max(0, accountDetails.Count - 6)).ToList();
            var olderMonths = accountDetails.Take(accountDetails.Count - last6Months.Count).ToList();
            var finalRows = new List<AccountDetail>();
            if (olderMonths.Count > 0) { finalRows.Add(new AccountDetail { Month = "Summary", Description = $"Previous {olderMonths.Count} month(s)", Debit = olderMonths.Sum(x => x.Debit), Credit = olderMonths.Sum(x => x.Credit), Balance = olderMonths.LastOrDefault()?.Balance ?? 0 }); }
            finalRows.AddRange(last6Months);
            string balanceMessage = runningBalance > 0 ? "Please make payment of the remaining balance within this month. Thank you!" : "There's no remaining balance. All dues are cleared.";

            return new ReportData { FullName = fullName, Address = formattedAddress, Contact = contact, HomeownerId = homeownerId, AccountDetails = finalRows, TotalDebit = totalDebit, TotalCredit = totalCreditFromDb, RunningBalance = runningBalance, BalanceMessage = balanceMessage };
        }
        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (lvResidents.CheckedItems.Count > 1)
            {
                int count = lvResidents.CheckedItems.Count;
                string message = $"You are about to save statements for {count} owners as individual PDF files. Continue?";
                if (MessageBox.Show(message, "Confirm Bulk Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BulkSaveStatementsAsPDF(lvResidents.CheckedItems);
                }
            }
            else if (lvResidents.SelectedItems.Count == 1)
            {
                int homeownerId = Convert.ToInt32(lvResidents.SelectedItems[0].SubItems[0].Text);
                var unitInfo = ((int unitId, string unitNumber))lvResidents.SelectedItems[0].Tag;
                if (homeownerToResidentIdMap.TryGetValue(homeownerId, out int residentId))
                {
                    ShowSingleReport(residentId, unitInfo.unitId);
                }
            }
            else
            {
                MessageBox.Show("Please select one owner to view their statement, or check the boxes for multiple owners to save their statements in bulk.", "Action Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BulkSaveStatementsAsPDF(ListView.CheckedListViewItemCollection itemsToSave)
        {
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Please select a location where the report folder will be created.";
                    folderDialog.ShowNewFolderButton = true;
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = folderDialog.SelectedPath;
                        string baseFolderName = "SOAReport_Residents";
                        string finalFolderPath = Path.Combine(selectedPath, baseFolderName);
                        int counter = 2;
                        while (Directory.Exists(finalFolderPath))
                        {
                            finalFolderPath = Path.Combine(selectedPath, $"{baseFolderName} ({counter})");
                            counter++;
                        }
                        Directory.CreateDirectory(finalFolderPath);
                        int savedCount = 0;
                        foreach (ListViewItem item in itemsToSave)
                        {
                            int homeownerId = Convert.ToInt32(item.SubItems[0].Text);
                            var unitInfo = ((int unitId, string unitNumber))item.Tag;

                            if (homeownerToResidentIdMap.TryGetValue(homeownerId, out int residentId))
                            {
                                ReportData data = GenerateReportData(residentId, unitInfo.unitId);
                                LocalReport report = new LocalReport();
                                report.ReportEmbeddedResource = "RECOMANAGESYS.SOAReport.rdlc";
                                report.DataSources.Add(new ReportDataSource("AccountDetails", data.AccountDetails));
                                ReportParameter[] parameters = {
                                    new ReportParameter("txtResident", data.FullName), new ReportParameter("txtAddress", data.Address), new ReportParameter("txtContact", data.Contact),
                                    new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy")), new ReportParameter("txtHomeownerId", data.HomeownerId),
                                    new ReportParameter("txtDebit", data.TotalDebit.ToString("F2")), new ReportParameter("txtCredits", data.TotalCredit.ToString("F2")),
                                    new ReportParameter("txtRemaining", data.RunningBalance.ToString("F2")), new ReportParameter("totalBalance", data.RunningBalance.ToString("F2")),
                                    new ReportParameter("txtBal", data.RunningBalance.ToString("F2")), new ReportParameter("txtMessage", data.BalanceMessage)
                                };
                                report.SetParameters(parameters);
                                byte[] pdfBytes = report.Render("PDF");
                                string safeName = string.Join("_", data.FullName.Split(Path.GetInvalidFileNameChars()));
                                string fileName = Path.Combine(finalFolderPath, $"SOAReport_{safeName}_Unit_{unitInfo.unitNumber}.pdf");
                                File.WriteAllBytes(fileName, pdfBytes);
                                savedCount++;
                            }
                        }
                        MessageBox.Show($"{savedCount} statement(s) have been saved to the folder:\n\n{finalFolderPath}", "Bulk Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during the bulk save operation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSingleReport(int residentId, int unitId)
        {
            ReportData data = GenerateReportData(residentId, unitId);
            this.currentResidentFullName = data.FullName;
            using (Form reportForm = new Form())
            {
                reportForm.WindowState = FormWindowState.Maximized;
                var reportViewer = new Microsoft.Reporting.WinForms.ReportViewer { Dock = DockStyle.Fill };
                this.reportViewerForSaving = reportViewer;
                ToolStrip toolStrip = new ToolStrip { Dock = DockStyle.Top };
                ToolStripDropDownButton btnSave = new ToolStripDropDownButton("Save Report") { ToolTipText = "Save the current report", DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
                ToolStripMenuItem pdfItem = new ToolStripMenuItem("Save as PDF") { Tag = "PDF" };
                ToolStripMenuItem wordItem = new ToolStripMenuItem("Save as Word (.docx)") { Tag = "WORDOPENXML" };
                ToolStripMenuItem excelItem = new ToolStripMenuItem("Save as Excel (.xlsx)") { Tag = "EXCELOPENXML" };
                btnSave.DropDownItems.AddRange(new ToolStripItem[] { pdfItem, wordItem, excelItem });
                pdfItem.Click += SaveReportFormat_Click;
                wordItem.Click += SaveReportFormat_Click;
                excelItem.Click += SaveReportFormat_Click;
                toolStrip.Items.Add(btnSave);
                reportForm.Controls.Add(reportViewer);
                reportForm.Controls.Add(toolStrip);
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportEmbeddedResource = "RECOMANAGESYS.SOAReport.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AccountDetails", data.AccountDetails));
                ReportParameter[] parameters = {
                    new ReportParameter("txtResident", data.FullName), new ReportParameter("txtAddress", data.Address),
                    new ReportParameter("txtContact", data.Contact),
                    new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy")),
                    new ReportParameter("txtHomeownerId", data.HomeownerId),
                    new ReportParameter("txtDebit", data.TotalDebit.ToString("F2")),
                    new ReportParameter("txtCredits", data.TotalCredit.ToString("F2")),
                    new ReportParameter("txtRemaining", data.RunningBalance.ToString("F2")),
                    new ReportParameter("totalBalance", data.RunningBalance.ToString("F2")),
                    new ReportParameter("txtBal", data.RunningBalance.ToString("F2")),
                    new ReportParameter("txtMessage", data.BalanceMessage)
                };
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.RefreshReport();
                reportForm.ShowDialog();
            }
        }

        private void UpdateToggleSelectButton()
        {
            if (btnToggleSelect != null)
            {
                if (lvResidents.Items.Count > 0 && lvResidents.CheckedItems.Count == lvResidents.Items.Count)
                {
                    btnToggleSelect.Text = "Deselect All";
                }
                else
                {
                    btnToggleSelect.Text = "Select All";
                }
            }
        }
        private void lvResidents_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateToggleSelectButton();
        }
        private void btnToggleSelect_Click(object sender, EventArgs e)
        {
            bool allChecked = (lvResidents.Items.Count > 0 && lvResidents.CheckedItems.Count == lvResidents.Items.Count);
            bool checkState = !allChecked;
            foreach (ListViewItem item in lvResidents.Items)
            {
                item.Checked = checkState;
            }
            UpdateToggleSelectButton();
        }
        private void SaveReportFormat_Click(object sender, EventArgs e)
        {
            if (this.reportViewerForSaving == null || string.IsNullOrEmpty(this.currentResidentFullName)) return;
            ToolStripItem clickedItem = sender as ToolStripItem;
            if (clickedItem?.Tag == null) return;
            string format = clickedItem.Tag.ToString();
            string extension = "", filter = "";
            switch (format)
            {
                case "WORDOPENXML": extension = "docx"; filter = "Word Document (*.docx)|*.docx"; break;
                case "EXCELOPENXML": extension = "xlsx"; filter = "Excel Workbook (*.xlsx)|*.xlsx"; break;
                default: extension = "pdf"; filter = "PDF file (*.pdf)|*.pdf"; break;
            }
            byte[] reportBytes = this.reportViewerForSaving.LocalReport.Render(format);
            string safeResidentName = string.Join("_", this.currentResidentFullName.Split(Path.GetInvalidFileNameChars()));
            SaveFileDialog saveDialog = new SaveFileDialog { Filter = filter, Title = "Save Report", FileName = $"SOAReport_{safeResidentName}.{extension}" };
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try { File.WriteAllBytes(saveDialog.FileName, reportBytes); MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                catch (Exception ex) { MessageBox.Show("Error saving report: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        public ComboBox ResidentFilterComboBox => cmbResidentFilter;
    }
}