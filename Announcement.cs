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
    public partial class Announcement : UserControl
    {
        public Announcement()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e) //btnPostAnnouncement
        {
            PostAnnouncement postForm = new PostAnnouncement(this); // pass reference
            postForm.Show();
        }

        private void PanelAnnouncement_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadAnnouncement(object sender, EventArgs e)
        {
            LoadAnnouncement();
        }
        public void LoadAnnouncement()
        {
            PanelAnnouncement.Controls.Clear();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Title, Message, DatePosted FROM Announcements ORDER BY DatePosted DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                int y = 10;

                while (reader.Read())
                {
                    int announcementId = Convert.ToInt32(reader["Id"]);

                    Panel panel = new Panel();
                    panel.Width = PanelAnnouncement.Width - 40;
                    panel.Height = 120;
                    panel.Left = 10;
                    panel.Top = y;
                    panel.BorderStyle = BorderStyle.FixedSingle;

                    Label lblTitle = new Label();
                    lblTitle.Text = reader["Title"].ToString();
                    lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
                    lblTitle.AutoSize = true;
                    lblTitle.Location = new Point(10, 10);

                    Label lblDate = new Label();
                    lblDate.Text = Convert.ToDateTime(reader["DatePosted"]).ToString("g");
                    lblDate.Font = new Font("Arial", 8, FontStyle.Italic);
                    lblDate.AutoSize = true;
                    lblDate.Location = new Point(10, 35);

                    Label lblMessage = new Label();
                    lblMessage.Text = reader["Message"].ToString();
                    lblMessage.AutoSize = true;
                    lblMessage.MaximumSize = new Size(panel.Width - 150, 0); // leave space for buttons
                    lblMessage.Location = new Point(10, 55);

                    //Edit Button
                    Button btnEdit = new Button();
                    btnEdit.Text = "Edit";
                    btnEdit.Tag = announcementId; // store ID
                    btnEdit.Width = 60;
                    btnEdit.Location = new Point(panel.Width - 140, 10);
                    btnEdit.Click += (s, e) => EditAnnouncement((int)btnEdit.Tag);

                    // Delete Button
                    Button btnDelete = new Button();
                    btnDelete.Text = "Delete";
                    btnDelete.Tag = announcementId; // store ID
                    btnDelete.Width = 60;
                    btnDelete.Location = new Point(panel.Width - 70, 10);
                    btnDelete.Click += (s, e) => DeleteAnnouncement((int)btnDelete.Tag);

                    // Add to panel
                    panel.Controls.Add(lblTitle);
                    panel.Controls.Add(lblDate);
                    panel.Controls.Add(lblMessage);
                    panel.Controls.Add(btnEdit);
                    panel.Controls.Add(btnDelete);

                    PanelAnnouncement.Controls.Add(panel);
                    y += panel.Height + 10;
                }
            }
        }
        private void EditAnnouncement(int id)
        {
            // Fetch current title & message from DB
            string currentTitle = "", currentMessage = "";
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT Title, Message FROM Announcements WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentTitle = reader["Title"].ToString();
                    currentMessage = reader["Message"].ToString();
                }
            }

            // Open PostAnnouncement form in "edit mode"
            PostAnnouncement editForm = new PostAnnouncement(this, id, currentTitle, currentMessage);
            editForm.ShowDialog();
        }

        
        private void DeleteAnnouncement(int id)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this announcement?",
                                                  "Confirm Delete", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Announcements WHERE Id=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadAnnouncement(); // refresh display
            }
        }
    }
    
}

