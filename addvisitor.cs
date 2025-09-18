using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace RECOMANAGESYS
{

    public partial class addvisitor : Form
    {
        public addvisitor()
        {
            InitializeComponent();
            VisitorDTP.Value = DateTime.Now;
            VisitorDTP.Value = DateTime.Now;

            VisitorDTP.Format = DateTimePickerFormat.Custom;
            VisitorDTP.CustomFormat = "dddd, dd MMMM yyyy";

            VisitorDTP.Format = DateTimePickerFormat.Time;
            VisitorDTP.ShowUpDown = true;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void addvisitor_Load(object sender, EventArgs e)
        {

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
                cmd.Parameters.AddWithValue("@Purpose", Purposetxt.Text);
                cmd.Parameters.AddWithValue("@TimeIn", VisitorDTP.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }


        }
        private bool ValidateInputs()
        {



            if (string.IsNullOrWhiteSpace(VisitorNametxt.Text) ||
               string.IsNullOrWhiteSpace(ContactNumtxt.Text) ||
               string.IsNullOrWhiteSpace(Purposetxt.Text))

            {
                MessageBox.Show("Please fill in all required fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;

        }

    }
}

