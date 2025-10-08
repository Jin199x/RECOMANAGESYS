namespace RECOMANAGESYS
{
    partial class dashboardControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dashboardControl));
            this.panelQuickAnnouncements = new System.Windows.Forms.Panel();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.btnNotif = new FontAwesome.Sharp.IconButton();
            this.lblAnnounce = new System.Windows.Forms.Label();
            this.lblNotifCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.visitorShowPanel = new System.Windows.Forms.Panel();
            this.lblTodayVisitors = new System.Windows.Forms.Label();
            this.lvVisitor = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSchedule = new System.Windows.Forms.Panel();
            this.panelGarbageDisplay = new System.Windows.Forms.Panel();
            this.lblNextDay1 = new System.Windows.Forms.Label();
            this.lblNextTime1 = new System.Windows.Forms.Label();
            this.lblNextDay2 = new System.Windows.Forms.Label();
            this.lblNextTime2 = new System.Windows.Forms.Label();
            this.visitorShowPanel.SuspendLayout();
            this.panelGarbageDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelQuickAnnouncements
            // 
            this.panelQuickAnnouncements.AutoScroll = true;
            this.panelQuickAnnouncements.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelQuickAnnouncements.Location = new System.Drawing.Point(104, 221);
            this.panelQuickAnnouncements.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelQuickAnnouncements.Name = "panelQuickAnnouncements";
            this.panelQuickAnnouncements.Size = new System.Drawing.Size(1047, 290);
            this.panelQuickAnnouncements.TabIndex = 1;
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.BackColor = System.Drawing.Color.Transparent;
            this.lblGreeting.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.Location = new System.Drawing.Point(723, 62);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(202, 32);
            this.lblGreeting.TabIndex = 5;
            this.lblGreeting.Text = "Greeting Label";
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.BackColor = System.Drawing.Color.Transparent;
            this.lblDateTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(723, 118);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(215, 32);
            this.lblDateTime.TabIndex = 6;
            this.lblDateTime.Text = "TimeDate Label";
            this.lblDateTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnNotif
            // 
            this.btnNotif.BackColor = System.Drawing.Color.Transparent;
            this.btnNotif.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnNotif.BackgroundImage")));
            this.btnNotif.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNotif.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotif.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotif.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnNotif.IconColor = System.Drawing.Color.Black;
            this.btnNotif.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNotif.Location = new System.Drawing.Point(1141, 60);
            this.btnNotif.Name = "btnNotif";
            this.btnNotif.Size = new System.Drawing.Size(48, 45);
            this.btnNotif.TabIndex = 7;
            this.btnNotif.UseVisualStyleBackColor = false;
            this.btnNotif.Click += new System.EventHandler(this.btnNotif_Click);
            // 
            // lblAnnounce
            // 
            this.lblAnnounce.AutoSize = true;
            this.lblAnnounce.BackColor = System.Drawing.Color.Transparent;
            this.lblAnnounce.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnnounce.Location = new System.Drawing.Point(98, 187);
            this.lblAnnounce.Name = "lblAnnounce";
            this.lblAnnounce.Size = new System.Drawing.Size(300, 32);
            this.lblAnnounce.TabIndex = 8;
            this.lblAnnounce.Text = "Quick Announcements";
            this.lblAnnounce.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblNotifCount
            // 
            this.lblNotifCount.AutoSize = true;
            this.lblNotifCount.BackColor = System.Drawing.Color.Transparent;
            this.lblNotifCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotifCount.Location = new System.Drawing.Point(1165, 51);
            this.lblNotifCount.Name = "lblNotifCount";
            this.lblNotifCount.Size = new System.Drawing.Size(23, 25);
            this.lblNotifCount.TabIndex = 10;
            this.lblNotifCount.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(98, 570);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 32);
            this.label1.TabIndex = 10;
            this.label1.Text = "Visitors Today";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(469, 570);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 32);
            this.label2.TabIndex = 11;
            this.label2.Text = "Scheduled Events";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(840, 570);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(252, 32);
            this.label3.TabIndex = 12;
            this.label3.Text = "Garbage Schedule";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // visitorShowPanel
            // 
            this.visitorShowPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.visitorShowPanel.Controls.Add(this.label4);
            this.visitorShowPanel.Controls.Add(this.lvVisitor);
            this.visitorShowPanel.Controls.Add(this.lblTodayVisitors);
            this.visitorShowPanel.Location = new System.Drawing.Point(104, 613);
            this.visitorShowPanel.Name = "visitorShowPanel";
            this.visitorShowPanel.Size = new System.Drawing.Size(294, 217);
            this.visitorShowPanel.TabIndex = 13;
            // 
            // lblTodayVisitors
            // 
            this.lblTodayVisitors.AutoSize = true;
            this.lblTodayVisitors.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTodayVisitors.Location = new System.Drawing.Point(71, 25);
            this.lblTodayVisitors.Name = "lblTodayVisitors";
            this.lblTodayVisitors.Size = new System.Drawing.Size(72, 52);
            this.lblTodayVisitors.TabIndex = 0;
            this.lblTodayVisitors.Text = "00";
            // 
            // lvVisitor
            // 
            this.lvVisitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvVisitor.HideSelection = false;
            this.lvVisitor.Location = new System.Drawing.Point(3, 97);
            this.lvVisitor.Name = "lvVisitor";
            this.lvVisitor.Size = new System.Drawing.Size(288, 120);
            this.lvVisitor.TabIndex = 1;
            this.lvVisitor.UseCompatibleStateImageBehavior = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(149, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "Visitors";
            // 
            // panelSchedule
            // 
            this.panelSchedule.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelSchedule.Location = new System.Drawing.Point(475, 613);
            this.panelSchedule.Name = "panelSchedule";
            this.panelSchedule.Size = new System.Drawing.Size(294, 217);
            this.panelSchedule.TabIndex = 14;
            // 
            // panelGarbageDisplay
            // 
            this.panelGarbageDisplay.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelGarbageDisplay.Controls.Add(this.lblNextTime2);
            this.panelGarbageDisplay.Controls.Add(this.lblNextDay2);
            this.panelGarbageDisplay.Controls.Add(this.lblNextTime1);
            this.panelGarbageDisplay.Controls.Add(this.lblNextDay1);
            this.panelGarbageDisplay.Location = new System.Drawing.Point(846, 613);
            this.panelGarbageDisplay.Name = "panelGarbageDisplay";
            this.panelGarbageDisplay.Size = new System.Drawing.Size(294, 217);
            this.panelGarbageDisplay.TabIndex = 15;
            // 
            // lblNextDay1
            // 
            this.lblNextDay1.AutoSize = true;
            this.lblNextDay1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextDay1.Location = new System.Drawing.Point(15, 25);
            this.lblNextDay1.Name = "lblNextDay1";
            this.lblNextDay1.Size = new System.Drawing.Size(105, 40);
            this.lblNextDay1.TabIndex = 3;
            this.lblNextDay1.Text = "Day1";
            // 
            // lblNextTime1
            // 
            this.lblNextTime1.AutoSize = true;
            this.lblNextTime1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextTime1.Location = new System.Drawing.Point(17, 65);
            this.lblNextTime1.Name = "lblNextTime1";
            this.lblNextTime1.Size = new System.Drawing.Size(72, 25);
            this.lblNextTime1.TabIndex = 3;
            this.lblNextTime1.Text = "Time1";
            // 
            // lblNextDay2
            // 
            this.lblNextDay2.AutoSize = true;
            this.lblNextDay2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextDay2.Location = new System.Drawing.Point(15, 111);
            this.lblNextDay2.Name = "lblNextDay2";
            this.lblNextDay2.Size = new System.Drawing.Size(105, 40);
            this.lblNextDay2.TabIndex = 4;
            this.lblNextDay2.Text = "Day2";
            // 
            // lblNextTime2
            // 
            this.lblNextTime2.AutoSize = true;
            this.lblNextTime2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextTime2.Location = new System.Drawing.Point(17, 163);
            this.lblNextTime2.Name = "lblNextTime2";
            this.lblNextTime2.Size = new System.Drawing.Size(72, 25);
            this.lblNextTime2.TabIndex = 5;
            this.lblNextTime2.Text = "Time2";
            // 
            // dashboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panelGarbageDisplay);
            this.Controls.Add(this.visitorShowPanel);
            this.Controls.Add(this.lblNotifCount);
            this.Controls.Add(this.lblAnnounce);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnNotif);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblGreeting);
            this.Controls.Add(this.panelQuickAnnouncements);
            this.Controls.Add(this.panelSchedule);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "dashboardControl";
            this.Size = new System.Drawing.Size(1249, 920);
            this.Load += new System.EventHandler(this.dashboardControl_Load);
            this.visitorShowPanel.ResumeLayout(false);
            this.visitorShowPanel.PerformLayout();
            this.panelGarbageDisplay.ResumeLayout(false);
            this.panelGarbageDisplay.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelQuickAnnouncements;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Label lblDateTime;
        private FontAwesome.Sharp.IconButton btnNotif;
        private System.Windows.Forms.Label lblAnnounce;
        private System.Windows.Forms.Label lblNotifCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel visitorShowPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView lvVisitor;
        private System.Windows.Forms.Label lblTodayVisitors;
        private System.Windows.Forms.Panel panelSchedule;
        private System.Windows.Forms.Panel panelGarbageDisplay;
        private System.Windows.Forms.Label lblNextTime2;
        private System.Windows.Forms.Label lblNextDay2;
        private System.Windows.Forms.Label lblNextTime1;
        private System.Windows.Forms.Label lblNextDay1;
    }
}
