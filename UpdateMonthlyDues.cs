using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class UpdateMonthlyDues : Form

    {
        public Action OnPaymentSaved;
        private int selectedHomeownerId;
        private List<Tuple<int, string>> residentUnits; // UnitID, Address
        private decimal dueRate = 100; // fixed per month

        public UpdateMonthlyDues()
        {
            InitializeComponent();
            btnSearch.Click += btnSearch_Click;
            cmbUnits.SelectedIndexChanged += cmbUnits_SelectedIndexChanged;
            clbMissedMonths.ItemCheck += clbMissedMonths_ItemCheck;
            residentUnits = new List<Tuple<int, string>>();
        }

        private void cancelvisitor_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtResidentID.Text, out selectedHomeownerId))
            {
                MessageBox.Show("Please enter a valid Resident ID.");
                return;
            }

            residentUnits.Clear();
            cmbUnits.Items.Clear();
            clbMissedMonths.Items.Clear();
            lblResidentName.Text = "";
            lblUnit.Text = "";
            lblRemainingDebt.Text = "0.00";
            lblTotalAmount.Text = "0.00";

            // Fetch resident info with IsActive check
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT FirstName, MiddleName, LastName, HomeAddress, IsActive 
          FROM Residents 
          WHERE HomeownerID=@residentId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", selectedHomeownerId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool isActive = Convert.ToBoolean(reader["IsActive"]);
                        if (!isActive)
                        {
                            MessageBox.Show("This resident is inactive.", "Inactive Resident", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Close();
                            return;
                        }

                        lblResidentName.Text = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}";
                        string defaultAddress = reader["HomeAddress"].ToString();

                        // Fetch units for this resident
                        using (SqlConnection conn2 = DatabaseHelper.GetConnection())
                        using (SqlCommand cmd2 = new SqlCommand(
                            @"SELECT hu.UnitID, r.HomeAddress 
                      FROM HomeownerUnits hu
                      INNER JOIN Residents r ON hu.HomeownerID = r.HomeownerID
                      WHERE hu.HomeownerID=@residentId", conn2))
                        {
                            cmd2.Parameters.AddWithValue("@residentId", selectedHomeownerId);
                            conn2.Open();
                            using (SqlDataReader reader2 = cmd2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    int unitId = Convert.ToInt32(reader2["UnitID"]);
                                    string address = reader2["HomeAddress"] != DBNull.Value
                                        ? reader2["HomeAddress"].ToString()
                                        : defaultAddress;

                                    residentUnits.Add(new Tuple<int, string>(unitId, address));
                                    cmbUnits.Items.Add(unitId.ToString()); // Only UnitID shown
                                }
                            }
                        }

                        // Auto-select and lock combo box if only 1 unit
                        if (cmbUnits.Items.Count == 1)
                        {
                            cmbUnits.SelectedIndex = 0;
                            cmbUnits.Enabled = false;
                        }
                        else if (cmbUnits.Items.Count > 1)
                        {
                            cmbUnits.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Resident not found.");
                        return;
                    }
                }
            }
        }


        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1) return;

            int unitId = residentUnits[cmbUnits.SelectedIndex].Item1;
            string address = residentUnits[cmbUnits.SelectedIndex].Item2;

            lblUnit.Text = address;

            LoadMissedMonths(unitId);
        }

        private void LoadMissedMonths(int unitId)
        {
            clbMissedMonths.Items.Clear();
            HashSet<string> paidMonths = new HashSet<string>();
            decimal totalPaid = 0;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MonthCovered, AmountPaid FROM MonthlyDues 
                  WHERE HomeownerId=@residentId AND UnitID=@unitId", conn))
            {
                cmd.Parameters.AddWithValue("@residentId", selectedHomeownerId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        paidMonths.Add(reader["MonthCovered"].ToString());
                        totalPaid += Convert.ToDecimal(reader["AmountPaid"]);
                    }
                }
            }

            DateTime now = DateTime.Now;
            for (int m = 1; m <= now.Month; m++)
            {
                string monthName = new DateTime(now.Year, m, 1).ToString("MMMM yyyy");
                if (!paidMonths.Contains(monthName))
                    clbMissedMonths.Items.Add(monthName);
            }

            lblRemainingDebt.Text = (clbMissedMonths.Items.Count * dueRate).ToString("F2");
            UpdateTotalAmount(); // Reset total
        }

        private void clbMissedMonths_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Delay calculation until after the check state is applied
            this.BeginInvoke((Action)(() => UpdateTotalAmount()));
        }

        private void UpdateTotalAmount()
        {
            decimal total = clbMissedMonths.CheckedItems.Count * dueRate;
            // Include the item currently being checked/unchecked
            foreach (int i in clbMissedMonths.CheckedIndices)
            {
                total += 0; // Already counted
            }

            lblTotalAmount.Text = total.ToString("F2");
        }

        private void savevisitor_Click(object sender, EventArgs e)
        {
            if (cmbUnits.SelectedIndex == -1 || clbMissedMonths.CheckedItems.Count == 0)
            {
                MessageBox.Show("Select a unit and at least one month to pay.");
                return;
            }

            int unitId = residentUnits[cmbUnits.SelectedIndex].Item1;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("", conn))
            {
                conn.Open();
                foreach (var month in clbMissedMonths.CheckedItems)
                {
                    string monthCovered = month.ToString();

                    SqlCommand checkCmd = new SqlCommand(
                        @"SELECT COUNT(*) FROM MonthlyDues 
                          WHERE HomeownerId=@residentId AND UnitID=@unitId AND MonthCovered=@monthCovered", conn);
                    checkCmd.Parameters.AddWithValue("@residentId", selectedHomeownerId);
                    checkCmd.Parameters.AddWithValue("@unitId", unitId);
                    checkCmd.Parameters.AddWithValue("@monthCovered", monthCovered);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        MessageBox.Show($"Month {monthCovered} is already paid.");
                        continue;
                    }

                    cmd.CommandText = @"INSERT INTO MonthlyDues
                        (HomeownerId, UnitID, PaymentDate, AmountPaid, DueRate, MonthCovered)
                        VALUES (@residentId, @unitId, @paymentDate, @amountPaid, @dueRate, @monthCovered)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@residentId", selectedHomeownerId);
                    cmd.Parameters.AddWithValue("@unitId", unitId);
                    cmd.Parameters.AddWithValue("@paymentDate", dtpPaymentDate.Value);
                    cmd.Parameters.AddWithValue("@amountPaid", decimal.Parse(lblTotalAmount.Text));
                    cmd.Parameters.AddWithValue("@dueRate", dueRate);
                    cmd.Parameters.AddWithValue("@monthCovered", monthCovered);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Payment(s) saved successfully!");
            LoadMissedMonths(residentUnits[cmbUnits.SelectedIndex].Item1);
            OnPaymentSaved?.Invoke();
            this.Close();
        }

        private void UpdateMonthlyDues_Load(object sender, EventArgs e)
        {

        }
    }
}
