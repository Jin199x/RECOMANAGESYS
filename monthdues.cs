using Microsoft.Reporting.WinForms;
using System;
using System.Collections;
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
        private int lastSelectedResidentId = -1;
        private int lastSelectedUnitId = -1;
        private string currentResidentFullName = "";
        private Microsoft.Reporting.WinForms.ReportViewer reportViewerForSaving;

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
            public string ResidentId { get; set; }
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
            lvResidents.Columns.Clear();
            lvResidents.Columns.Add("ResidentID", 80);
            lvResidents.Columns.Add("Full Name", 200);
            lvResidents.Columns.Add("Address", 250);
            lvResidents.Columns.Add("Total Paid", 100);
            lvResidents.Columns.Add("Total Missed", 100);
            lvResidents.Columns.Add("Status", 100);
            lvMonths.View = View.Details;
            lvMonths.FullRowSelect = true;
            lvMonths.Columns.Clear();
            lvMonths.Columns.Add("Month", 150);
            lvMonths.Columns.Add("Status", 100);
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //no code inside
        }

        private void addvisitor_Click(object sender, EventArgs e)
        {
            frmPayment payform = new frmPayment();
            payform.ShowDialog();
            LoadResidentsList();
            if (lastSelectedResidentId != -1)
                LoadMonthlyDues(lastSelectedResidentId, lastSelectedUnitId);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateMonthlyDues updateDues = new UpdateMonthlyDues();
            updateDues.OnPaymentSaved = () =>
            {
                if (lastSelectedResidentId != -1)
                    LoadMonthlyDues(lastSelectedResidentId, lastSelectedUnitId);
            };
            updateDues.ShowDialog();
        }

        public void LoadResidentsList()
        {
            string currentFilter = cmbResidentFilter.SelectedItem?.ToString();
            int selectedResidentId = lastSelectedResidentId;
            bool? isActive = null;
            if (currentFilter == "Active Residents") isActive = true;
            else if (currentFilter == "Inactive Residents") isActive = false;
            LoadResidents(isActive);
            if (selectedResidentId != -1)
            {
                foreach (ListViewItem item in lvResidents.Items)
                {
                    if (int.Parse(item.SubItems[0].Text) == selectedResidentId)
                    {
                        item.Selected = true;
                        item.Focused = true;
                        lvResidents.EnsureVisible(item.Index);
                        break;
                    }
                }
            }
        }

        private void LoadResidents(bool? isActive)
        {
            lvResidents.Items.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT r.HomeownerID, r.FirstName, r.MiddleName, r.LastName, r.HomeAddress, r.IsActive FROM Residents r WHERE EXISTS (SELECT 1 FROM MonthlyDues md WHERE md.HomeownerId = r.HomeownerID)";
                if (isActive.HasValue) query += " AND r.IsActive = @isActive";
                query += " ORDER BY r.HomeownerID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (isActive.HasValue) cmd.Parameters.AddWithValue("@isActive", isActive.Value);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int residentId = Convert.ToInt32(reader["HomeownerID"]);
                            string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                            string address = reader["HomeAddress"].ToString();
                            bool status = Convert.ToBoolean(reader["IsActive"]);
                            List<int> unitIds = new List<int>();
                            using (SqlConnection connUnits = DatabaseHelper.GetConnection())
                            using (SqlCommand cmdUnits = new SqlCommand("SELECT DISTINCT UnitID FROM MonthlyDues WHERE HomeownerId=@residentId", connUnits))
                            {
                                cmdUnits.Parameters.AddWithValue("@residentId", residentId);
                                connUnits.Open();
                                using (SqlDataReader unitReader = cmdUnits.ExecuteReader())
                                {
                                    while (unitReader.Read()) unitIds.Add(Convert.ToInt32(unitReader["UnitID"]));
                                }
                            }
                            foreach (int unitId in unitIds)
                            {
                                var (totalPaid, totalMissed) = GetPaymentSummary(residentId, unitId);
                                ListViewItem item = new ListViewItem(residentId.ToString());
                                item.SubItems.Add(fullName);
                                item.SubItems.Add(address);
                                item.SubItems.Add(totalPaid.ToString("F2"));
                                item.SubItems.Add(totalMissed.ToString());
                                item.SubItems.Add(status ? "Active" : "Inactive");
                                lvResidents.Items.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private bool HasPayments(int residentId, int unitId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM MonthlyDues WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private (decimal totalPaid, int totalMissed) GetPaymentSummary(int residentId, int unitId)
        {
            decimal totalPaid = 0;
            var paidMonths = new HashSet<string>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT MonthCovered, AmountPaid FROM MonthlyDues WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalPaid += Convert.ToDecimal(reader["AmountPaid"]);
                        paidMonths.Add(reader["MonthCovered"].ToString());
                    }
                }
            }
            int missed = 0;
            DateTime now = DateTime.Now;
            for (int m = 1; m <= now.Month; m++)
            {
                string monthName = new DateTime(now.Year, m, 1).ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName)) missed++;
            }
            return (totalPaid, missed);
        }

        private void LvResidents_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvResidents.SelectedItems.Count == 0) return;
            var selected = lvResidents.SelectedItems[0];
            lastSelectedResidentId = int.Parse(selected.SubItems[0].Text);
            lastSelectedUnitId = GetUnitIdForResident(lastSelectedResidentId);
            LoadMonthlyDues(lastSelectedResidentId, lastSelectedUnitId);
        }

        private int GetUnitIdForResident(int residentId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 UnitID FROM HomeownerUnits WHERE HomeownerID=@residentId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void LoadMonthlyDues(int residentId, int unitId)
        {
            lvMonths.Items.Clear();
            List<(string Month, DateTime PaymentDate, decimal Amount)> payments = new List<(string, DateTime, decimal)>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT MonthCovered, PaymentDate, AmountPaid FROM MonthlyDues WHERE HomeownerId=@residentId AND UnitID=@unitId ORDER BY MonthCovered ASC", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add((reader["MonthCovered"].ToString(), Convert.ToDateTime(reader["PaymentDate"]), Convert.ToDecimal(reader["AmountPaid"])));
                    }
                }
            }
            bool isActive = true;
            DateTime? inactiveDate = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT IsActive, InactiveDate FROM Residents WHERE HomeownerID=@residentId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isActive = Convert.ToBoolean(reader["IsActive"]);
                        if (reader["InactiveDate"] != DBNull.Value)
                            inactiveDate = Convert.ToDateTime(reader["InactiveDate"]);
                    }
                }
            }
            HashSet<string> paidMonths = new HashSet<string>();
            foreach (var p in payments)
            {
                ListViewItem item = new ListViewItem(p.Month) { SubItems = { "Paid", p.PaymentDate.ToString("MMMM dd, yyyy") }, BackColor = Color.FromArgb(220, 255, 220) };
                lvMonths.Items.Add(item);
                paidMonths.Add(p.Month);
            }
            DateTime now = DateTime.Now;
            DateTime endMonth = isActive ? now : (inactiveDate ?? now);
            for (int m = 1; m <= endMonth.Month; m++)
            {
                string monthName = new DateTime(endMonth.Year, m, 1).ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName))
                {
                    ListViewItem missedItem = new ListViewItem(monthName) { SubItems = { "Missed", "" }, BackColor = Color.FromArgb(255, 220, 220) };
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
            cmbResidentFilter.SelectedIndex = 0;
            UpdateToggleSelectButton();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            lvResidents.Items.Clear();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadResidentsList();
                return;
            }
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"SELECT r.HomeownerID, r.FirstName, r.MiddleName, r.LastName, r.HomeAddress, r.IsActive FROM Residents r WHERE r.FirstName LIKE @keyword OR r.LastName LIKE @keyword OR r.HomeownerID LIKE @keyword", conn))
            {
                cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int residentId = Convert.ToInt32(reader["HomeownerID"]);
                        string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                        string address = reader["HomeAddress"].ToString();
                        bool status = Convert.ToBoolean(reader["IsActive"]);
                        int unitId = GetUnitIdForResident(residentId);
                        if (HasPayments(residentId, unitId))
                        {
                            var (totalPaid, totalMissed) = GetPaymentSummary(residentId, unitId);
                            ListViewItem item = new ListViewItem(residentId.ToString());
                            item.SubItems.Add(fullName);
                            item.SubItems.Add(address);
                            item.SubItems.Add(totalPaid.ToString("F2"));
                            item.SubItems.Add(totalMissed.ToString());
                            item.SubItems.Add(status ? "Active" : "Inactive");
                            lvResidents.Items.Add(item);
                        }
                    }
                }
            }
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

        public void cmbResidentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbResidentFilter.SelectedItem.ToString())
            {
                case "Active Residents": LoadResidents(true); break;
                case "Inactive Residents": LoadResidents(false); break;
                case "All Residents": LoadResidents(null); break;
            }
        }
        public ComboBox ResidentFilterComboBox => cmbResidentFilter;

        private void GenerateMissingMonthlyDues() { /* Future use */ }

        private ReportData GenerateReportData(int ResidentId)
        {
            string fullName = "", address = "", contact = "";
            List<AccountDetail> accountDetails = new List<AccountDetail>();
            decimal totalDebit = 0m, runningBalance = 0m, totalCreditFromDb = 0m;
            Dictionary<DateTime, decimal> monthPaid = new Dictionary<DateTime, decimal>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmdResident = new SqlCommand("SELECT FirstName, MiddleName, LastName, HomeAddress, ContactNumber FROM Residents WHERE HomeownerID=@residentId", conn))
                {
                    cmdResident.Parameters.AddWithValue("@residentId", residentId);
                    using (SqlDataReader reader = cmdResident.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                            address = reader["HomeAddress"].ToString();
                            contact = reader["ContactNumber"].ToString();
                        }
                    }
                }
                using (SqlCommand cmdPayments = new SqlCommand("SELECT MonthCovered, AmountPaid FROM MonthlyDues WHERE HomeownerId=@residentId ORDER BY MonthCovered", conn))
                {
                    cmdPayments.Parameters.AddWithValue("@residentId", residentId);
                    using (SqlDataReader reader = cmdPayments.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (DateTime.TryParse(reader["MonthCovered"].ToString(), out DateTime monthDate))
                            {
                                DateTime monthKey = new DateTime(monthDate.Year, monthDate.Month, 1);
                                decimal amt = Convert.ToDecimal(reader["AmountPaid"]);
                                totalCreditFromDb += amt;
                                if (monthPaid.ContainsKey(monthKey)) monthPaid[monthKey] += amt; else monthPaid[monthKey] = amt;
                            }
                        }
                    }
                }
            }
            const decimal monthlyDue = 100m;
            decimal leftoverCredit = 0m;
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime earliestMonth = monthPaid.Keys.Count > 0 ? monthPaid.Keys.Min() : currentMonth;
            DateTime lastMonth = monthPaid.Keys.Count > 0 ? monthPaid.Keys.Max() : currentMonth;
            if (lastMonth < currentMonth) lastMonth = currentMonth;
            DateTime iter = earliestMonth;
            while (iter <= lastMonth)
            {
                decimal paidForThisMonth = monthPaid.ContainsKey(iter) ? monthPaid[iter] : 0m;
                decimal creditToApply = paidForThisMonth + leftoverCredit;
                string status;
                decimal balanceChange;
                if (creditToApply >= monthlyDue) { status = "Paid"; leftoverCredit = creditToApply - monthlyDue; balanceChange = monthlyDue - creditToApply; }
                else { status = "Missed"; leftoverCredit = 0; balanceChange = monthlyDue - creditToApply; }
                if (iter > currentMonth && creditToApply > 0) status = "Advanced Paid";
                runningBalance += balanceChange;
                totalDebit += monthlyDue;
                accountDetails.Add(new AccountDetail { Month = iter.ToString("MMMM yyyy"), Description = status, Debit = monthlyDue, Credit = creditToApply > monthlyDue ? monthlyDue : creditToApply, Balance = runningBalance });
                iter = iter.AddMonths(1);
            }
            var sortedMonths = accountDetails.OrderBy(x => DateTime.ParseExact(x.Month, "MMMM yyyy", CultureInfo.InvariantCulture)).ToList();
            var last6Months = sortedMonths.Skip(Math.Max(0, sortedMonths.Count - 6)).ToList();
            var olderMonths = sortedMonths.Take(sortedMonths.Count - last6Months.Count).ToList();
            var finalRows = new List<AccountDetail>();
            if (olderMonths.Count > 0)
            {
                finalRows.Add(new AccountDetail { Month = "Summary", Description = $"Previous {olderMonths.Count} month(s)", Debit = olderMonths.Sum(x => x.Debit), Credit = olderMonths.Sum(x => x.Credit), Balance = olderMonths.LastOrDefault()?.Balance ?? 0 });
            }
            finalRows.AddRange(last6Months);
            string balanceMessage = runningBalance > 0 ? "Please make payment of the remaining balance within this month. Thank you!" : "There's no remaining balance. All dues are cleared.";
            return new ReportData { FullName = fullName, Address = address, Contact = contact, ResidentId = residentId.ToString(), AccountDetails = finalRows, TotalDebit = totalDebit, TotalCredit = totalCreditFromDb, RunningBalance = runningBalance, BalanceMessage = balanceMessage };
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            // SCENARIO 1: More than one resident is CHECKED (Bulk Save Action)
            if (lvResidents.CheckedItems.Count > 1)
            {
                int count = lvResidents.CheckedItems.Count;
                string message = $"You are about to save statements for {count} residents as individual PDF files. Continue?";

                if (MessageBox.Show(message, "Confirm Bulk Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BulkSaveStatementsAsPDF(lvResidents.CheckedItems);
                }
            }
            // SCENARIO 2: Exactly ONE resident is HIGHLIGHTED (Single View Action)
            else if (lvResidents.SelectedItems.Count == 1)
            {
                int residentId = Convert.ToInt32(lvResidents.SelectedItems[0].SubItems[0].Text);
                ShowSingleReport(residentId);
            }
            // SCENARIO 3: No clear action
            else
            {
                MessageBox.Show("Please select one resident to view their statement, or check the boxes for multiple residents to save their statements in bulk.", "Action Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- GEMINI: This is the final, corrected version. Please replace your old method with this. ---
        private void BulkSaveStatementsAsPDF(ListView.CheckedListViewItemCollection itemsToSave)
        {
            try
            {
                // 1. Re-introduce the FolderBrowserDialog to let the user choose a location.
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Please select a location where the report folder will be created.";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 2. Get the location the user chose (e.g., "C:\Users\YourName\Documents")
                        string selectedPath = folderDialog.SelectedPath;
                        string baseFolderName = "SOAReport_Residents";

                        // 3. Combine the user's path with our base folder name.
                        string finalFolderPath = Path.Combine(selectedPath, baseFolderName);
                        int counter = 2;

                        // 4. Check if that folder already exists inside the chosen location.
                        // If it does, find the next available name like "(2)", "(3)", etc.
                        while (Directory.Exists(finalFolderPath))
                        {
                            finalFolderPath = Path.Combine(selectedPath, $"{baseFolderName} ({counter})");
                            counter++;
                        }

                        // 5. Create the new, unique directory in the user's chosen location.
                        Directory.CreateDirectory(finalFolderPath);

                        int savedCount = 0;
                        foreach (ListViewItem item in itemsToSave)
                        {
                            int residentId = Convert.ToInt32(item.SubItems[0].Text);
                            ReportData data = GenerateReportData(residentId);

                            LocalReport report = new LocalReport();
                            report.ReportEmbeddedResource = "RECOMANAGESYS.SOAReport.rdlc";
                            report.DataSources.Add(new ReportDataSource("AccountDetails", data.AccountDetails));

                            ReportParameter[] parameters = {
                        new ReportParameter("txtResident", data.FullName), new ReportParameter("txtAddress", data.Address), new ReportParameter("txtContact", data.Contact),
                        new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy")), new ReportParameter("txtResidentId", data.ResidentId),
                        new ReportParameter("txtDebit", data.TotalDebit.ToString("F2")), new ReportParameter("txtCredits", data.TotalCredit.ToString("F2")),
                        new ReportParameter("txtRemaining", data.RunningBalance.ToString("F2")), new ReportParameter("totalBalance", data.RunningBalance.ToString("F2")),
                        new ReportParameter("txtBal", data.RunningBalance.ToString("F2")), new ReportParameter("txtMessage", data.BalanceMessage)
                    };
                            report.SetParameters(parameters);

                            byte[] pdfBytes = report.Render("PDF");

                            string safeName = string.Join("_", data.FullName.Split(Path.GetInvalidFileNameChars()));
                            string fileName = Path.Combine(finalFolderPath, $"SOAReport_{safeName}.pdf");

                            File.WriteAllBytes(fileName, pdfBytes);
                            savedCount++;
                        }

                        // 6. Inform the user of the exact full path where the files were saved.
                        MessageBox.Show($"{savedCount} statement(s) have been saved to the folder:\n\n{finalFolderPath}", "Bulk Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during the bulk save operation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSingleReport(int residentId)
        {
            ReportData data = GenerateReportData(residentId);
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
                reportViewer.ShowExportButton = false;
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportEmbeddedResource = "RECOMANAGESYS.SOAReport.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AccountDetails", data.AccountDetails));
                ReportParameter[] parameters = {
            new ReportParameter("txtResident", data.FullName), new ReportParameter("txtAddress", data.Address), 
            new ReportParameter("txtContact", data.Contact),
            new ReportParameter("txtDate", DateTime.Now.ToString("MMMM dd, yyyy")), 
            new ReportParameter("txtResidentId", data.ResidentId),
            new ReportParameter("txtDebit", data.TotalDebit.ToString("F2")), 
            new ReportParameter("txtCredits", data.TotalCredit.ToString("F2")),
            new ReportParameter("txtRemaining", data.RunningBalance.ToString("F2")), 
            new ReportParameter("totalBalance", data.RunningBalance.ToString("F2")),
            new ReportParameter("txtBal", data.RunningBalance.ToString("F2")), 
            new ReportParameter("txtMessage", data.BalanceMessage) };
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
    }
}