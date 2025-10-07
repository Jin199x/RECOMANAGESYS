namespace RECOMANAGESYS
{
    partial class AddUnits
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddUnits));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.unitNumbertxt = new System.Windows.Forms.TextBox();
            this.blocktxt = new System.Windows.Forms.TextBox();
            this.Savebtn = new System.Windows.Forms.Button();
            this.Cancelbtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbUnitType = new System.Windows.Forms.ComboBox();
            this.HomeownerID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DTPOwnership = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbApprovedBy = new System.Windows.Forms.ComboBox();
            this.lblHomeownerInfo = new System.Windows.Forms.Label();
            this.cmbNumRooms = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(97, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unit(Lot) Number";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(97, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "Block";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // unitNumbertxt
            // 
            this.unitNumbertxt.Location = new System.Drawing.Point(102, 165);
            this.unitNumbertxt.Multiline = true;
            this.unitNumbertxt.Name = "unitNumbertxt";
            this.unitNumbertxt.Size = new System.Drawing.Size(282, 35);
            this.unitNumbertxt.TabIndex = 3;
            // 
            // blocktxt
            // 
            this.blocktxt.Location = new System.Drawing.Point(102, 246);
            this.blocktxt.Multiline = true;
            this.blocktxt.Name = "blocktxt";
            this.blocktxt.Size = new System.Drawing.Size(282, 35);
            this.blocktxt.TabIndex = 4;
            // 
            // Savebtn
            // 
            this.Savebtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.Savebtn.FlatAppearance.BorderSize = 0;
            this.Savebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Savebtn.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.Savebtn.ForeColor = System.Drawing.Color.White;
            this.Savebtn.Location = new System.Drawing.Point(657, 394);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(100, 42);
            this.Savebtn.TabIndex = 6;
            this.Savebtn.Text = "Save";
            this.Savebtn.UseVisualStyleBackColor = false;
            this.Savebtn.Click += new System.EventHandler(this.Savebtn_Click);
            // 
            // Cancelbtn
            // 
            this.Cancelbtn.FlatAppearance.BorderSize = 0;
            this.Cancelbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancelbtn.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F);
            this.Cancelbtn.Location = new System.Drawing.Point(535, 393);
            this.Cancelbtn.Name = "Cancelbtn";
            this.Cancelbtn.Size = new System.Drawing.Size(100, 42);
            this.Cancelbtn.TabIndex = 7;
            this.Cancelbtn.Text = "Cancel";
            this.Cancelbtn.UseVisualStyleBackColor = true;
            this.Cancelbtn.Click += new System.EventHandler(this.Cancelbtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(97, 298);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 27);
            this.label4.TabIndex = 9;
            this.label4.Text = "Unit Type";
            // 
            // cmbUnitType
            // 
            this.cmbUnitType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUnitType.FormattingEnabled = true;
            this.cmbUnitType.Location = new System.Drawing.Point(102, 328);
            this.cmbUnitType.Name = "cmbUnitType";
            this.cmbUnitType.Size = new System.Drawing.Size(282, 33);
            this.cmbUnitType.TabIndex = 12;
            this.cmbUnitType.SelectedIndexChanged += new System.EventHandler(this.cmbUnitType_SelectedIndexChanged);
            // 
            // HomeownerID
            // 
            this.HomeownerID.Location = new System.Drawing.Point(439, 249);
            this.HomeownerID.Multiline = true;
            this.HomeownerID.Name = "HomeownerID";
            this.HomeownerID.Size = new System.Drawing.Size(318, 35);
            this.HomeownerID.TabIndex = 14;
            this.HomeownerID.TextChanged += new System.EventHandler(this.HomeownerID_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(434, 219);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(165, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Homeowner ID";
            // 
            // DTPOwnership
            // 
            this.DTPOwnership.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DTPOwnership.Location = new System.Drawing.Point(439, 333);
            this.DTPOwnership.Name = "DTPOwnership";
            this.DTPOwnership.Size = new System.Drawing.Size(318, 30);
            this.DTPOwnership.TabIndex = 18;
            this.DTPOwnership.ValueChanged += new System.EventHandler(this.DTPOwnership_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(434, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 27);
            this.label5.TabIndex = 19;
            this.label5.Text = "Approved By:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(434, 303);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(201, 27);
            this.label7.TabIndex = 20;
            this.label7.Text = "Date of Ownership";
            // 
            // cmbApprovedBy
            // 
            this.cmbApprovedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbApprovedBy.FormattingEnabled = true;
            this.cmbApprovedBy.Location = new System.Drawing.Point(439, 165);
            this.cmbApprovedBy.Name = "cmbApprovedBy";
            this.cmbApprovedBy.Size = new System.Drawing.Size(318, 33);
            this.cmbApprovedBy.TabIndex = 21;
            // 
            // lblHomeownerInfo
            // 
            this.lblHomeownerInfo.AutoSize = true;
            this.lblHomeownerInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblHomeownerInfo.Location = new System.Drawing.Point(532, 95);
            this.lblHomeownerInfo.Name = "lblHomeownerInfo";
            this.lblHomeownerInfo.Size = new System.Drawing.Size(86, 16);
            this.lblHomeownerInfo.TabIndex = 16;
            this.lblHomeownerInfo.Text = "VALIDATION";
            // 
            // cmbNumRooms
            // 
            this.cmbNumRooms.FormattingEnabled = true;
            this.cmbNumRooms.Location = new System.Drawing.Point(102, 405);
            this.cmbNumRooms.Name = "cmbNumRooms";
            this.cmbNumRooms.Size = new System.Drawing.Size(282, 24);
            this.cmbNumRooms.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(102, 383);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 16);
            this.label3.TabIndex = 23;
            this.label3.Text = "How Many Rooms";
            // 
            // AddUnits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(858, 503);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbNumRooms);
            this.Controls.Add(this.cmbApprovedBy);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DTPOwnership);
            this.Controls.Add(this.lblHomeownerInfo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.HomeownerID);
            this.Controls.Add(this.cmbUnitType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Cancelbtn);
            this.Controls.Add(this.Savebtn);
            this.Controls.Add(this.blocktxt);
            this.Controls.Add(this.unitNumbertxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddUnits";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AddUnits_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox unitNumbertxt;
        private System.Windows.Forms.TextBox blocktxt;
        private System.Windows.Forms.Button Savebtn;
        private System.Windows.Forms.Button Cancelbtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbUnitType;
        private System.Windows.Forms.TextBox HomeownerID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker DTPOwnership;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbApprovedBy;
        private System.Windows.Forms.Label lblHomeownerInfo;
        private System.Windows.Forms.ComboBox cmbNumRooms;
        private System.Windows.Forms.Label label3;
    }
}