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
           // lvResidents.Columns.Add("Total Missed", 100);
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
                LoadResidentsList();
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
            if (currentFilter == "Active Units") isActive = true;
            else if (currentFilter == "Inactive Units") isActive = false;

            LoadResidents(isActive, txtSearch.Text.Trim());

            if (selectedHomeownerId != -1)
            {
                foreach (ListViewItem item in lvResidents.Items)
                {
                    int currentItemId;
                    if (int.TryParse(item.SubItems[0].Text, out currentItemId))
                    {
                        if (currentItemId == selectedHomeownerId)
                        {
                            item.Selected = true;
                            item.Focused = true;
                            lvResidents.EnsureVisible(item.Index);
                            break;
                        }
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
        WITH UnitPaymentSummary AS (
            SELECT
                UnitID,
                SUM(AmountPaid) AS TotalPaid,
                MIN(CONVERT(DATETIME, '01 ' + MonthCovered)) AS FirstPaymentDate,
                COUNT(DISTINCT MonthCovered) AS PaidMonthsCount 
            FROM MonthlyDues
            GROUP BY UnitID
        )
        SELECT
            u.UnitID, u.UnitNumber, u.Block, u.UnitType,
            r_owner.HomeAddress,
            ISNULL(ups.TotalPaid, 0) AS TotalPaid,
            CASE
                WHEN COALESCE(hu_owner.DateOfOwnership, ups.FirstPaymentDate) IS NOT NULL
                THEN (DATEDIFF(month, COALESCE(hu_owner.DateOfOwnership, ups.FirstPaymentDate), GETDATE()) + 1) - ISNULL(ups.PaidMonthsCount, 0)
                ELSE 0
            END AS TotalMissed,
            r_owner.ResidentID,
            r_owner.HomeownerID,
            COALESCE(r_owner.FirstName + ' ' + r_owner.MiddleName + ' ' + r_owner.LastName, 'Currently no owner') AS FullName,
            CASE 
                WHEN r_owner.ResidentID IS NOT NULL AND r_owner.IsActive = 1 THEN 1 
                ELSE 0 
            END AS EffectiveIsActive
        FROM TBL_Units u
        LEFT JOIN UnitPaymentSummary ups ON u.UnitID = ups.UnitID
        LEFT JOIN HomeownerUnits hu_owner ON u.UnitID = hu_owner.UnitID AND hu_owner.IsCurrent = 1
        LEFT JOIN Residents r_owner ON hu_owner.ResidentID = r_owner.ResidentID AND r_owner.ResidencyType = 'Owner'
        WHERE
            (
                (@isActive IS NULL AND (
                    (r_owner.ResidentID IS NOT NULL AND r_owner.IsActive = 1)
                    OR 
                    (NOT EXISTS (
                        SELECT 1 FROM HomeownerUnits hu_check JOIN Residents r_check ON hu_check.ResidentID = r_check.ResidentID
                        WHERE hu_check.UnitID = u.UnitID AND hu_check.IsCurrent = 1 AND r_check.IsActive = 1
                    ))
                )) OR
                (@isActive = 1 AND r_owner.ResidentID IS NOT NULL AND r_owner.IsActive = 1) OR
                (@isActive = 0 AND NOT EXISTS (
                    SELECT 1 FROM HomeownerUnits hu_check JOIN Residents r_check ON hu_check.ResidentID = r_check.ResidentID
                    WHERE hu_check.UnitID = u.UnitID AND hu_check.IsCurrent = 1 AND r_check.IsActive = 1
                ))
            )
            AND (@keyword IS NULL OR (r_owner.FirstName LIKE @keyword OR r_owner.LastName LIKE @keyword OR r_owner.HomeownerID LIKE @keyword OR u.UnitNumber LIKE @keyword OR u.Block LIKE @keyword))
        ORDER BY u.Block, u.UnitNumber";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isActive", (object)isActive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : $"%{keyword}%");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int homeownerId = reader["HomeownerID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["HomeownerID"]);
                            int residentId = reader["ResidentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ResidentID"]);
                            int unitId = Convert.ToInt32(reader["UnitID"]);
                            string fullName = reader["FullName"].ToString();
                            bool effectiveStatus = Convert.ToBoolean(reader["EffectiveIsActive"]);
                            string unitNumber = reader["UnitNumber"].ToString();
                            string block = reader["Block"].ToString();
                            string unitType = reader["UnitType"].ToString();
                            decimal totalPaid = Convert.ToDecimal(reader["TotalPaid"]);
                            int totalMissed = Convert.ToInt32(reader["TotalMissed"]);

                            string residentHomeAddress = reader["HomeAddress"] == DBNull.Value ? "" : reader["HomeAddress"].ToString();
                            string formattedAddress = string.IsNullOrWhiteSpace(residentHomeAddress)
                                ? $"Unit {unitNumber} Block {block}"
                                : $"Unit {unitNumber} Block {block}, {residentHomeAddress}";

                            homeownerToResidentIdMap[homeownerId] = residentId;

                            ListViewItem item = new ListViewItem(homeownerId == 0 ? "N/A" : homeownerId.ToString()); 
                            item.SubItems.Add(fullName);                                                            
                            item.SubItems.Add(formattedAddress);                                                 
                            item.SubItems.Add(totalPaid.ToString("F2"));                                          
                            //item.SubItems.Add(Math.Max(0, totalMissed) > 0 ? Math.Max(0, totalMissed).ToString() : "");
                            item.SubItems.Add(unitType);                                                          
                            item.SubItems.Add(effectiveStatus ? "Active" : "Inactive");                           

                            item.Tag = new { UnitId = unitId, UnitNumber = unitNumber, ResidentId = residentId };

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
            var tagData = (dynamic)selected.Tag;

            int residentId = tagData.ResidentId;
            int unitId = tagData.UnitId;
            int.TryParse(selected.SubItems[0].Text, out int homeownerId);

            lastSelectedHomeownerId = homeownerId;
            lastSelectedUnitId = unitId;
            LoadMonthlyDues(residentId, unitId);
        }

        private void LoadMonthlyDues(int residentId, int unitId)
        {
            lvMonths.Items.Clear();
            var payments = new List<(string Month, DateTime PmtDate, decimal Amt, string PayerType, string PayerName)>();
            DateTime startDate = DateTime.Now;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string paymentQuery;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (residentId > 0)
                {
                    string ownershipQuery = @"
                SELECT hu.DateOfOwnership, r.IsActive, r.InactiveDate 
                FROM HomeownerUnits hu
                JOIN Residents r ON hu.ResidentID = r.ResidentID
                WHERE hu.ResidentID = @residentId AND hu.UnitID = @unitId AND hu.IsCurrent = 1";

                    using (SqlCommand ownershipCmd = new SqlCommand(ownershipQuery, conn))
                    {
                        ownershipCmd.Parameters.AddWithValue("@residentId", residentId);
                        ownershipCmd.Parameters.AddWithValue("@unitId", unitId);
                        using (SqlDataReader reader = ownershipCmd.ExecuteReader())
                        {
                            if (reader.Read() && reader["DateOfOwnership"] != DBNull.Value)
                            {
                                startDate = Convert.ToDateTime(reader["DateOfOwnership"]);
                            }
                        }
                    }

                    paymentQuery = @"SELECT MonthCovered, PaymentDate, AmountPaid, PaidByResidencyType, PaidByResidentName 
                             FROM MonthlyDues 
                             WHERE ResidentID=@residentId AND UnitID=@unitId 
                             ORDER BY CONVERT(DATETIME, '01 ' + MonthCovered)";
                    cmd.Parameters.AddWithValue("@residentId", residentId);
                }
                else
                {
                    string firstPaymentQuery = "SELECT MIN(CONVERT(DATETIME, '01 ' + MonthCovered)) FROM MonthlyDues WHERE UnitID = @unitId";
                    using (SqlCommand firstPayCmd = new SqlCommand(firstPaymentQuery, conn))
                    {
                        firstPayCmd.Parameters.AddWithValue("@unitId", unitId);
                        var result = firstPayCmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            startDate = Convert.ToDateTime(result);
                        }
                    }

                    paymentQuery = @"SELECT MonthCovered, PaymentDate, AmountPaid, PaidByResidencyType, PaidByResidentName 
                             FROM MonthlyDues 
                             WHERE UnitID=@unitId 
                             ORDER BY CONVERT(DATETIME, '01 ' + MonthCovered)";
                }

                cmd.CommandText = paymentQuery;
                cmd.Parameters.AddWithValue("@unitId", unitId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add((
                            reader["MonthCovered"].ToString(),
                            Convert.ToDateTime(reader["PaymentDate"]),
                            Convert.ToDecimal(reader["AmountPaid"]),
                            reader["PaidByResidencyType"] as string ?? "N/A",
                            reader["PaidByResidentName"] as string ?? "N/A"
                        ));
                    }
                }
            }

            HashSet<string> paidMonths = new HashSet<string>();
            foreach (var p in payments)
            {
                string paidByDisplay = p.PayerType;
                if (!string.IsNullOrWhiteSpace(p.PayerName) && p.PayerName != "N/A")
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

            DateTime endDate = DateTime.Now;

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
            cmbResidentFilter.Items.Add("All Units");
            cmbResidentFilter.Items.Add("Active Units");
            cmbResidentFilter.Items.Add("Inactive Units");
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
                string message = $"You are about to save statements for {count} owner(s) as individual PDF files. Continue?";
                if (MessageBox.Show(message, "Confirm Bulk Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BulkSaveStatementsAsPDF(lvResidents.CheckedItems);
                }
            }
            else if (lvResidents.SelectedItems.Count == 1)
            {
                var selectedItem = lvResidents.SelectedItems[0];
                if (int.TryParse(selectedItem.SubItems[0].Text, out int homeownerId))
                {
                    var tagData = (dynamic)selectedItem.Tag; 
                    int unitId = tagData.UnitId;             

                    if (homeownerToResidentIdMap.TryGetValue(homeownerId, out int residentId))
                    {
                        ShowSingleReport(residentId, unitId);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select one owner to view their statement, or check the boxes for one or more owners to save their statements in bulk.", "Action Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            if (int.TryParse(item.SubItems[0].Text, out int homeownerId))
                            {
                                var tagData = (dynamic)item.Tag; 
                                int unitId = tagData.UnitId;   
                                string unitNumber = tagData.UnitNumber;

                                if (homeownerToResidentIdMap.TryGetValue(homeownerId, out int residentId))
                                {
                                    ReportData data = GenerateReportData(residentId, unitId);
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
                                    string fileName = Path.Combine(finalFolderPath, $"SOAReport_{safeName}_Unit_{unitNumber}.pdf");
                                    File.WriteAllBytes(fileName, pdfBytes);
                                    savedCount++;
                                }
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