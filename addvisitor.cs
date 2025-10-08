using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class addvisitor : Form
    {
        public event EventHandler VisitorAdded; //auto refresh
        public addvisitor()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            VisitorDTP.Value = DateTime.Now;
            VisitorDTP.Format = DateTimePickerFormat.Custom;
            VisitorDTP.CustomFormat = "dddd, dd MMMM yyyy";

            VisitorDTP.Format = DateTimePickerFormat.Time;
            VisitorDTP.ShowUpDown = true;

            // Attach event handler for "Other" selection
            Purposetxt.SelectedIndexChanged += Purposetxt_SelectedIndexChanged;
        }

        private void label6_Click(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void label3_Click(object sender, EventArgs e) { }

        private void label4_Click(object sender, EventArgs e) { }

        private void label5_Click(object sender, EventArgs e) { }

        private void addvisitor_Load(object sender, EventArgs e)
        {
            // Populate ComboBox if not already done in Designer
            if (Purposetxt.Items.Count == 0)
            {
                Purposetxt.Items.AddRange(new object[]
                {
                    "Meeting",
                    "Delivery",
                    "Monthly Dues",
                    "Personal Visit",
                    "Requesting Documents",
                    "Other"
                });
            }
            Purposetxt.SelectedIndex = 0;
        }

        private void Purposetxt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Purposetxt.SelectedItem.ToString() == "Other")
            {
                // Show input dialog for custom reason
                string input = Interaction.InputBox(
                    "Please specify the other reason:",
                    "Other Reason",
                    "");

                if (string.IsNullOrWhiteSpace(input))
                {
                    // If user cancels or leaves blank, revert to default selection
                    Purposetxt.SelectedIndex = 0;
                }
                else
                {
                    // Replace "Other" with custom input and select it
                    Purposetxt.Items[Purposetxt.SelectedIndex] = input;
                    Purposetxt.SelectedItem = input;
                }
            }
        }

        private void savevisitorbtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"INSERT INTO TBL_VisitorsLog
                                (VisitorName, ContactNumber, Date, VisitPurpose, TimeIn)
                                VALUES 
                                (@Name, @ContactNumber, @Date, @Purpose, @TimeIn)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", VisitorNametxt.Text);
                cmd.Parameters.AddWithValue("@ContactNumber", ContactNumtxt.Text);
                cmd.Parameters.AddWithValue("@Date", VisitorDTP.Value.Date);
                cmd.Parameters.AddWithValue("@Purpose", Purposetxt.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@TimeIn", VisitorDTP.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                VisitorAdded?.Invoke(this, EventArgs.Empty);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(VisitorNametxt.Text) ||
                string.IsNullOrWhiteSpace(ContactNumtxt.Text) ||
                Purposetxt.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void VisitorNametxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Ignore Enter key
            }
        }

        private void ContactNumtxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Ignore Enter key
            }
        }
    }
}
