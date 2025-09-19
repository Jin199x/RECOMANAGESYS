namespace RECOMANAGESYS
{
    partial class UnlockAccountsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnlockAccountsForm));
            this.dgvLockedAccounts = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchLock = new System.Windows.Forms.TextBox();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.btnUnlock = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLockedAccounts)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvLockedAccounts
            // 
            this.dgvLockedAccounts.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvLockedAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLockedAccounts.Location = new System.Drawing.Point(21, 192);
            this.dgvLockedAccounts.Name = "dgvLockedAccounts";
            this.dgvLockedAccounts.RowHeadersWidth = 62;
            this.dgvLockedAccounts.RowTemplate.Height = 28;
            this.dgvLockedAccounts.Size = new System.Drawing.Size(1133, 460);
            this.dgvLockedAccounts.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnUnlock);
            this.panel1.Controls.Add(this.searchbtn);
            this.panel1.Controls.Add(this.searchLock);
            this.panel1.Controls.Add(this.dgvLockedAccounts);
            this.panel1.Location = new System.Drawing.Point(52, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1175, 671);
            this.panel1.TabIndex = 1;
            // 
            // searchLock
            // 
            this.searchLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchLock.Location = new System.Drawing.Point(757, 40);
            this.searchLock.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchLock.Multiline = true;
            this.searchLock.Name = "searchLock";
            this.searchLock.Size = new System.Drawing.Size(397, 47);
            this.searchLock.TabIndex = 11;
            this.searchLock.TextChanged += new System.EventHandler(this.searchLock_TextChanged);
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(1098, 40);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(56, 48);
            this.searchbtn.TabIndex = 12;
            this.searchbtn.UseVisualStyleBackColor = true;
            // 
            // btnUnlock
            // 
            this.btnUnlock.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnUnlock.FlatAppearance.BorderSize = 0;
            this.btnUnlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnlock.ForeColor = System.Drawing.Color.White;
            this.btnUnlock.Location = new System.Drawing.Point(757, 117);
            this.btnUnlock.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnlock.Name = "btnUnlock";
            this.btnUnlock.Size = new System.Drawing.Size(397, 48);
            this.btnUnlock.TabIndex = 13;
            this.btnUnlock.Text = "Unlock Account";
            this.btnUnlock.UseVisualStyleBackColor = false;
            this.btnUnlock.Click += new System.EventHandler(this.btnUnlock_Click);
            // 
            // UnlockAccountsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1278, 841);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UnlockAccountsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UnlockAccountsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLockedAccounts)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLockedAccounts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox searchLock;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.Button btnUnlock;
    }
}