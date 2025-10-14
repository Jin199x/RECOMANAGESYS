namespace RECOMANAGESYS
{
    partial class UpdateMonthlyDues
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMonthlyDues));
            this.lblUnitAddress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cancelvisitor = new System.Windows.Forms.Button();
            this.lblRemainingDebt = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.lblResidentName = new System.Windows.Forms.Label();
            this.clbMissedMonths = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.btnSelectHomeowner = new System.Windows.Forms.Button();
            this.txtHomeownerIDDisplay = new System.Windows.Forms.TextBox();
            this.lblNames = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbNames = new System.Windows.Forms.ComboBox();
            this.cmbResidency = new System.Windows.Forms.ComboBox();
            this.cmbChange = new System.Windows.Forms.ComboBox();
            this.cmbPaid = new System.Windows.Forms.ComboBox();
            this.cmbRemarks = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnToggleSelectAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUnitAddress
            // 
            this.lblUnitAddress.AutoSize = true;
            this.lblUnitAddress.BackColor = System.Drawing.Color.Transparent;
            this.lblUnitAddress.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnitAddress.Location = new System.Drawing.Point(228, 408);
            this.lblUnitAddress.Name = "lblUnitAddress";
            this.lblUnitAddress.Size = new System.Drawing.Size(118, 27);
            this.lblUnitAddress.TabIndex = 2;
            this.lblUnitAddress.Text = "[ADDRESS]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(719, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 31);
            this.label3.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(106, 719);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 27);
            this.label5.TabIndex = 6;
            this.label5.Text = "PAYMENT DATE:";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(979, 743);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 52);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Update";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.savevisitor_Click);
            // 
            // cancelvisitor
            // 
            this.cancelvisitor.FlatAppearance.BorderColor = System.Drawing.SystemColors.InfoText;
            this.cancelvisitor.FlatAppearance.BorderSize = 0;
            this.cancelvisitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelvisitor.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelvisitor.ForeColor = System.Drawing.Color.Black;
            this.cancelvisitor.Location = new System.Drawing.Point(823, 743);
            this.cancelvisitor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cancelvisitor.Name = "cancelvisitor";
            this.cancelvisitor.Size = new System.Drawing.Size(112, 52);
            this.cancelvisitor.TabIndex = 15;
            this.cancelvisitor.Text = "Cancel";
            this.cancelvisitor.UseVisualStyleBackColor = true;
            this.cancelvisitor.Click += new System.EventHandler(this.cancelvisitor_Click);
            // 
            // lblRemainingDebt
            // 
            this.lblRemainingDebt.AutoSize = true;
            this.lblRemainingDebt.BackColor = System.Drawing.Color.Transparent;
            this.lblRemainingDebt.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemainingDebt.Location = new System.Drawing.Point(225, 454);
            this.lblRemainingDebt.Name = "lblRemainingDebt";
            this.lblRemainingDebt.Size = new System.Drawing.Size(117, 27);
            this.lblRemainingDebt.TabIndex = 13;
            this.lblRemainingDebt.Text = "[BALANCE]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(90, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(135, 27);
            this.label6.TabIndex = 12;
            this.label6.Text = "Resident ID:";
            // 
            // dtpPaymentDate
            // 
            this.dtpPaymentDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpPaymentDate.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpPaymentDate.Location = new System.Drawing.Point(323, 712);
            this.dtpPaymentDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.Size = new System.Drawing.Size(361, 34);
            this.dtpPaymentDate.TabIndex = 11;
            // 
            // lblResidentName
            // 
            this.lblResidentName.AutoSize = true;
            this.lblResidentName.BackColor = System.Drawing.Color.Transparent;
            this.lblResidentName.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResidentName.Location = new System.Drawing.Point(224, 356);
            this.lblResidentName.Name = "lblResidentName";
            this.lblResidentName.Size = new System.Drawing.Size(170, 27);
            this.lblResidentName.TabIndex = 17;
            this.lblResidentName.Text = "[Resident Name]";
            // 
            // clbMissedMonths
            // 
            this.clbMissedMonths.BackColor = System.Drawing.SystemColors.ControlLight;
            this.clbMissedMonths.FormattingEnabled = true;
            this.clbMissedMonths.Location = new System.Drawing.Point(243, 503);
            this.clbMissedMonths.Name = "clbMissedMonths";
            this.clbMissedMonths.Size = new System.Drawing.Size(229, 142);
            this.clbMissedMonths.TabIndex = 18;
            this.clbMissedMonths.SelectedIndexChanged += new System.EventHandler(this.clbMissedMonths_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(92, 454);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 27);
            this.label1.TabIndex = 20;
            this.label1.Text = "Balance:";
            // 
            // cmbUnits
            // 
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(231, 295);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(121, 28);
            this.cmbUnits.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(90, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 27);
            this.label2.TabIndex = 22;
            this.label2.Text = "Unit #:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(92, 501);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 27);
            this.label4.TabIndex = 23;
            this.label4.Text = "Unpaid:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(106, 653);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 27);
            this.label7.TabIndex = 24;
            this.label7.Text = "Auto Pay:";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalAmount.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAmount.Location = new System.Drawing.Point(246, 653);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(190, 27);
            this.lblTotalAmount.TabIndex = 25;
            this.lblTotalAmount.Text = "[pay total amount]";
            // 
            // btnSelectHomeowner
            // 
            this.btnSelectHomeowner.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnSelectHomeowner.FlatAppearance.BorderSize = 0;
            this.btnSelectHomeowner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectHomeowner.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectHomeowner.ForeColor = System.Drawing.Color.White;
            this.btnSelectHomeowner.Location = new System.Drawing.Point(383, 173);
            this.btnSelectHomeowner.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectHomeowner.Name = "btnSelectHomeowner";
            this.btnSelectHomeowner.Size = new System.Drawing.Size(212, 35);
            this.btnSelectHomeowner.TabIndex = 26;
            this.btnSelectHomeowner.Text = "Select Homeowner...";
            this.btnSelectHomeowner.UseVisualStyleBackColor = false;
            // 
            // txtHomeownerIDDisplay
            // 
            this.txtHomeownerIDDisplay.Location = new System.Drawing.Point(231, 178);
            this.txtHomeownerIDDisplay.Multiline = true;
            this.txtHomeownerIDDisplay.Name = "txtHomeownerIDDisplay";
            this.txtHomeownerIDDisplay.ReadOnly = true;
            this.txtHomeownerIDDisplay.Size = new System.Drawing.Size(120, 37);
            this.txtHomeownerIDDisplay.TabIndex = 27;
            this.txtHomeownerIDDisplay.TabStop = false;
            // 
            // lblNames
            // 
            this.lblNames.AutoSize = true;
            this.lblNames.BackColor = System.Drawing.Color.Transparent;
            this.lblNames.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNames.Location = new System.Drawing.Point(363, 242);
            this.lblNames.Name = "lblNames";
            this.lblNames.Size = new System.Drawing.Size(145, 27);
            this.lblNames.TabIndex = 55;
            this.lblNames.Text = "Select Name:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(90, 242);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(119, 27);
            this.label13.TabIndex = 54;
            this.label13.Text = "Residency:";
            // 
            // cmbNames
            // 
            this.cmbNames.FormattingEnabled = true;
            this.cmbNames.Location = new System.Drawing.Point(514, 239);
            this.cmbNames.Name = "cmbNames";
            this.cmbNames.Size = new System.Drawing.Size(148, 28);
            this.cmbNames.TabIndex = 53;
            // 
            // cmbResidency
            // 
            this.cmbResidency.FormattingEnabled = true;
            this.cmbResidency.Location = new System.Drawing.Point(229, 239);
            this.cmbResidency.Name = "cmbResidency";
            this.cmbResidency.Size = new System.Drawing.Size(121, 28);
            this.cmbResidency.TabIndex = 52;
            // 
            // cmbChange
            // 
            this.cmbChange.FormattingEnabled = true;
            this.cmbChange.Location = new System.Drawing.Point(913, 503);
            this.cmbChange.Name = "cmbChange";
            this.cmbChange.Size = new System.Drawing.Size(121, 28);
            this.cmbChange.TabIndex = 61;
            // 
            // cmbPaid
            // 
            this.cmbPaid.FormattingEnabled = true;
            this.cmbPaid.Location = new System.Drawing.Point(913, 458);
            this.cmbPaid.Name = "cmbPaid";
            this.cmbPaid.Size = new System.Drawing.Size(121, 28);
            this.cmbPaid.TabIndex = 60;
            // 
            // cmbRemarks
            // 
            this.cmbRemarks.FormattingEnabled = true;
            this.cmbRemarks.Location = new System.Drawing.Point(913, 558);
            this.cmbRemarks.Name = "cmbRemarks";
            this.cmbRemarks.Size = new System.Drawing.Size(121, 28);
            this.cmbRemarks.TabIndex = 59;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(782, 561);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 20);
            this.label11.TabIndex = 58;
            this.label11.Text = "Remarks:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(782, 511);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 20);
            this.label10.TabIndex = 57;
            this.label10.Text = "Change given:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(782, 461);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 20);
            this.label8.TabIndex = 56;
            this.label8.Text = "Amount Paid:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(90, 356);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 27);
            this.label9.TabIndex = 62;
            this.label9.Text = "Name:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(90, 408);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 27);
            this.label12.TabIndex = 63;
            this.label12.Text = "Address:";
            // 
            // btnToggleSelectAll
            // 
            this.btnToggleSelectAll.Location = new System.Drawing.Point(478, 501);
            this.btnToggleSelectAll.Name = "btnToggleSelectAll";
            this.btnToggleSelectAll.Size = new System.Drawing.Size(112, 29);
            this.btnToggleSelectAll.TabIndex = 64;
            this.btnToggleSelectAll.Text = "Select All";
            this.btnToggleSelectAll.UseVisualStyleBackColor = true;
            // 
            // UpdateMonthlyDues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1251, 882);
            this.Controls.Add(this.btnToggleSelectAll);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbChange);
            this.Controls.Add(this.cmbPaid);
            this.Controls.Add(this.cmbRemarks);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblNames);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cmbNames);
            this.Controls.Add(this.cmbResidency);
            this.Controls.Add(this.txtHomeownerIDDisplay);
            this.Controls.Add(this.btnSelectHomeowner);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbUnits);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clbMissedMonths);
            this.Controls.Add(this.lblResidentName);
            this.Controls.Add(this.cancelvisitor);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblRemainingDebt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dtpPaymentDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblUnitAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "UpdateMonthlyDues";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.UpdateMonthlyDues_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblUnitAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button cancelvisitor;
        private System.Windows.Forms.Label lblRemainingDebt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpPaymentDate;
        private System.Windows.Forms.Label lblResidentName;
        private System.Windows.Forms.CheckedListBox clbMissedMonths;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Button btnSelectHomeowner;
        private System.Windows.Forms.TextBox txtHomeownerIDDisplay;
        private System.Windows.Forms.Label lblNames;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbNames;
        private System.Windows.Forms.ComboBox cmbResidency;
        private System.Windows.Forms.ComboBox cmbChange;
        private System.Windows.Forms.ComboBox cmbPaid;
        private System.Windows.Forms.ComboBox cmbRemarks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnToggleSelectAll;
    }
}