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
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class dashboardControl : UserControl
    {
        public dashboardControl()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void dashboardControl_Load(object sender, EventArgs e)
        {
            // Date and Time
            lblDateTime.Text = DateTime.Now.ToString("dddd, MMM dd yyyy | hh:mm tt");

            // Dynamic Greeting
            int hour = DateTime.Now.Hour;
            string greeting;

            if (hour < 12)
                greeting = "Good Morning";
            else if (hour < 18)
                greeting = "Good Afternoon";
            else
                greeting = "Good Evening";

            lblGreeting.Text = $"{greeting}, {CurrentUser.FullName}!";
            LoadDashboardAnnouncements();
        }
        private void LoadDashboardAnnouncements()
        {
            panelQuickAnnouncements.Controls.Clear();
            Random rnd = new Random(); // for slight rotation

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT TOP 8 Id, Title, Message, DatePosted, IsImportant
            FROM Announcements
            WHERE ExpirationDate IS NULL OR ExpirationDate >= GETDATE()
            ORDER BY 
                CASE WHEN IsImportant = 1 THEN 0 ELSE 1 END,
                DatePosted DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int x = 10;
                    int y = 10;
                    int count = 0;
                    int noteSize = 235;
                    int spacing = 20;

                    while (reader.Read())
                    {
                        bool isImportant = reader["IsImportant"] != DBNull.Value && Convert.ToBoolean(reader["IsImportant"]);

                        // Store values before attaching events
                        string noteTitle = reader["Title"].ToString();
                        string noteMessage = reader["Message"].ToString();

                        Panel note = new Panel();
                        note.Width = noteSize;
                        note.Height = noteSize;
                        note.Left = x;
                        note.Top = y;
                        note.BackColor = isImportant ? Color.FromArgb(255, 250, 210) : Color.FromArgb(255, 255, 200);
                        note.BorderStyle = BorderStyle.FixedSingle;

                        // Rounded corners
                        note.Region = System.Drawing.Region.FromHrgn(
                            CreateRoundRectRgn(0, 0, note.Width, note.Height, 15, 15)
                        );

                        // Slight random rotation
                        float angle = rnd.Next(-5, 6);
                        note.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            e.Graphics.TranslateTransform(note.Width / 2, note.Height / 2);
                            e.Graphics.RotateTransform(angle);
                            e.Graphics.TranslateTransform(-note.Width / 2, -note.Height / 2);
                        };

                        // Labels
                        Label lblTitle = new Label();
                        lblTitle.Text = (isImportant ? "⚠️ " : "") + noteTitle;
                        lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                        lblTitle.AutoSize = false;
                        lblTitle.Width = note.Width - 20;
                        lblTitle.Height = 35;
                        lblTitle.Location = new Point(10, 10);
                        lblTitle.ForeColor = isImportant ? Color.Red : Color.Black;

                        Label lblMsg = new Label();
                        lblMsg.Text = noteMessage;
                        lblMsg.Font = new Font("Segoe UI", 9);
                        lblMsg.AutoSize = false;
                        lblMsg.Width = note.Width - 20;
                        lblMsg.Height = 130;
                        lblMsg.Location = new Point(10, 50);
                        lblMsg.ForeColor = Color.FromArgb(60, 60, 60);

                        note.Controls.Add(lblTitle);
                        note.Controls.Add(lblMsg);
                        panelQuickAnnouncements.Controls.Add(note);

                        // Tooltip for important notes
                        if (isImportant)
                        {
                            ToolTip noteToolTip = new ToolTip();
                            noteToolTip.AutoPopDelay = 5000;
                            noteToolTip.InitialDelay = 500;
                            noteToolTip.ReshowDelay = 200;
                            noteToolTip.ShowAlways = true;

                            noteToolTip.SetToolTip(note, "Important Announcement!");
                            foreach (Control ctrl in note.Controls)
                                noteToolTip.SetToolTip(ctrl, "Important Announcement!");
                        }

                        // Click events
                        note.Cursor = Cursors.Hand;
                        note.Click += (s, e) =>
                        {
                            AnnouncementViewForm viewForm = new AnnouncementViewForm(
                                noteTitle,
                                noteMessage,
                                isImportant
                            );
                            viewForm.StartPosition = FormStartPosition.CenterScreen;
                            viewForm.ShowDialog();
                        };

                        foreach (Control ctrl in note.Controls)
                        {
                            ctrl.Cursor = Cursors.Hand;
                            ctrl.Click += (s, e) =>
                            {
                                AnnouncementViewForm viewForm = new AnnouncementViewForm(
                                    noteTitle,
                                    noteMessage,
                                    isImportant
                                );
                                viewForm.StartPosition = FormStartPosition.CenterScreen;
                                viewForm.ShowDialog();
                            };
                        }

                        // Positioning
                        count++;
                        if (count % 4 == 0)
                        {
                            x = 10;
                            y += note.Height + spacing;
                        }
                        else
                        {
                            x += note.Width + spacing;
                        }
                    }
                }
            }
        }



        // Helper for rounded corners
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

    }
}

    




