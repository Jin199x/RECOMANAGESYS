using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class Announcement : UserControl
    {
        public event EventHandler AnnouncementChanged;
        private ToolTip toolTip = new ToolTip(); // mouse hover

        public Announcement()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void button3_Click(object sender, EventArgs e) // btnPostAnnouncement
        {
            PostAnnouncement postForm = new PostAnnouncement(this);

            postForm.AnnouncementChanged += (s, args) =>
            {
                this.LoadAnnouncement();
                AnnouncementChanged?.Invoke(this, EventArgs.Empty);
            };

            postForm.ShowDialog();
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

                string query = @"
                    SELECT Id, Title, Message, DatePosted, ExpirationDate, IsImportant 
                    FROM Announcements 
                    WHERE ExpirationDate IS NULL OR ExpirationDate >= GETDATE() 
                    ORDER BY DatePosted DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                int y = 10;

                while (reader.Read())
                {
                    int announcementId = Convert.ToInt32(reader["Id"]);

                    Panel panel = new Panel
                    {
                        Width = panelAnnouncement.Width - 40,
                        Left = 10,
                        Top = y,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White
                    };

                    // Flow container for labels
                    FlowLayoutPanel container = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        WrapContents = false,
                        AutoSize = true,
                        Location = new Point(10, 10),
                        BackColor = Color.Transparent
                    };

                    bool isImportant = reader["IsImportant"] != DBNull.Value && Convert.ToBoolean(reader["IsImportant"]);

                    // Title Label
                    Label lblTitle = new Label
                    {
                        Text = (isImportant ? "⚠️ " : "") + reader["Title"].ToString(),
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        AutoSize = true,
                        BackColor = Color.Transparent
                    };

                    if (isImportant)
                    {
                        lblTitle.ForeColor = Color.Red;
                        toolTip.SetToolTip(lblTitle, "Important Announcement!");
                        panel.BackColor = Color.FromArgb(255, 255, 230);
                    }

                    container.Controls.Add(lblTitle);

                    // Posted date
                    Label lblDate = new Label
                    {
                        Text = "Posted: " + Convert.ToDateTime(reader["DatePosted"]).ToString("g"),
                        Font = new Font("Arial", 6, FontStyle.Italic),
                        AutoSize = true,
                        BackColor = Color.Transparent
                    };
                    container.Controls.Add(lblDate);

                    // Expiration date
                    if (reader["ExpirationDate"] != DBNull.Value)
                    {
                        Label lblExpire = new Label
                        {
                            Text = "Expires: " + Convert.ToDateTime(reader["ExpirationDate"]).ToString("d"),
                            Font = new Font("Arial", 6, FontStyle.Italic),
                            AutoSize = true,
                            BackColor = Color.Transparent
                        };
                        container.Controls.Add(lblExpire);
                    }

                    // Message Label
                    Label lblMessage = new Label
                    {
                        Text = reader["Message"].ToString(),
                        Font = new Font("Arial", 9, FontStyle.Regular),
                        AutoSize = true,
                        MaximumSize = new Size(panel.Width - 180, 0),
                        BackColor = Color.Transparent
                    };
                    container.Controls.Add(lblMessage);

                    // Add container to panel
                    panel.Controls.Add(container);

                    // open the AnnouncementViewForm
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
                    Button btnEdit = new Button
                    {
                        Text = "Edit",
                        Tag = announcementId,
                        Width = 60,
                        Height = 28,
                        Location = new Point(panel.Width - 160, 10)
                    };
                    btnEdit.Click += (s, e) => EditAnnouncement((int)btnEdit.Tag);

                    // Delete Button
                    Button btnDelete = new Button
                    {
                        Text = "Delete",
                        Tag = announcementId,
                        Width = 80,
                        Height = 28,
                        Location = new Point(panel.Width - 90, 10)
                    };
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
                    ORDER BY CASE WHEN IsImportant = 1 THEN 0 ELSE 1 END, DatePosted DESC";

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

            PostAnnouncement editForm = new PostAnnouncement(this, id, currentTitle, currentMessage, currentExpire);
            editForm.AnnouncementChanged += (s, e) => AnnouncementChanged?.Invoke(this, EventArgs.Empty);
            editForm.ShowDialog();
        }

        private void DeleteAnnouncement(int id)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this announcement?",
                "Confirm Delete",
                MessageBoxButtons.YesNo
            );

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
                AnnouncementChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        public void RefreshData()
        {
            LoadAnnouncement();
        }
    }
}
