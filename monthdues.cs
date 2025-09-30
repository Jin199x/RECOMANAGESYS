using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RECOMANAGESYS
{
    public partial class monthdues : UserControl
    {
        private int lastSelectedResidentId = -1;
        private int lastSelectedUnitId = -1;

        public monthdues()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Configure ListViews
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
            lvMonths.Columns.Add("Status", 100); // Only shows Paid/Missed
            lvMonths.Columns.Add("Payment Date", 150);

            LoadResidentsList();

            // Hook MouseClick event to handle any click on row/subitems
            lvResidents.MouseClick += LvResidents_MouseClick;
        }



        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // panel1 paint event restored, no code inside
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
            // Save current filter selection
            string currentFilter = cmbResidentFilter.SelectedItem?.ToString();

            // Save currently selected resident ID
            int selectedResidentId = lastSelectedResidentId;

            bool? isActive = null;
            if (currentFilter == "Active Residents") isActive = true;
            else if (currentFilter == "Inactive Residents") isActive = false;

            LoadResidents(isActive);

            // Restore previously selected resident in the ListView
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
                string query = @"SELECT r.HomeownerID, r.FirstName, r.MiddleName, r.LastName, r.HomeAddress, r.IsActive
                         FROM Residents r
                         WHERE EXISTS (SELECT 1 FROM MonthlyDues md WHERE md.HomeownerId = r.HomeownerID)";

                if (isActive.HasValue)
                    query += " AND r.IsActive = @isActive";

                query += " ORDER BY r.HomeownerID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (isActive.HasValue)
                        cmd.Parameters.AddWithValue("@isActive", isActive.Value);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int residentId = Convert.ToInt32(reader["HomeownerID"]);
                            string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                            string address = reader["HomeAddress"].ToString();
                            bool status = Convert.ToBoolean(reader["IsActive"]);

                            // Get all units with payments
                            List<int> unitIds = new List<int>();
                            using (SqlConnection connUnits = DatabaseHelper.GetConnection())
                            using (SqlCommand cmdUnits = new SqlCommand(
                                "SELECT DISTINCT UnitID FROM MonthlyDues WHERE HomeownerId=@residentId", connUnits))
                            {
                                cmdUnits.Parameters.AddWithValue("@residentId", residentId);
                                connUnits.Open();
                                using (SqlDataReader unitReader = cmdUnits.ExecuteReader())
                                {
                                    while (unitReader.Read())
                                        unitIds.Add(Convert.ToInt32(unitReader["UnitID"]));
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
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM MonthlyDues WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
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
            using (SqlCommand cmd = new SqlCommand(
                "SELECT MonthCovered, AmountPaid FROM MonthlyDues WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
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
                if (!paidMonths.Contains(monthName))
                    missed++;
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
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 UnitID FROM HomeownerUnits WHERE HomeownerID=@residentId", conn))
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

            // Get all payments first
            List<(string Month, DateTime PaymentDate, decimal Amount)> payments = new List<(string, DateTime, decimal)>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MonthCovered, PaymentDate, AmountPaid 
          FROM MonthlyDues
          WHERE HomeownerId=@residentId AND UnitID=@unitId
          ORDER BY MonthCovered ASC", conn))
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
                            Convert.ToDecimal(reader["AmountPaid"])
                        ));
                    }
                }
            }

            // Get resident status and inactive date
            bool isActive = true;
            DateTime? inactiveDate = null;
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT IsActive, InactiveDate FROM Residents WHERE HomeownerID=@residentId", conn))
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
                ListViewItem item = new ListViewItem(p.Month);
                item.SubItems.Add("Paid");
                item.SubItems.Add(p.PaymentDate.ToString("MMMM dd, yyyy"));
                item.BackColor = Color.FromArgb(220, 255, 220);
                lvMonths.Items.Add(item);
                paidMonths.Add(p.Month);
            }

            DateTime now = DateTime.Now;
            DateTime endMonth = isActive ? now : (inactiveDate ?? now); // Stop at inactive date
            for (int m = 1; m <= endMonth.Month; m++)
            {
                string monthName = new DateTime(endMonth.Year, m, 1).ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName))
                {
                    ListViewItem missedItem = new ListViewItem(monthName);
                    missedItem.SubItems.Add("Missed");
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
            cmbResidentFilter.SelectedIndex = 0;
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
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT r.HomeownerID, r.FirstName, r.MiddleName, r.LastName, r.HomeAddress
                  FROM Residents r
                  WHERE r.FirstName LIKE @keyword OR r.LastName LIKE @keyword OR r.HomeownerID LIKE @keyword", conn))
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

                        int unitId = GetUnitIdForResident(residentId);

                        if (HasPayments(residentId, unitId))
                        {
                            (decimal totalPaid, int totalMissed) = GetPaymentSummary(residentId, unitId);

                            ListViewItem item = new ListViewItem(residentId.ToString());
                            item.SubItems.Add(fullName);
                            item.SubItems.Add(address);
                            item.SubItems.Add(totalPaid.ToString("F2"));
                            item.SubItems.Add(totalMissed.ToString());

                            // Determine if resident is active
                            bool isActive = false;
                            using (SqlConnection connCheck = DatabaseHelper.GetConnection())
                            using (SqlCommand cmdCheck = new SqlCommand(
                                "SELECT COUNT(*) FROM Residents WHERE HomeownerID=@residentId", connCheck))
                            {
                                cmdCheck.Parameters.AddWithValue("@residentId", residentId);
                                connCheck.Open();
                                isActive = (int)cmdCheck.ExecuteScalar() > 0;
                            }
                            item.SubItems.Add(isActive ? "Active" : "Inactive");

                            lvResidents.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (lvResidents.SelectedItems.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Please select a resident to generate the receipt.", "No Resident Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int residentId = Convert.ToInt32(lvResidents.SelectedItems[0].SubItems[0].Text);

            string fullName = "";
            string address = "";
            List<(string Month, decimal Amount, DateTime PaymentDate)> payments = new List<(string, decimal, DateTime)>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                SqlCommand cmdResident = new SqlCommand(
                    "SELECT FirstName, MiddleName, LastName, HomeAddress FROM Residents WHERE HomeownerID=@residentId", conn);
                cmdResident.Parameters.AddWithValue("@residentId", residentId);

                using (SqlDataReader reader = cmdResident.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                        address = reader["HomeAddress"].ToString();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Resident not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                SqlCommand cmdPayments = new SqlCommand(
                    "SELECT MonthCovered, AmountPaid, PaymentDate FROM MonthlyDues WHERE HomeownerId=@residentId ORDER BY MonthCovered", conn);
                cmdPayments.Parameters.AddWithValue("@residentId", residentId);

                using (SqlDataReader reader = cmdPayments.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["MonthCovered"].ToString();
                        decimal amount = Convert.ToDecimal(reader["AmountPaid"]);
                        DateTime date = Convert.ToDateTime(reader["PaymentDate"]);
                        payments.Add((month, amount, date));
                    }
                }
            }

            StringBuilder html = new StringBuilder();
            html.Append(@"
<html>
<head>
    <title>Phase 2F Mabuhay Homes</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 30px; }
        h2 { text-align: center; font-weight: bold; margin-bottom: 20px; }
        p { font-size: 14px; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        th, td { border: 1px solid #333; padding: 8px; text-align: left; font-size: 13px; }
        th { background-color: #f0f0f0; }
        tr:nth-child(even) { background-color: #f9f9f9; }
        .total-row td { font-weight: bold; background-color: #e0e0e0; }
    </style>
</head>
<body>");

            html.Append($"<h2>Payment Receipt</h2>");
            html.Append($"<p><b>Resident:</b> {fullName}<br>");
            html.Append($"<b>Address:</b> {address}</p>");

            html.Append("<table>");
            html.Append("<tr><th>Month Covered</th><th>Amount Paid</th><th>Payment Date</th></tr>");

            decimal totalPaid = 0;
            foreach (var p in payments)
            {
                html.Append($"<tr><td>{p.Month}</td><td>{p.Amount:F2}</td><td>{p.PaymentDate:MMMM dd, yyyy}</td></tr>");
                totalPaid += p.Amount;
            }

            html.Append($"<tr class='total-row'><td>Total Paid</td><td>{totalPaid:F2}</td><td></td></tr>");
            html.Append("</table></body></html>");

            WebBrowser webBrowser = new WebBrowser
            {
                DocumentText = html.ToString()
            };

            webBrowser.DocumentCompleted += (s, ev) =>
            {
                webBrowser.Print();
                System.Windows.Forms.MessageBox.Show("The receipt is ready to print or save as PDF.", "Print Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        public void cmbResidentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbResidentFilter.SelectedItem.ToString())
            {
                case "Active Residents":
                    LoadResidents(true);
                    break;
                case "Inactive Residents":
                    LoadResidents(false);
                    break;
                case "All Residents":
                    LoadResidents(null);
                    break;
            }
        }
        public ComboBox ResidentFilterComboBox
        {
            get { return cmbResidentFilter; }
        }

    }
}
