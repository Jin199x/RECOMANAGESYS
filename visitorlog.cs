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

namespace RECOMANAGESYS
{
    public partial class visitorlog : UserControl
    {

        public visitorlog()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            VisitorLog();
        }
        public void RefreshData()
        {
            VisitorLog();
        }
        private void VisitorLog()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT 
                        VisitorID,
                        VisitorName,
                        ContactNumber,
                        FORMAT(Date, 'MMM dd, yyyy') AS Date,                           
                        VisitPurpose,
                        FORMAT(TimeIn, 'hh :mm tt') AS TimeIn,
                        CASE
                            WHEN TimeOut IS NULL THEN 'Active'
                            ELSE FORMAT(TimeOut, 'hh :mm tt')
                        END AS TimeOut                                                          
                        FROM TBL_VisitorsLog
                        ORDER BY VisitorID DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                VisitorLogDGV.DataSource = dt;
                DGVFormat();
            }
        }

        private void DGVFormat()
        {
            try
            {
                VisitorLogDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


                if (VisitorLogDGV.Columns.Contains("VisitDate"))
                    VisitorLogDGV.Columns["VisitDate"].DefaultCellStyle.Format = "MMM dd, yyyy";

                if (VisitorLogDGV.Columns.Contains("TimeIn"))
                    VisitorLogDGV.Columns["TimeIn"].DefaultCellStyle.Format = "hh:mm tt";

                if (VisitorLogDGV.Columns.Contains("TimeOut"))
                    VisitorLogDGV.Columns["TimeOut"].DefaultCellStyle.Format = "hh:mm tt";


                foreach (DataGridViewRow row in VisitorLogDGV.Rows)
                {
                    if (row.Cells["TimeOut"]?.Value?.ToString() == "Active")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error formatting grid: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void visitorlog_Load(object sender, EventArgs e)
        {

        }

        private void searchbtn_Click(object sender, EventArgs e)
        {

        }

        private void addvisitor_Click(object sender, EventArgs e)
        {
            addvisitor visitor = new addvisitor();
            if (visitor.ShowDialog() == DialogResult.OK)
            {
                VisitorLog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (VisitorLogDGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a visitor first", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (VisitorLogDGV.SelectedRows[0].Cells["VisitorID"].Value == null ||
                VisitorLogDGV.SelectedRows[0].Cells["VisitorID"].Value == DBNull.Value)
            {
                MessageBox.Show("Invalid visitor record selected", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int visitorID = Convert.ToInt32(VisitorLogDGV.SelectedRows[0].Cells["VisitorID"].Value);

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE TBL_VisitorsLog SET TimeOut = GETDATE() WHERE VisitorID = @VisitorID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VisitorID", visitorID);
                    cmd.ExecuteNonQuery();
                    VisitorLog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recording exit: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VisitorLogDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
