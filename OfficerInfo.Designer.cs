namespace RECOMANAGESYS
{
    partial class OfficerInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfficerInfo));
            this.officerPanel = new System.Windows.Forms.Panel();
            this.viewLockAccounts = new System.Windows.Forms.Button();
            this.Editbtn = new System.Windows.Forms.Button();
            this.registerbtn = new System.Windows.Forms.Button();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.Deletebtn = new System.Windows.Forms.Button();
            this.Refreshbtn = new System.Windows.Forms.Button();
            this.DGVOfficers = new System.Windows.Forms.DataGridView();
            this.officerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVOfficers)).BeginInit();
            this.SuspendLayout();
            // 
            // officerPanel
            // 
            this.officerPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.officerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.officerPanel.Controls.Add(this.viewLockAccounts);
            this.officerPanel.Controls.Add(this.Editbtn);
            this.officerPanel.Controls.Add(this.registerbtn);
            this.officerPanel.Controls.Add(this.searchbtn);
            this.officerPanel.Controls.Add(this.txtSearch);
            this.officerPanel.Controls.Add(this.Deletebtn);
            this.officerPanel.Controls.Add(this.Refreshbtn);
            this.officerPanel.Controls.Add(this.DGVOfficers);
            this.officerPanel.Location = new System.Drawing.Point(35, 50);
            this.officerPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.officerPanel.Name = "officerPanel";
            this.officerPanel.Size = new System.Drawing.Size(1192, 751);
            this.officerPanel.TabIndex = 4;
            this.officerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.officerPanel_Paint);
            // 
            // viewLockAccounts
            // 
            this.viewLockAccounts.Location = new System.Drawing.Point(649, 98);
            this.viewLockAccounts.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.viewLockAccounts.Name = "viewLockAccounts";
            this.viewLockAccounts.Size = new System.Drawing.Size(170, 46);
            this.viewLockAccounts.TabIndex = 14;
            this.viewLockAccounts.Text = "Lock Accounts";
            this.viewLockAccounts.UseVisualStyleBackColor = true;
            this.viewLockAccounts.Click += new System.EventHandler(this.viewLockAccounts_Click);
            // 
            // Editbtn
            // 
            this.Editbtn.Location = new System.Drawing.Point(190, 81);
            this.Editbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Editbtn.Name = "Editbtn";
            this.Editbtn.Size = new System.Drawing.Size(110, 44);
            this.Editbtn.TabIndex = 13;
            this.Editbtn.Text = "Edit";
            this.Editbtn.UseVisualStyleBackColor = true;
            this.Editbtn.Click += new System.EventHandler(this.Editbtn_Click);
            // 
            // registerbtn
            // 
            this.registerbtn.Location = new System.Drawing.Point(843, 98);
            this.registerbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.registerbtn.Name = "registerbtn";
            this.registerbtn.Size = new System.Drawing.Size(326, 46);
            this.registerbtn.TabIndex = 12;
            this.registerbtn.Text = "Register Officer Account";
            this.registerbtn.UseVisualStyleBackColor = true;
            this.registerbtn.Click += new System.EventHandler(this.registerbtn_Click);
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(1113, 31);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(56, 48);
            this.searchbtn.TabIndex = 11;
            this.searchbtn.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(772, 31);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(397, 47);
            this.txtSearch.TabIndex = 10;
            // 
            // Deletebtn
            // 
            this.Deletebtn.Location = new System.Drawing.Point(332, 81);
            this.Deletebtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(125, 44);
            this.Deletebtn.TabIndex = 1;
            this.Deletebtn.Text = "Delete";
            this.Deletebtn.UseVisualStyleBackColor = true;
            this.Deletebtn.Click += new System.EventHandler(this.Deletebtn_Click);
            // 
            // Refreshbtn
            // 
            this.Refreshbtn.Location = new System.Drawing.Point(48, 81);
            this.Refreshbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Refreshbtn.Name = "Refreshbtn";
            this.Refreshbtn.Size = new System.Drawing.Size(110, 44);
            this.Refreshbtn.TabIndex = 0;
            this.Refreshbtn.Text = "Refresh";
            this.Refreshbtn.UseVisualStyleBackColor = true;
            this.Refreshbtn.Click += new System.EventHandler(this.Refreshbtn_Click);
            // 
            // DGVOfficers
            // 
            this.DGVOfficers.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.DGVOfficers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVOfficers.Location = new System.Drawing.Point(21, 164);
            this.DGVOfficers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DGVOfficers.Name = "DGVOfficers";
            this.DGVOfficers.RowHeadersWidth = 51;
            this.DGVOfficers.RowTemplate.Height = 24;
            this.DGVOfficers.Size = new System.Drawing.Size(1149, 559);
            this.DGVOfficers.TabIndex = 8;
            // 
            // OfficerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1278, 841);
            this.Controls.Add(this.officerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "OfficerInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.OfficerInfo_Load);
            this.officerPanel.ResumeLayout(false);
            this.officerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVOfficers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel officerPanel;
        private System.Windows.Forms.Button Editbtn;
        private System.Windows.Forms.Button registerbtn;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button Deletebtn;
        private System.Windows.Forms.Button Refreshbtn;
        private System.Windows.Forms.DataGridView DGVOfficers;
        private System.Windows.Forms.Button viewLockAccounts;
    }
}