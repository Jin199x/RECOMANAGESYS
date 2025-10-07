namespace RECOMANAGESYS
{
    partial class dashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dashboard));
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.NavPanel = new System.Windows.Forms.Panel();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnRepositories = new System.Windows.Forms.Button();
            this.btnVisitorlog = new System.Windows.Forms.Button();
            this.btnScheduling = new System.Windows.Forms.Button();
            this.btnAnnouncement = new System.Windows.Forms.Button();
            this.btnMonthlydues = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.ProfilePicDB = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.NavPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProfilePicDB)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1560, 58);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(312, 58);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1248, 920);
            this.flowLayoutPanel1.TabIndex = 2;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(27, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(601, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Computerized Record Management System";
            // 
            // NavPanel
            // 
            this.NavPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("NavPanel.BackgroundImage")));
            this.NavPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.NavPanel.Controls.Add(this.btnProfile);
            this.NavPanel.Controls.Add(this.btnRepositories);
            this.NavPanel.Controls.Add(this.btnVisitorlog);
            this.NavPanel.Controls.Add(this.btnScheduling);
            this.NavPanel.Controls.Add(this.btnAnnouncement);
            this.NavPanel.Controls.Add(this.btnMonthlydues);
            this.NavPanel.Controls.Add(this.btnDashboard);
            this.NavPanel.Controls.Add(this.button1);
            this.NavPanel.Controls.Add(this.lblRole);
            this.NavPanel.Controls.Add(this.lblName);
            this.NavPanel.Controls.Add(this.ProfilePicDB);
            this.NavPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.NavPanel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NavPanel.ForeColor = System.Drawing.SystemColors.Control;
            this.NavPanel.Location = new System.Drawing.Point(0, 58);
            this.NavPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NavPanel.Name = "NavPanel";
            this.NavPanel.Size = new System.Drawing.Size(312, 920);
            this.NavPanel.TabIndex = 1;
            this.NavPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // btnProfile
            // 
            this.btnProfile.BackColor = System.Drawing.Color.Gainsboro;
            this.btnProfile.FlatAppearance.BorderSize = 0;
            this.btnProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfile.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProfile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnProfile.Location = new System.Drawing.Point(94, 709);
            this.btnProfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(199, 70);
            this.btnProfile.TabIndex = 4;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = false;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // btnRepositories
            // 
            this.btnRepositories.BackColor = System.Drawing.Color.Gainsboro;
            this.btnRepositories.FlatAppearance.BorderSize = 0;
            this.btnRepositories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRepositories.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRepositories.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRepositories.Location = new System.Drawing.Point(94, 621);
            this.btnRepositories.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRepositories.Name = "btnRepositories";
            this.btnRepositories.Size = new System.Drawing.Size(199, 75);
            this.btnRepositories.TabIndex = 4;
            this.btnRepositories.Text = "Document\r\nRepositories\r\n";
            this.btnRepositories.UseVisualStyleBackColor = false;
            this.btnRepositories.Click += new System.EventHandler(this.btnRepositories_Click);
            // 
            // btnVisitorlog
            // 
            this.btnVisitorlog.BackColor = System.Drawing.Color.Gainsboro;
            this.btnVisitorlog.FlatAppearance.BorderSize = 0;
            this.btnVisitorlog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisitorlog.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVisitorlog.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnVisitorlog.Location = new System.Drawing.Point(94, 542);
            this.btnVisitorlog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnVisitorlog.Name = "btnVisitorlog";
            this.btnVisitorlog.Size = new System.Drawing.Size(199, 75);
            this.btnVisitorlog.TabIndex = 4;
            this.btnVisitorlog.Text = "Visitor Log";
            this.btnVisitorlog.UseVisualStyleBackColor = false;
            this.btnVisitorlog.Click += new System.EventHandler(this.btnVisitorlog_Click);
            // 
            // btnScheduling
            // 
            this.btnScheduling.BackColor = System.Drawing.Color.Gainsboro;
            this.btnScheduling.FlatAppearance.BorderSize = 0;
            this.btnScheduling.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScheduling.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScheduling.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnScheduling.Location = new System.Drawing.Point(94, 461);
            this.btnScheduling.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnScheduling.Name = "btnScheduling";
            this.btnScheduling.Size = new System.Drawing.Size(199, 75);
            this.btnScheduling.TabIndex = 6;
            this.btnScheduling.Text = "Scheduling";
            this.btnScheduling.UseVisualStyleBackColor = false;
            this.btnScheduling.Click += new System.EventHandler(this.btnScheduling_Click);
            // 
            // btnAnnouncement
            // 
            this.btnAnnouncement.BackColor = System.Drawing.Color.Gainsboro;
            this.btnAnnouncement.FlatAppearance.BorderSize = 0;
            this.btnAnnouncement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnnouncement.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnnouncement.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAnnouncement.Location = new System.Drawing.Point(94, 385);
            this.btnAnnouncement.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAnnouncement.Name = "btnAnnouncement";
            this.btnAnnouncement.Size = new System.Drawing.Size(199, 65);
            this.btnAnnouncement.TabIndex = 5;
            this.btnAnnouncement.Text = "Announcements";
            this.btnAnnouncement.UseVisualStyleBackColor = false;
            this.btnAnnouncement.Click += new System.EventHandler(this.btnAnnouncement_Click);
            // 
            // btnMonthlydues
            // 
            this.btnMonthlydues.BackColor = System.Drawing.Color.Gainsboro;
            this.btnMonthlydues.FlatAppearance.BorderSize = 0;
            this.btnMonthlydues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonthlydues.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMonthlydues.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnMonthlydues.Location = new System.Drawing.Point(94, 306);
            this.btnMonthlydues.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMonthlydues.Name = "btnMonthlydues";
            this.btnMonthlydues.Size = new System.Drawing.Size(199, 75);
            this.btnMonthlydues.TabIndex = 4;
            this.btnMonthlydues.Text = "Monthly Dues";
            this.btnMonthlydues.UseVisualStyleBackColor = false;
            this.btnMonthlydues.Click += new System.EventHandler(this.btnMonthlydues_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.Gainsboro;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDashboard.Location = new System.Drawing.Point(94, 227);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(199, 75);
            this.btnDashboard.TabIndex = 3;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(33, 842);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(240, 62);
            this.button1.TabIndex = 0;
            this.button1.Text = "LOGOUT";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.BackColor = System.Drawing.Color.Transparent;
            this.lblRole.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblRole.Location = new System.Drawing.Point(75, 187);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(99, 28);
            this.lblRole.TabIndex = 0;
            this.lblRole.Text = "Position";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblName.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblName.Location = new System.Drawing.Point(75, 159);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(75, 28);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // ProfilePicDB
            // 
            this.ProfilePicDB.BackgroundImage = global::RECOMANAGESYS.Properties.Resources.Little_red_riding_hood__1_;
            this.ProfilePicDB.Image = global::RECOMANAGESYS.Properties.Resources.Little_red_riding_hood__1_;
            this.ProfilePicDB.Location = new System.Drawing.Point(80, 6);
            this.ProfilePicDB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ProfilePicDB.Name = "ProfilePicDB";
            this.ProfilePicDB.Size = new System.Drawing.Size(153, 149);
            this.ProfilePicDB.TabIndex = 0;
            this.ProfilePicDB.TabStop = false;
            this.ProfilePicDB.Click += new System.EventHandler(this.ProfilePicDB_Click);
            // 
            // dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1560, 978);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.NavPanel);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximumSize = new System.Drawing.Size(1560, 978);
            this.Name = "dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "dashboard";
            this.Load += new System.EventHandler(this.dashboard_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.NavPanel.ResumeLayout(false);
            this.NavPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProfilePicDB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel NavPanel;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.PictureBox ProfilePicDB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnRepositories;
        private System.Windows.Forms.Button btnVisitorlog;
        private System.Windows.Forms.Button btnScheduling;
        private System.Windows.Forms.Button btnAnnouncement;
        private System.Windows.Forms.Button btnMonthlydues;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label1;
    }
}