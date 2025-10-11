using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq; 
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class addvisitor : Form
    {
        public event EventHandler VisitorAdded;

        public addvisitor()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            VisitorDTP.Value = DateTime.Now;
            VisitorDTP.Format = DateTimePickerFormat.Custom;
            VisitorDTP.CustomFormat = "dddd, dd MMMM yyyy";
            VisitorDTP.Format = DateTimePickerFormat.Time;

            Purposetxt.SelectedIndexChanged += Purposetxt_SelectedIndexChanged;
        }

        private void addvisitor_Load(object sender, EventArgs e)
        {
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
                string input = Interaction.InputBox(
                    "Please specify the other reason:",
                    "Other Reason",
                    "");

                if (string.IsNullOrWhiteSpace(input))
                {
                    Purposetxt.SelectedIndex = 0;
                }
                else
                {
                    if (!Purposetxt.Items.Contains(input))
                    {
                        Purposetxt.Items.Insert(Purposetxt.Items.Count - 1, input);
                    }
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

                cmd.Parameters.AddWithValue("@Name", VisitorNametxt.Text.Trim());
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
            string visitorName = VisitorNametxt.Text.Trim();
            string contactNumber = ContactNumtxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(visitorName) ||
                string.IsNullOrWhiteSpace(contactNumber) ||
                Purposetxt.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Incomplete Form",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!visitorName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("The Visitor Name must only contain letters and spaces.", "Invalid Name",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (contactNumber.Length != 11 || !contactNumber.All(char.IsDigit))
            {
                MessageBox.Show("The Contact Number must be exactly 11 digits.", "Invalid Contact Number",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void VisitorNametxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SelectNextControl((Control)sender, true, true, true, true);
            }

            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; 
            }
        }

        private void ContactNumtxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            if (ContactNumtxt.Text.Length >= 11 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cancelvisitorbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}