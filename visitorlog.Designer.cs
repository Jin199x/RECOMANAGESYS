namespace RECOMANAGESYS
{
    partial class visitorlog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(visitorlog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.VisitorLogDGV = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.addvisitor = new FontAwesome.Sharp.IconButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VisitorLogDGV)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.searchbtn);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.VisitorLogDGV);
            this.panel1.Location = new System.Drawing.Point(26, 144);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1060, 572);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button3.Location = new System.Drawing.Point(186, 20);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 41);
            this.button3.TabIndex = 5;
            this.button3.Text = "Refresh";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(26, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 41);
            this.button1.TabIndex = 3;
            this.button1.Text = "Update Out Time";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // searchbtn
            // 
            this.searchbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchbtn.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.searchbtn.IconColor = System.Drawing.Color.Black;
            this.searchbtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.searchbtn.IconSize = 28;
            this.searchbtn.Location = new System.Drawing.Point(985, 22);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(50, 38);
            this.searchbtn.TabIndex = 2;
            this.searchbtn.UseVisualStyleBackColor = true;
            this.searchbtn.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(682, 22);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 38);
            this.textBox1.TabIndex = 1;
            // 
            // VisitorLogDGV
            // 
            this.VisitorLogDGV.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.VisitorLogDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.VisitorLogDGV.Location = new System.Drawing.Point(22, 79);
            this.VisitorLogDGV.Name = "VisitorLogDGV";
            this.VisitorLogDGV.RowHeadersWidth = 51;
            this.VisitorLogDGV.RowTemplate.Height = 24;
            this.VisitorLogDGV.Size = new System.Drawing.Size(1013, 470);
            this.VisitorLogDGV.TabIndex = 0;
            this.VisitorLogDGV.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.VisitorLogDGV_CellContentClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.addvisitor);
            this.panel2.Location = new System.Drawing.Point(26, 32);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1060, 90);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3.BackgroundImage")));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(22, 20);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(211, 48);
            this.panel3.TabIndex = 1;
            // 
            // addvisitor
            // 
            this.addvisitor.BackColor = System.Drawing.SystemColors.HotTrack;
            this.addvisitor.FlatAppearance.BorderSize = 0;
            this.addvisitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addvisitor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.addvisitor.ForeColor = System.Drawing.Color.White;
            this.addvisitor.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.addvisitor.IconColor = System.Drawing.Color.White;
            this.addvisitor.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.addvisitor.IconSize = 25;
            this.addvisitor.Location = new System.Drawing.Point(902, 24);
            this.addvisitor.Name = "addvisitor";
            this.addvisitor.Size = new System.Drawing.Size(133, 41);
            this.addvisitor.TabIndex = 0;
            this.addvisitor.Text = "Add Visitor";
            this.addvisitor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.addvisitor.UseVisualStyleBackColor = false;
            this.addvisitor.Click += new System.EventHandler(this.addvisitor_Click);
            // 
            // visitorlog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "visitorlog";
            this.Size = new System.Drawing.Size(1110, 736);
            this.Load += new System.EventHandler(this.visitorlog_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VisitorLogDGV)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private FontAwesome.Sharp.IconButton addvisitor;
        private System.Windows.Forms.DataGridView VisitorLogDGV;
        private System.Windows.Forms.TextBox textBox1;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}
