namespace RECOMANAGESYS
{
    partial class Homeowners
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Homeowners));
            this.button4 = new System.Windows.Forms.Button();
            this.Edit = new System.Windows.Forms.Button();
            this.AddResidentsbtn = new System.Windows.Forms.Button();
            this.refreshbtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Deletebtn = new System.Windows.Forms.Button();
            this.AddUnitbtn = new System.Windows.Forms.Button();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.DGVResidents = new System.Windows.Forms.DataGridView();
            this.ViewUnitbtn = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVResidents)).BeginInit();
            this.SuspendLayout();
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(749, 23);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(290, 28);
            this.button4.TabIndex = 5;
            this.button4.Text = "View Officer ";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Edit
            // 
            this.Edit.Location = new System.Drawing.Point(151, 76);
            this.Edit.Name = "Edit";
            this.Edit.Size = new System.Drawing.Size(136, 44);
            this.Edit.TabIndex = 3;
            this.Edit.Text = "Edit Residents";
            this.Edit.UseVisualStyleBackColor = true;
            this.Edit.Click += new System.EventHandler(this.Edit_Click);
            // 
            // AddResidentsbtn
            // 
            this.AddResidentsbtn.Location = new System.Drawing.Point(304, 76);
            this.AddResidentsbtn.Name = "AddResidentsbtn";
            this.AddResidentsbtn.Size = new System.Drawing.Size(140, 44);
            this.AddResidentsbtn.TabIndex = 2;
            this.AddResidentsbtn.Text = "Add residents";
            this.AddResidentsbtn.UseVisualStyleBackColor = true;
            this.AddResidentsbtn.Click += new System.EventHandler(this.AddResidentsbtn_Click);
            // 
            // refreshbtn
            // 
            this.refreshbtn.Location = new System.Drawing.Point(20, 76);
            this.refreshbtn.Name = "refreshbtn";
            this.refreshbtn.Size = new System.Drawing.Size(107, 44);
            this.refreshbtn.TabIndex = 1;
            this.refreshbtn.Text = "Refresh";
            this.refreshbtn.UseVisualStyleBackColor = true;
            this.refreshbtn.Click += new System.EventHandler(this.refreshbtn_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ViewUnitbtn);
            this.panel2.Controls.Add(this.Deletebtn);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.AddUnitbtn);
            this.panel2.Controls.Add(this.searchbtn);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.AddResidentsbtn);
            this.panel2.Controls.Add(this.DGVResidents);
            this.panel2.Controls.Add(this.refreshbtn);
            this.panel2.Controls.Add(this.Edit);
            this.panel2.Location = new System.Drawing.Point(18, 90);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1074, 626);
            this.panel2.TabIndex = 6;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // Deletebtn
            // 
            this.Deletebtn.Location = new System.Drawing.Point(585, 93);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(75, 23);
            this.Deletebtn.TabIndex = 13;
            this.Deletebtn.Text = "Delete";
            this.Deletebtn.UseVisualStyleBackColor = true;
            this.Deletebtn.Click += new System.EventHandler(this.Deletebtn_Click);
            // 
            // AddUnitbtn
            // 
            this.AddUnitbtn.Location = new System.Drawing.Point(462, 43);
            this.AddUnitbtn.Name = "AddUnitbtn";
            this.AddUnitbtn.Size = new System.Drawing.Size(107, 44);
            this.AddUnitbtn.TabIndex = 12;
            this.AddUnitbtn.Text = "Add Unit";
            this.AddUnitbtn.UseVisualStyleBackColor = true;
            this.AddUnitbtn.Click += new System.EventHandler(this.AddUnitbtn_Click);
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(980, 78);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(50, 38);
            this.searchbtn.TabIndex = 11;
            this.searchbtn.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(677, 78);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 38);
            this.textBox1.TabIndex = 10;
            // 
            // DGVResidents
            // 
            this.DGVResidents.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.DGVResidents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVResidents.Location = new System.Drawing.Point(18, 147);
            this.DGVResidents.Name = "DGVResidents";
            this.DGVResidents.RowHeadersWidth = 51;
            this.DGVResidents.RowTemplate.Height = 24;
            this.DGVResidents.Size = new System.Drawing.Size(1021, 447);
            this.DGVResidents.TabIndex = 8;
            this.DGVResidents.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ViewUnitbtn
            // 
            this.ViewUnitbtn.Location = new System.Drawing.Point(462, 107);
            this.ViewUnitbtn.Name = "ViewUnitbtn";
            this.ViewUnitbtn.Size = new System.Drawing.Size(107, 34);
            this.ViewUnitbtn.TabIndex = 14;
            this.ViewUnitbtn.Text = "View Unit";
            this.ViewUnitbtn.UseVisualStyleBackColor = true;
            this.ViewUnitbtn.Click += new System.EventHandler(this.ViewUnitbtn_Click);
            // 
            // Homeowners
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.panel2);
            this.Name = "Homeowners";
            this.Size = new System.Drawing.Size(1110, 736);
            this.Load += new System.EventHandler(this.Homeowners_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVResidents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button Edit;
        private System.Windows.Forms.Button AddResidentsbtn;
        private System.Windows.Forms.Button refreshbtn;
        private System.Windows.Forms.Panel panel2;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView DGVResidents;
        private System.Windows.Forms.Button AddUnitbtn;
        private System.Windows.Forms.Button Deletebtn;
        private System.Windows.Forms.Button ViewUnitbtn;
    }
}
