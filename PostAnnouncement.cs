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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace RECOMANAGESYS
{
    public partial class PostAnnouncement : Form
    {
        private Announcement parentControl;
        private int editId = -1; 
        public PostAnnouncement(Announcement parent)
        {
            InitializeComponent();
            parentControl = parent;
        }
        public PostAnnouncement(Announcement parent, int id, string title, string message)
        {
            InitializeComponent();
            parentControl = parent;
            editId = id;
            txtTitle.Text = title;
            txtMessage.Text = message;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd;

                if (editId == -1)
                {
                    // New announcement
                    string query = "INSERT INTO Announcements (Title, Message, DatePosted) VALUES (@title, @msg, @date)";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@msg", txtMessage.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                }
                else
                {
                    // Editing existing announcement
                    string query = "UPDATE Announcements SET Title=@title, Message=@msg WHERE Id=@id";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@msg", txtMessage.Text);
                    cmd.Parameters.AddWithValue("@id", editId);
                }

                cmd.ExecuteNonQuery();
            }

            if (parentControl != null)
            {
                parentControl.LoadAnnouncement(); // refresh display after post/edit
            }

            MessageBox.Show(editId == -1 ? "Announcement posted successfully!" : "Announcement updated successfully!");

            txtTitle.Clear();
            txtMessage.Clear();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

