namespace RECOMANAGESYS
{
    partial class BackupRestoreManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupRestoreManager));
            this.clbTables = new System.Windows.Forms.CheckedListBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnSelectPath = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRestoreHint = new System.Windows.Forms.Label();
            this.lblBackUpHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // clbTables
            // 
            this.clbTables.FormattingEnabled = true;
            this.clbTables.Location = new System.Drawing.Point(116, 202);
            this.clbTables.Name = "clbTables";
            this.clbTables.Size = new System.Drawing.Size(203, 211);
            this.clbTables.TabIndex = 0;
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.MediumTurquoise;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackup.Location = new System.Drawing.Point(116, 550);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(126, 46);
            this.btnBackup.TabIndex = 1;
            this.btnBackup.Text = "Back Up";
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click_1);
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.Location = new System.Drawing.Point(116, 620);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(126, 47);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnSelectPath
            // 
            this.btnSelectPath.Location = new System.Drawing.Point(116, 419);
            this.btnSelectPath.Name = "btnSelectPath";
            this.btnSelectPath.Size = new System.Drawing.Size(147, 41);
            this.btnSelectPath.TabIndex = 3;
            this.btnSelectPath.Text = "Select Path:";
            this.btnSelectPath.UseVisualStyleBackColor = true;
            this.btnSelectPath.Click += new System.EventHandler(this.btnSelectPath_Click);
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Location = new System.Drawing.Point(116, 466);
            this.txtBackupPath.Multiline = true;
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(203, 64);
            this.txtBackupPath.TabIndex = 4;
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(545, 202);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(350, 511);
            this.rtbLog.TabIndex = 5;
            this.rtbLog.Text = "";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(194, 690);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(303, 23);
            this.progressBar.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(602, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Back up and Restore Log";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(112, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Choose file/s to back up:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(112, 690);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Progress:";
            // 
            // lblRestoreHint
            // 
            this.lblRestoreHint.AutoSize = true;
            this.lblRestoreHint.BackColor = System.Drawing.Color.Transparent;
            this.lblRestoreHint.ForeColor = System.Drawing.Color.Red;
            this.lblRestoreHint.Location = new System.Drawing.Point(265, 627);
            this.lblRestoreHint.Name = "lblRestoreHint";
            this.lblRestoreHint.Size = new System.Drawing.Size(173, 40);
            this.lblRestoreHint.TabIndex = 10;
            this.lblRestoreHint.Text = "  Select a folder using \r\n\'Select Path\' to restore.";
            this.lblRestoreHint.Visible = false;
            // 
            // lblBackUpHint
            // 
            this.lblBackUpHint.AutoSize = true;
            this.lblBackUpHint.BackColor = System.Drawing.Color.Transparent;
            this.lblBackUpHint.ForeColor = System.Drawing.Color.Red;
            this.lblBackUpHint.Location = new System.Drawing.Point(265, 556);
            this.lblBackUpHint.Name = "lblBackUpHint";
            this.lblBackUpHint.Size = new System.Drawing.Size(253, 40);
            this.lblBackUpHint.TabIndex = 11;
            this.lblBackUpHint.Text = " Select a folder using \'Select Path\' \r\n           to save the back-up.";
            this.lblBackUpHint.Visible = false;
            // 
            // BackupRestoreManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(978, 841);
            this.Controls.Add(this.lblBackUpHint);
            this.Controls.Add(this.lblRestoreHint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.txtBackupPath);
            this.Controls.Add(this.btnSelectPath);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.clbTables);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BackupRestoreManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BackupRestoreManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbTables;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnSelectPath;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRestoreHint;
        private System.Windows.Forms.Label lblBackUpHint;
    }
}