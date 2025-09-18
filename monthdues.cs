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

            // Configure ListViews
            lvResidents.View = View.Details;
            lvResidents.FullRowSelect = true;
            lvResidents.Columns.Clear();
            lvResidents.Columns.Add("ResidentID", 80);
            lvResidents.Columns.Add("UnitID", 80);
            lvResidents.Columns.Add("Full Name", 200);
            lvResidents.Columns.Add("Address", 250);
            lvResidents.Columns.Add("Total Paid", 100);
            lvResidents.Columns.Add("Total Missed", 100);

            lvMonths.View = View.Details;
            lvMonths.FullRowSelect = true;
            lvMonths.Columns.Clear();
            lvMonths.Columns.Add("Month", 150);
            lvMonths.Columns.Add("Status", 100);
            lvMonths.Columns.Add("Payment Date", 150);

            LoadResidentsList();

            // Hook MouseClick event to handle any click on row/subitems
            lvResidents.MouseClick += LvResidents_MouseClick;

        }

        private void addvisitor_Click(object sender, EventArgs e) //AddPayment Form
        {
            frmPayment payform = new frmPayment();
            payform.ShowDialog();

            LoadResidentsList();

            if (lastSelectedResidentId != -1 && lastSelectedUnitId != -1)
            {
                LoadMonthlyDues(lastSelectedResidentId, lastSelectedUnitId);
            }
        }
        private void button2_Click(object sender, EventArgs e) //UpdateMonthylDues Form
        {
            UpdateMonthlyDues updateDues = new UpdateMonthlyDues();
            updateDues.ShowDialog();
            LoadResidentsList();
        }



        private void LoadResidentsList()
        {
            lvResidents.Items.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT r.HomeownerID, r.FirstName, r.MiddleName, r.LastName, r.HomeAddress, u.UnitID
                  FROM Residents r
                  INNER JOIN HomeownerUnits hu ON r.HomeownerID = hu.HomeownerID
                  INNER JOIN TBL_Units u ON hu.UnitID = u.UnitID", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int residentId = Convert.ToInt32(reader["HomeownerID"]);
                        int unitId = Convert.ToInt32(reader["UnitID"]);
                        string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                        string address = reader["HomeAddress"].ToString();

                        // Get total paid and missed
                        (decimal totalPaid, int totalMissed) = GetPaymentSummary(residentId, unitId);

                        ListViewItem item = new ListViewItem(residentId.ToString());
                        item.SubItems.Add(unitId.ToString());
                        item.SubItems.Add(fullName);
                        item.SubItems.Add(address);
                        item.SubItems.Add(totalPaid.ToString("F2"));
                        item.SubItems.Add(totalMissed.ToString());

                        lvResidents.Items.Add(item);
                    }
                }
            }
        }

        private (decimal totalPaid, int totalMissed) GetPaymentSummary(int residentId, int unitId)
        {
            decimal totalPaid = 0;
            var paidMonths = new HashSet<string>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MonthCovered, AmountPaid FROM MonthlyDues 
                  WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
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

            // Count missed months up to current month
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
            lastSelectedUnitId = int.Parse(selected.SubItems[1].Text);

            // Refresh the residents list to update totals
            LoadResidentsList();

            // Keep the selected resident highlighted
            for (int i = 0; i < lvResidents.Items.Count; i++)
            {
                if (lvResidents.Items[i].SubItems[0].Text == lastSelectedResidentId.ToString())
                {
                    lvResidents.Items[i].Selected = true;
                    lvResidents.EnsureVisible(i);
                    break;
                }
            }

            LoadMonthlyDues(lastSelectedResidentId, lastSelectedUnitId);
        }

        private void LoadMonthlyDues(int residentId, int unitId)
        {
            lvMonths.Items.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MonthCovered, PaymentDate FROM MonthlyDues
          WHERE HomeownerId=@residentId AND UnitID=@unitId
          ORDER BY MonthCovered ASC", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", residentId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();

                var paidMonths = new HashSet<string>();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader["MonthCovered"].ToString();
                        DateTime paymentDate = Convert.ToDateTime(reader["PaymentDate"]);
                        paidMonths.Add(month);

                        ListViewItem item = new ListViewItem(month);
                        item.SubItems.Add("Paid");
                        item.BackColor = Color.FromArgb(220, 255, 220);
                        item.SubItems.Add(paymentDate.ToString("MMMM dd, yyyy")); // show payment date
                        lvMonths.Items.Add(item);
                    }
                }

                // Show missed months up to current month
                DateTime now = DateTime.Now;
                for (int m = 1; m <= now.Month; m++)
                {
                    string monthName = new DateTime(now.Year, m, 1).ToString("MMMM yyyy");
                    if (!paidMonths.Contains(monthName))
                    {
                        ListViewItem missedItem = new ListViewItem(monthName);
                        missedItem.SubItems.Add("Missed");
                        missedItem.SubItems.Add(""); // no payment date for missed
                        missedItem.BackColor = Color.LightCoral;
                        lvMonths.Items.Add(missedItem);
                    }
                }
            }

            lvMonths.BringToFront();
            lvMonths.Visible = true;
        }
        private void monthdues_Load(object sender, EventArgs e)
        {
            // In your form constructor or Load event
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            // Clear existing items
            lvResidents.Items.Clear();

            if (string.IsNullOrEmpty(keyword))
            {
                // Optionally, load all residents if search box is empty
                LoadResidentsList();
                return;
            }

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT HomeownerID, FirstName, LastName, HomeAddress 
          FROM Residents 
          WHERE FirstName LIKE @keyword OR LastName LIKE @keyword OR HomeownerID LIKE @keyword", conn))
            {
                cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["HomeownerID"].ToString());
                        item.SubItems.Add(reader["FirstName"].ToString());
                        item.SubItems.Add(reader["LastName"].ToString());
                        item.SubItems.Add(reader["HomeAddress"].ToString()); // optional column
                        lvResidents.Items.Add(item);
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

            // Fetch resident info and payments
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

            // Build formal HTML receipt
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

            // Display in WebBrowser for print
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

    }
}
