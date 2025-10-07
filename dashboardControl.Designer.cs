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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.btnNotif = new FontAwesome.Sharp.IconButton();
            this.lblAnnounce = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelQuickAnnouncements
            // 
            this.panelQuickAnnouncements.AutoScroll = true;
            this.panelQuickAnnouncements.Location = new System.Drawing.Point(104, 221);
            this.panelQuickAnnouncements.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelQuickAnnouncements.Name = "panelQuickAnnouncements";
            this.panelQuickAnnouncements.Size = new System.Drawing.Size(1047, 290);
            this.panelQuickAnnouncements.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(104, 564);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(294, 200);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Location = new System.Drawing.Point(482, 564);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(294, 200);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Location = new System.Drawing.Point(857, 564);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(294, 200);
            this.flowLayoutPanel3.TabIndex = 4;
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
            this.btnNotif.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnNotif.IconColor = System.Drawing.Color.Black;
            this.btnNotif.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNotif.Location = new System.Drawing.Point(1085, 150);
            this.btnNotif.Name = "btnNotif";
            this.btnNotif.Size = new System.Drawing.Size(66, 50);
            this.btnNotif.TabIndex = 7;
            this.btnNotif.UseVisualStyleBackColor = false;
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
            // dashboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.lblAnnounce);
            this.Controls.Add(this.btnNotif);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblGreeting);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panelQuickAnnouncements);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "dashboardControl";
            this.Size = new System.Drawing.Size(1249, 920);
            this.Load += new System.EventHandler(this.dashboardControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelQuickAnnouncements;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Label lblDateTime;
        private FontAwesome.Sharp.IconButton btnNotif;
        private System.Windows.Forms.Label lblAnnounce;
    }
}
