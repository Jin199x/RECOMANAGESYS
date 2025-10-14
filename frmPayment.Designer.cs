namespace RECOMANAGESYS
{
    partial class frmPayment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPayment));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblResidentName = new System.Windows.Forms.Label();
            this.lblResidentAddress = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.lblUnits = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUnitStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblDueRate = new System.Windows.Forms.Label();
            this.lblMonthCovered = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblAmountPaid = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbRemarks = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbPaid = new System.Windows.Forms.ComboBox();
            this.cmbChange = new System.Windows.Forms.ComboBox();
            this.cmbResidency = new System.Windows.Forms.ComboBox();
            this.cmbNames = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.lblNames = new System.Windows.Forms.Label();
            this.btnSelectHomeowner = new System.Windows.Forms.Button();
            this.txtHomeownerIDDisplay = new System.Windows.Forms.TextBox();
            this.clbAdvanceMonths = new System.Windows.Forms.CheckedListBox();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(984, 697);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 52);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Pay";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.InfoText;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(826, 697);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 52);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(117, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Homeowner ID:";
            // 
            // lblResidentName
            // 
            this.lblResidentName.AutoSize = true;
            this.lblResidentName.BackColor = System.Drawing.Color.Transparent;
            this.lblResidentName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblResidentName.Location = new System.Drawing.Point(236, 376);
            this.lblResidentName.Name = "lblResidentName";
            this.lblResidentName.Size = new System.Drawing.Size(133, 20);
            this.lblResidentName.TabIndex = 18;
            this.lblResidentName.Text = "[display full name]";
            // 
            // lblResidentAddress
            // 
            this.lblResidentAddress.AutoSize = true;
            this.lblResidentAddress.BackColor = System.Drawing.Color.Transparent;
            this.lblResidentAddress.Location = new System.Drawing.Point(256, 419);
            this.lblResidentAddress.Name = "lblResidentAddress";
            this.lblResidentAddress.Size = new System.Drawing.Size(126, 20);
            this.lblResidentAddress.TabIndex = 19;
            this.lblResidentAddress.Text = "[display address]";
            // 
            // cmbUnits
            // 
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(241, 283);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(121, 28);
            this.cmbUnits.TabIndex = 22;
            this.cmbUnits.SelectedIndexChanged += new System.EventHandler(this.cmbUnits_SelectedIndexChanged);
            // 
            // lblUnits
            // 
            this.lblUnits.AutoSize = true;
            this.lblUnits.BackColor = System.Drawing.Color.Transparent;
            this.lblUnits.Location = new System.Drawing.Point(117, 286);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(102, 20);
            this.lblUnits.TabIndex = 23;
            this.lblUnits.Text = "Unit Number:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(117, 331);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "Unit Status:";
            // 
            // lblUnitStatus
            // 
            this.lblUnitStatus.AutoSize = true;
            this.lblUnitStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblUnitStatus.Location = new System.Drawing.Point(239, 331);
            this.lblUnitStatus.Name = "lblUnitStatus";
            this.lblUnitStatus.Size = new System.Drawing.Size(64, 20);
            this.lblUnitStatus.TabIndex = 25;
            this.lblUnitStatus.Text = "[Status]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(114, 376);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 20);
            this.label3.TabIndex = 26;
            this.label3.Text = "Resident Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(114, 419);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = "Resident Address:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(117, 664);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 20);
            this.label5.TabIndex = 29;
            this.label5.Text = "Payment Date:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(673, 327);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 20);
            this.label6.TabIndex = 30;
            this.label6.Text = "Amount Paid:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(673, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 20);
            this.label7.TabIndex = 31;
            this.label7.Text = "Due Rate:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(673, 284);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(121, 20);
            this.label8.TabIndex = 32;
            this.label8.Text = "Month Covered:";
            // 
            // lblDueRate
            // 
            this.lblDueRate.AutoSize = true;
            this.lblDueRate.BackColor = System.Drawing.Color.Transparent;
            this.lblDueRate.Location = new System.Drawing.Point(821, 204);
            this.lblDueRate.Name = "lblDueRate";
            this.lblDueRate.Size = new System.Drawing.Size(128, 20);
            this.lblDueRate.TabIndex = 34;
            this.lblDueRate.Text = "[display due rate]";
            // 
            // lblMonthCovered
            // 
            this.lblMonthCovered.AutoSize = true;
            this.lblMonthCovered.BackColor = System.Drawing.Color.Transparent;
            this.lblMonthCovered.Location = new System.Drawing.Point(821, 284);
            this.lblMonthCovered.Name = "lblMonthCovered";
            this.lblMonthCovered.Size = new System.Drawing.Size(161, 20);
            this.lblMonthCovered.TabIndex = 35;
            this.lblMonthCovered.Text = "[display date covered]";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(117, 529);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 20);
            this.label9.TabIndex = 36;
            this.label9.Text = "Pay Month:";
            // 
            // lblAmountPaid
            // 
            this.lblAmountPaid.AutoSize = true;
            this.lblAmountPaid.BackColor = System.Drawing.Color.Transparent;
            this.lblAmountPaid.Location = new System.Drawing.Point(821, 241);
            this.lblAmountPaid.Name = "lblAmountPaid";
            this.lblAmountPaid.Size = new System.Drawing.Size(140, 20);
            this.lblAmountPaid.TabIndex = 38;
            this.lblAmountPaid.Text = "[pay monthly dues]";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(673, 372);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(110, 20);
            this.label10.TabIndex = 40;
            this.label10.Text = "Change given:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(673, 412);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 20);
            this.label11.TabIndex = 41;
            this.label11.Text = "Remarks:";
            // 
            // cmbRemarks
            // 
            this.cmbRemarks.FormattingEnabled = true;
            this.cmbRemarks.Location = new System.Drawing.Point(825, 412);
            this.cmbRemarks.Name = "cmbRemarks";
            this.cmbRemarks.Size = new System.Drawing.Size(121, 28);
            this.cmbRemarks.TabIndex = 42;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(673, 241);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 20);
            this.label12.TabIndex = 43;
            this.label12.Text = "Amount Covered:";
            // 
            // cmbPaid
            // 
            this.cmbPaid.FormattingEnabled = true;
            this.cmbPaid.Location = new System.Drawing.Point(825, 319);
            this.cmbPaid.Name = "cmbPaid";
            this.cmbPaid.Size = new System.Drawing.Size(121, 28);
            this.cmbPaid.TabIndex = 44;
            // 
            // cmbChange
            // 
            this.cmbChange.FormattingEnabled = true;
            this.cmbChange.Location = new System.Drawing.Point(825, 364);
            this.cmbChange.Name = "cmbChange";
            this.cmbChange.Size = new System.Drawing.Size(121, 28);
            this.cmbChange.TabIndex = 45;
            // 
            // cmbResidency
            // 
            this.cmbResidency.FormattingEnabled = true;
            this.cmbResidency.Location = new System.Drawing.Point(241, 240);
            this.cmbResidency.Name = "cmbResidency";
            this.cmbResidency.Size = new System.Drawing.Size(121, 28);
            this.cmbResidency.TabIndex = 46;
            // 
            // cmbNames
            // 
            this.cmbNames.FormattingEnabled = true;
            this.cmbNames.Location = new System.Drawing.Point(485, 240);
            this.cmbNames.Name = "cmbNames";
            this.cmbNames.Size = new System.Drawing.Size(148, 28);
            this.cmbNames.TabIndex = 47;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Location = new System.Drawing.Point(115, 243);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 20);
            this.label13.TabIndex = 48;
            this.label13.Text = "Residency:";
            // 
            // lblNames
            // 
            this.lblNames.AutoSize = true;
            this.lblNames.BackColor = System.Drawing.Color.Transparent;
            this.lblNames.Location = new System.Drawing.Point(375, 243);
            this.lblNames.Name = "lblNames";
            this.lblNames.Size = new System.Drawing.Size(104, 20);
            this.lblNames.TabIndex = 49;
            this.lblNames.Text = "Select Name:";
            // 
            // btnSelectHomeowner
            // 
            this.btnSelectHomeowner.Location = new System.Drawing.Point(379, 182);
            this.btnSelectHomeowner.Name = "btnSelectHomeowner";
            this.btnSelectHomeowner.Size = new System.Drawing.Size(182, 42);
            this.btnSelectHomeowner.TabIndex = 50;
            this.btnSelectHomeowner.Text = "Select Homeowner...";
            this.btnSelectHomeowner.UseVisualStyleBackColor = true;
            this.btnSelectHomeowner.Click += new System.EventHandler(this.btnSelectHomeowner_Click);
            // 
            // txtHomeownerIDDisplay
            // 
            this.txtHomeownerIDDisplay.Location = new System.Drawing.Point(241, 198);
            this.txtHomeownerIDDisplay.Multiline = true;
            this.txtHomeownerIDDisplay.Name = "txtHomeownerIDDisplay";
            this.txtHomeownerIDDisplay.ReadOnly = true;
            this.txtHomeownerIDDisplay.Size = new System.Drawing.Size(121, 26);
            this.txtHomeownerIDDisplay.TabIndex = 51;
            this.txtHomeownerIDDisplay.TabStop = false;
            // 
            // clbAdvanceMonths
            // 
            this.clbAdvanceMonths.FormattingEnabled = true;
            this.clbAdvanceMonths.Location = new System.Drawing.Point(240, 529);
            this.clbAdvanceMonths.Name = "clbAdvanceMonths";
            this.clbAdvanceMonths.Size = new System.Drawing.Size(220, 119);
            this.clbAdvanceMonths.TabIndex = 52;
            this.clbAdvanceMonths.SelectedIndexChanged += new System.EventHandler(this.clbAdvanceMonths_SelectedIndexChanged);
            // 
            // dtpPaymentDate
            // 
            this.dtpPaymentDate.Location = new System.Drawing.Point(240, 664);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.Size = new System.Drawing.Size(220, 26);
            this.dtpPaymentDate.TabIndex = 66;
            // 
            // frmPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1196, 829);
            this.Controls.Add(this.dtpPaymentDate);
            this.Controls.Add(this.clbAdvanceMonths);
            this.Controls.Add(this.txtHomeownerIDDisplay);
            this.Controls.Add(this.btnSelectHomeowner);
            this.Controls.Add(this.lblNames);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cmbNames);
            this.Controls.Add(this.cmbResidency);
            this.Controls.Add(this.cmbChange);
            this.Controls.Add(this.cmbPaid);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cmbRemarks);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblAmountPaid);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblMonthCovered);
            this.Controls.Add(this.lblDueRate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblUnitStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblUnits);
            this.Controls.Add(this.cmbUnits);
            this.Controls.Add(this.lblResidentAddress);
            this.Controls.Add(this.lblResidentName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmPayment_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblResidentName;
        private System.Windows.Forms.Label lblResidentAddress;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.Label lblUnits;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblUnitStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDueRate;
        private System.Windows.Forms.Label lblMonthCovered;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblAmountPaid;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbRemarks;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbPaid;
        private System.Windows.Forms.ComboBox cmbChange;
        private System.Windows.Forms.ComboBox cmbResidency;
        private System.Windows.Forms.ComboBox cmbNames;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblNames;
        private System.Windows.Forms.Button btnSelectHomeowner;
        private System.Windows.Forms.TextBox txtHomeownerIDDisplay;
        private System.Windows.Forms.CheckedListBox clbAdvanceMonths;
        private System.Windows.Forms.DateTimePicker dtpPaymentDate;
    }
}