using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class Announcement : UserControl
    {
        public Announcement()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }
        private ToolTip toolTip = new ToolTip(); //mouse hover
        private void button3_Click(object sender, EventArgs e) //btnPostAnnouncement
        {
            PostAnnouncement postForm = new PostAnnouncement(this); // open form
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
            panelAnnouncement.Controls.Clear();
            panelAnnouncement.AutoScroll = true;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Delete expired announcements from DB
                string cleanupQuery = "DELETE FROM Announcements WHERE ExpirationDate IS NOT NULL AND ExpirationDate < GETDATE()";
                SqlCommand cleanupCmd = new SqlCommand(cleanupQuery, conn);
                cleanupCmd.ExecuteNonQuery();

                string query = @"SELECT Id, Title, Message, DatePosted, ExpirationDate, IsImportant
                 FROM Announcements 
                 WHERE ExpirationDate IS NULL OR ExpirationDate >= GETDATE()
                 ORDER BY DatePosted DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                int y = 10;

                while (reader.Read())
                {
                    int announcementId = Convert.ToInt32(reader["Id"]);

                    Panel panel = new Panel();
                    panel.Width = panelAnnouncement.Width - 40;
                    panel.Left = 10;
                    panel.Top = y;
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    panel.BackColor = Color.White;

                    // Flow container for labels
                    FlowLayoutPanel container = new FlowLayoutPanel();
                    container.FlowDirection = FlowDirection.TopDown;
                    container.WrapContents = false;
                    container.AutoSize = true;
                    container.Location = new Point(10, 10);
                    container.BackColor = Color.Transparent;

                    bool isImportant = false;
                    if (reader["IsImportant"] != DBNull.Value)
                        isImportant = Convert.ToBoolean(reader["IsImportant"]);

                    // Title Label
                    Label lblTitle = new Label();
                    lblTitle.Text = (isImportant ? "⚠️ " : "") + reader["Title"].ToString();
                    lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
                    lblTitle.AutoSize = true;
                    lblTitle.BackColor = Color.Transparent;

                    if (isImportant)
                    {
                        lblTitle.ForeColor = Color.Red;
                        toolTip.SetToolTip(lblTitle, "Important Announcement!");
                    }

                    container.Controls.Add(lblTitle);

                    if (isImportant)
                        panel.BackColor = Color.FromArgb(255, 255, 230);

                    // Posted date
                    Label lblDate = new Label();
                    lblDate.Text = "Posted: " + Convert.ToDateTime(reader["DatePosted"]).ToString("g");
                    lblDate.Font = new Font("Arial", 6, FontStyle.Italic);
                    lblDate.AutoSize = true;
                    lblDate.BackColor = Color.Transparent;
                    container.Controls.Add(lblDate);

                    // Expiration date
                    if (reader["ExpirationDate"] != DBNull.Value)
                    {
                        Label lblExpire = new Label();
                        lblExpire.Text = "Expires: " + Convert.ToDateTime(reader["ExpirationDate"]).ToString("d");
                        lblExpire.Font = new Font("Arial", 6, FontStyle.Italic);
                        lblExpire.AutoSize = true;
                        lblExpire.BackColor = Color.Transparent;
                        container.Controls.Add(lblExpire);
                    }

                    // Message
                    Label lblMessage = new Label();
                    lblMessage.Text = reader["Message"].ToString();
                    lblMessage.Font = new Font("Arial", 9, FontStyle.Regular);
                    lblMessage.AutoSize = true;
                    lblMessage.MaximumSize = new Size(panel.Width - 180, 0);
                    lblMessage.BackColor = Color.Transparent;
                    container.Controls.Add(lblMessage);


                    // Add container to panel
                    panel.Controls.Add(container);

                    // Action to open the AnnouncementViewForm
                    Action openAnnouncement = () =>
                    {
                        AnnouncementViewForm viewForm = new AnnouncementViewForm(
                            lblTitle.Text,
                            lblMessage.Text, 
                            isImportant
                        );
                        viewForm.StartPosition = FormStartPosition.CenterScreen;
                        viewForm.ShowDialog();
                    };

                    panel.Cursor = Cursors.Hand;
                    panel.Click += (s, e) => openAnnouncement();

                    foreach (Control ctrl in panel.Controls)
                    {
                        ctrl.Cursor = Cursors.Hand;
                        ctrl.Click += (s, e) => openAnnouncement();
                    }

                    // Edit Button
                    Button btnEdit = new Button();
                    btnEdit.Text = "Edit";
                    btnEdit.Tag = announcementId;
                    btnEdit.Width = 60;
                    btnEdit.Height = 28;
                    btnEdit.Location = new Point(panel.Width - 160, 10);
                    btnEdit.Click += (s, e) => EditAnnouncement((int)btnEdit.Tag);

                    // Delete Button
                    Button btnDelete = new Button();
                    btnDelete.Text = "Delete";
                    btnDelete.Tag = announcementId;
                    btnDelete.Width = 80;
                    btnDelete.Height = 28;
                    btnDelete.Location = new Point(panel.Width - 90, 10);
                    btnDelete.Click += (s, e) => DeleteAnnouncement((int)btnDelete.Tag);

                    panel.Controls.Add(btnEdit);
                    panel.Controls.Add(btnDelete);
                    panelAnnouncement.Controls.Add(panel);

                    y += panel.Height + 10;
                }
            }
        }
        public List<(string Title, string Message, bool IsImportant, DateTime? Expiry)> GetRecentAnnouncements(int limit = 6)
        {
            var announcements = new List<(string Title, string Message, bool IsImportant, DateTime? Expiry)>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT TOP (@limit) Title, Message, IsImportant, ExpirationDate
            FROM Announcements
            WHERE ExpirationDate IS NULL OR ExpirationDate >= GETDATE()
            ORDER BY 
                CASE WHEN IsImportant = 1 THEN 0 ELSE 1 END,  -- Important first
                DatePosted DESC";  // Newest next

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@limit", limit);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        announcements.Add((
                            reader["Title"].ToString(),
                            reader["Message"].ToString(),
                            reader["IsImportant"] != DBNull.Value && Convert.ToBoolean(reader["IsImportant"]),
                            reader["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpirationDate"]) : (DateTime?)null
                        ));
                    }
                }
            }

            return announcements;
        }

        private void EditAnnouncement(int id)
        {
            // current title & message (and expiration)
            string currentTitle = "", currentMessage = "";
            DateTime? currentExpire = null;

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT Title, Message, ExpirationDate FROM Announcements WHERE Id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentTitle = reader["Title"].ToString();
                    currentMessage = reader["Message"].ToString();
                    if (reader["ExpirationDate"] != DBNull.Value)
                        currentExpire = Convert.ToDateTime(reader["ExpirationDate"]);
                }
            }

            // Open PostAnnouncement form in edit mode
            PostAnnouncement editForm = new PostAnnouncement(this, id, currentTitle, currentMessage, currentExpire);
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

                LoadAnnouncement();
            }
        }
    }
}
