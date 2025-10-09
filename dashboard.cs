using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace RECOMANAGESYS
{
    public partial class dashboard : Form
    {
        private dashboardControl dash;
        private monthdues dues;
        private Announcement announce;
        private scheduling sched;
        private visitorlog log;
        private docurepo repo;
        private Homeowners homeowners;


        public dashboard()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }
        private void ShowControl(UserControl control)
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(control);
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dashboard_Load(object sender, EventArgs e)
        {
            lblName.Text = loginform.CurrentUser.FullName;
            lblRole.Text = loginform.CurrentUser.Role;

            dash = new dashboardControl();
            dues = new monthdues();
            announce = new Announcement();
            sched = new scheduling();
            log = new visitorlog();
            repo = new docurepo();
            homeowners = new Homeowners();

            flowLayoutPanel1.Controls.Add(dash);

            LoadUserProfilePicture();

            announce.AnnouncementChanged += (_, __) =>
            {
                dash.RefreshAnnouncements();
            };

            sched.EventsChanged += (s, ev) =>
            {
                dash.RefreshScheduledEvents();
            };
            sched.GarbageSchedulesChanged += (s, ev) =>
            {
                dash.RefreshGarbageSchedule();
            };

            homeowners.MonthDuesControl = dues;

            EnforcePermissions();
        }

        private void EnforcePermissions()
        {
            btnAnnouncement.Visible = loginform.CurrentUser.HasPermission("CanAccessAnnouncements");
            btnRepositories.Visible = loginform.CurrentUser.HasPermission("CanAccessDocuments");
            btnMonthlydues.Visible = loginform.CurrentUser.HasPermission("CanAccessMonthlyDues");
            // btnEditDues.Visible = loginform.CurrentUser.HasPermission("CanEditMonthlyDues");
            btnProfile.Visible = loginform.CurrentUser.HasPermission("CanAccessProfiles");

            btnScheduling.Visible = loginform.CurrentUser.HasPermission("CanAccessScheduling");
            btnVisitorlog.Visible = loginform.CurrentUser.HasPermission("CanAccessVisitorLog");
        }

        private void LoadUserProfilePicture()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT ProfilePicture FROM Users WHERE Username = @username AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", loginform.CurrentUser.Username);
                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            byte[] imgBytes = (byte[])result;
                            using (MemoryStream ms = new MemoryStream(imgBytes))
                            {
                                ProfilePicDB.Image = Image.FromStream(ms);
                                ProfilePicDB.SizeMode = PictureBoxSizeMode.Zoom;
                                ProfilePicDB.BorderStyle = BorderStyle.FixedSingle;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile picture: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Message Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                loginform login = new loginform();
                login.Show();
                this.Hide();
            }
        }
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            dash.RefreshData();    
            ShowControl(dash);
        }

        private void btnMonthlydues_Click(object sender, EventArgs e)
        {
            dues.RefreshData();
            ShowControl(dues);
        }

        public void btnAnnouncement_Click(object sender, EventArgs e)
        {
            announce.RefreshData();
            ShowControl(announce);
        }

        public void btnScheduling_Click(object sender, EventArgs e)
        {
            sched.RefreshData();
            ShowControl(sched);
        }

        public void btnVisitorlog_Click(object sender, EventArgs e)
        {
            log.RefreshData();
            ShowControl(log);
        }

        private void btnRepositories_Click(object sender, EventArgs e)
        {
            repo.RefreshData();
            ShowControl(repo);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            homeowners.RefreshData();
            ShowControl(homeowners);
        }

        private void ProfilePicDB_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void ShowAnnouncementForm()
        {
            ShowControl(announce);
        }
        public void OpenSchedulingTab(string tabName)
        {
            btnScheduling_Click(this, EventArgs.Empty); // show the panel
            if (sched == null) return;

            switch (tabName)
            {
                case "Event":
                    sched.ShowEventsTab();
                    break;
                case "Garbage":
                    sched.ShowGarbageScheduleTab();
                    break;
            }
        }
    }
}
