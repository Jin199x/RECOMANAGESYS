namespace RECOMANAGESYS
{
    partial class docurepo
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblResetFilter = new System.Windows.Forms.Label();
            this.flowBreadcrumb = new System.Windows.Forms.FlowLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDate = new System.Windows.Forms.Button();
            this.btnModified = new System.Windows.Forms.Button();
            this.btnType = new System.Windows.Forms.Button();
            this.panelDesktop = new System.Windows.Forms.Panel();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.searchDocu = new System.Windows.Forms.TextBox();
            this.buttonAddFolder = new System.Windows.Forms.Button();
            this.buttonAddFile = new System.Windows.Forms.Button();
            this.btnSafeguard = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(29, 41);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1192, 63);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblResetFilter);
            this.panel2.Controls.Add(this.flowBreadcrumb);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panelDesktop);
            this.panel2.Controls.Add(this.searchbtn);
            this.panel2.Controls.Add(this.searchDocu);
            this.panel2.Controls.Add(this.buttonAddFolder);
            this.panel2.Controls.Add(this.buttonAddFile);
            this.panel2.Controls.Add(this.btnSafeguard);
            this.panel2.Location = new System.Drawing.Point(29, 134);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1192, 751);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // lblResetFilter
            // 
            this.lblResetFilter.AutoSize = true;
            this.lblResetFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResetFilter.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblResetFilter.Location = new System.Drawing.Point(432, 142);
            this.lblResetFilter.Name = "lblResetFilter";
            this.lblResetFilter.Size = new System.Drawing.Size(91, 20);
            this.lblResetFilter.TabIndex = 16;
            this.lblResetFilter.Text = "Reset Filter";
            // 
            // flowBreadcrumb
            // 
            this.flowBreadcrumb.BackColor = System.Drawing.Color.Transparent;
            this.flowBreadcrumb.Location = new System.Drawing.Point(36, 100);
            this.flowBreadcrumb.Name = "flowBreadcrumb";
            this.flowBreadcrumb.Size = new System.Drawing.Size(523, 28);
            this.flowBreadcrumb.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.btnDate);
            this.panel3.Controls.Add(this.btnModified);
            this.panel3.Controls.Add(this.btnType);
            this.panel3.Location = new System.Drawing.Point(36, 131);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(390, 35);
            this.panel3.TabIndex = 14;
            // 
            // btnDate
            // 
            this.btnDate.Location = new System.Drawing.Point(244, 3);
            this.btnDate.Name = "btnDate";
            this.btnDate.Size = new System.Drawing.Size(143, 28);
            this.btnDate.TabIndex = 17;
            this.btnDate.Text = "Date Added";
            this.btnDate.UseVisualStyleBackColor = true;
            // 
            // btnModified
            // 
            this.btnModified.Location = new System.Drawing.Point(92, 3);
            this.btnModified.Name = "btnModified";
            this.btnModified.Size = new System.Drawing.Size(146, 28);
            this.btnModified.TabIndex = 16;
            this.btnModified.Text = "Last Modified";
            this.btnModified.UseVisualStyleBackColor = true;
            // 
            // btnType
            // 
            this.btnType.Location = new System.Drawing.Point(3, 3);
            this.btnType.Name = "btnType";
            this.btnType.Size = new System.Drawing.Size(83, 28);
            this.btnType.TabIndex = 15;
            this.btnType.Text = "Type";
            this.btnType.UseVisualStyleBackColor = true;
            // 
            // panelDesktop
            // 
            this.panelDesktop.Location = new System.Drawing.Point(36, 172);
            this.panelDesktop.Name = "panelDesktop";
            this.panelDesktop.Size = new System.Drawing.Size(1134, 554);
            this.panelDesktop.TabIndex = 12;
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(1102, 34);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(56, 48);
            this.searchbtn.TabIndex = 11;
            this.searchbtn.UseVisualStyleBackColor = true;
            this.searchbtn.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // searchDocu
            // 
            this.searchDocu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchDocu.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchDocu.Location = new System.Drawing.Point(762, 34);
            this.searchDocu.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchDocu.Multiline = true;
            this.searchDocu.Name = "searchDocu";
            this.searchDocu.Size = new System.Drawing.Size(397, 47);
            this.searchDocu.TabIndex = 10;
            this.searchDocu.TextChanged += new System.EventHandler(this.searchDocu_TextChanged);
            // 
            // buttonAddFolder
            // 
            this.buttonAddFolder.BackColor = System.Drawing.SystemColors.HotTrack;
            this.buttonAddFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonAddFolder.FlatAppearance.BorderSize = 0;
            this.buttonAddFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddFolder.ForeColor = System.Drawing.Color.White;
            this.buttonAddFolder.Location = new System.Drawing.Point(211, 32);
            this.buttonAddFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAddFolder.Name = "buttonAddFolder";
            this.buttonAddFolder.Size = new System.Drawing.Size(169, 48);
            this.buttonAddFolder.TabIndex = 9;
            this.buttonAddFolder.Text = "Add Folder";
            this.buttonAddFolder.UseVisualStyleBackColor = false;
            this.buttonAddFolder.Click += new System.EventHandler(this.buttonAddFolder_Click);
            // 
            // buttonAddFile
            // 
            this.buttonAddFile.BackColor = System.Drawing.SystemColors.HotTrack;
            this.buttonAddFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonAddFile.FlatAppearance.BorderSize = 0;
            this.buttonAddFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddFile.ForeColor = System.Drawing.Color.White;
            this.buttonAddFile.Location = new System.Drawing.Point(36, 32);
            this.buttonAddFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAddFile.Name = "buttonAddFile";
            this.buttonAddFile.Size = new System.Drawing.Size(169, 48);
            this.buttonAddFile.TabIndex = 7;
            this.buttonAddFile.Text = "Add File";
            this.buttonAddFile.UseVisualStyleBackColor = false;
            this.buttonAddFile.Click += new System.EventHandler(this.buttonAddFile_Click);
            // 
            // btnSafeguard
            // 
            this.btnSafeguard.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnSafeguard.FlatAppearance.BorderSize = 0;
            this.btnSafeguard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSafeguard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSafeguard.ForeColor = System.Drawing.Color.White;
            this.btnSafeguard.Location = new System.Drawing.Point(869, 100);
            this.btnSafeguard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSafeguard.Name = "btnSafeguard";
            this.btnSafeguard.Size = new System.Drawing.Size(286, 48);
            this.btnSafeguard.TabIndex = 5;
            this.btnSafeguard.Text = "Backup Restore Manager";
            this.btnSafeguard.UseVisualStyleBackColor = false;
            this.btnSafeguard.Click += new System.EventHandler(this.btnSafeguard_Click);
            // 
            // docurepo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "docurepo";
            this.Size = new System.Drawing.Size(1249, 920);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSafeguard;
        private System.Windows.Forms.Button buttonAddFile;
        private System.Windows.Forms.Button buttonAddFolder;
        private System.Windows.Forms.TextBox searchDocu;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.Panel panelDesktop;
        private System.Windows.Forms.Button btnDate;
        private System.Windows.Forms.Button btnModified;
        private System.Windows.Forms.Button btnType;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.FlowLayoutPanel flowBreadcrumb;
        private System.Windows.Forms.Label lblResetFilter;
    }
}
