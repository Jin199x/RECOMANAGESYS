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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnToggleSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clbTables
            // 
            this.clbTables.FormattingEnabled = true;
            this.clbTables.Location = new System.Drawing.Point(190, 222);
            this.clbTables.Name = "clbTables";
            this.clbTables.Size = new System.Drawing.Size(396, 234);
            this.clbTables.TabIndex = 0;
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.MediumTurquoise;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackup.Location = new System.Drawing.Point(416, 557);
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
            this.btnRestore.Location = new System.Drawing.Point(591, 556);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(126, 47);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(277, 470);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(309, 23);
            this.progressBar.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(74, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 29);
            this.label2.TabIndex = 8;
            this.label2.Text = "Choose file/s to back up:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(186, 473);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Progress:";
            // 
            // btnToggleSelect
            // 
            this.btnToggleSelect.Location = new System.Drawing.Point(461, 190);
            this.btnToggleSelect.Name = "btnToggleSelect";
            this.btnToggleSelect.Size = new System.Drawing.Size(125, 26);
            this.btnToggleSelect.TabIndex = 12;
            this.btnToggleSelect.Text = "Select All";
            this.btnToggleSelect.UseVisualStyleBackColor = true;
            this.btnToggleSelect.Click += new System.EventHandler(this.btnToggleSelect_Click);
            // 
            // BackupRestoreManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(794, 678);
            this.Controls.Add(this.btnToggleSelect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.clbTables);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BackupRestoreManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbTables;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnToggleSelect;
    }
}