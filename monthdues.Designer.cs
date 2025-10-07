namespace RECOMANAGESYS
{
    partial class monthdues
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
            this.cmbResidentFilter = new System.Windows.Forms.ComboBox();
            this.lvResidents = new System.Windows.Forms.ListView();
            this.lvMonths = new System.Windows.Forms.ListView();
            this.updatePayment = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.addPayment = new FontAwesome.Sharp.IconButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cmbResidentFilter);
            this.panel1.Controls.Add(this.lvResidents);
            this.panel1.Controls.Add(this.lvMonths);
            this.panel1.Controls.Add(this.updatePayment);
            this.panel1.Controls.Add(this.btnGenerate);
            this.panel1.Controls.Add(this.searchbtn);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Location = new System.Drawing.Point(30, 164);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1193, 727);
            this.panel1.TabIndex = 1;
            // 
            // cmbResidentFilter
            // 
            this.cmbResidentFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbResidentFilter.FormattingEnabled = true;
            this.cmbResidentFilter.Location = new System.Drawing.Point(768, 65);
            this.cmbResidentFilter.Name = "cmbResidentFilter";
            this.cmbResidentFilter.Size = new System.Drawing.Size(283, 30);
            this.cmbResidentFilter.TabIndex = 9;
            this.cmbResidentFilter.Text = "Resident Filter";
            this.cmbResidentFilter.SelectedIndexChanged += new System.EventHandler(this.cmbResidentFilter_SelectedIndexChanged);
            // 
            // lvResidents
            // 
            this.lvResidents.GridLines = true;
            this.lvResidents.HideSelection = false;
            this.lvResidents.Location = new System.Drawing.Point(110, 111);
            this.lvResidents.Name = "lvResidents";
            this.lvResidents.Size = new System.Drawing.Size(972, 281);
            this.lvResidents.TabIndex = 8;
            this.lvResidents.UseCompatibleStateImageBehavior = false;
            this.lvResidents.View = System.Windows.Forms.View.Details;
            // 
            // lvMonths
            // 
            this.lvMonths.GridLines = true;
            this.lvMonths.HideSelection = false;
            this.lvMonths.Location = new System.Drawing.Point(110, 398);
            this.lvMonths.Name = "lvMonths";
            this.lvMonths.Size = new System.Drawing.Size(972, 270);
            this.lvMonths.TabIndex = 7;
            this.lvMonths.UseCompatibleStateImageBehavior = false;
            this.lvMonths.View = System.Windows.Forms.View.Details;
            // 
            // updatePayment
            // 
            this.updatePayment.BackColor = System.Drawing.SystemColors.HotTrack;
            this.updatePayment.FlatAppearance.BorderSize = 0;
            this.updatePayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updatePayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updatePayment.ForeColor = System.Drawing.Color.White;
            this.updatePayment.Location = new System.Drawing.Point(24, 24);
            this.updatePayment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.updatePayment.Name = "updatePayment";
            this.updatePayment.Size = new System.Drawing.Size(174, 48);
            this.updatePayment.TabIndex = 5;
            this.updatePayment.Text = "Update ";
            this.updatePayment.UseVisualStyleBackColor = false;
            this.updatePayment.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(204, 24);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(225, 48);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generate Report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(1108, 12);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(57, 39);
            this.searchbtn.TabIndex = 3;
            this.searchbtn.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(768, 12);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(397, 39);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.addPayment);
            this.panel2.Location = new System.Drawing.Point(30, 29);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1193, 113);
            this.panel2.TabIndex = 2;
            // 
            // addPayment
            // 
            this.addPayment.BackColor = System.Drawing.SystemColors.HotTrack;
            this.addPayment.FlatAppearance.BorderSize = 0;
            this.addPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addPayment.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.addPayment.ForeColor = System.Drawing.Color.White;
            this.addPayment.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.addPayment.IconColor = System.Drawing.Color.White;
            this.addPayment.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.addPayment.IconSize = 25;
            this.addPayment.Location = new System.Drawing.Point(964, 30);
            this.addPayment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addPayment.Name = "addPayment";
            this.addPayment.Size = new System.Drawing.Size(201, 51);
            this.addPayment.TabIndex = 5;
            this.addPayment.Text = "Add Payment";
            this.addPayment.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.addPayment.UseVisualStyleBackColor = false;
            this.addPayment.Click += new System.EventHandler(this.addvisitor_Click);
            // 
            // monthdues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "monthdues";
            this.Size = new System.Drawing.Size(1249, 920);
            this.Load += new System.EventHandler(this.monthdues_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSearch;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.Button btnGenerate;
        private FontAwesome.Sharp.IconButton addPayment;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button updatePayment;
        private System.Windows.Forms.ListView lvMonths;
        private System.Windows.Forms.ListView lvResidents;
        private System.Windows.Forms.ComboBox cmbResidentFilter;
    }
}
