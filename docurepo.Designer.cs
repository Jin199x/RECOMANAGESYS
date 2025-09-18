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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonAddFolder = new System.Windows.Forms.Button();
            this.buttonAddFile = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panelDesktop = new System.Windows.Forms.Panel();
            this.searchbtn = new FontAwesome.Sharp.IconButton();
            this.buttonBack = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
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
            this.panel2.Controls.Add(this.buttonBack);
            this.panel2.Controls.Add(this.panelDesktop);
            this.panel2.Controls.Add(this.searchbtn);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.buttonAddFolder);
            this.panel2.Controls.Add(this.buttonAddFile);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(29, 134);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1192, 751);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(762, 34);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(397, 47);
            this.textBox1.TabIndex = 10;
            // 
            // buttonAddFolder
            // 
            this.buttonAddFolder.BackColor = System.Drawing.SystemColors.HotTrack;
            this.buttonAddFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonAddFolder.FlatAppearance.BorderSize = 0;
            this.buttonAddFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddFolder.ForeColor = System.Drawing.Color.White;
            this.buttonAddFolder.Location = new System.Drawing.Point(36, 92);
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
            this.buttonAddFile.Location = new System.Drawing.Point(36, 34);
            this.buttonAddFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAddFile.Name = "buttonAddFile";
            this.buttonAddFile.Size = new System.Drawing.Size(169, 48);
            this.buttonAddFile.TabIndex = 7;
            this.buttonAddFile.Text = "Add File";
            this.buttonAddFile.UseVisualStyleBackColor = false;
            this.buttonAddFile.Click += new System.EventHandler(this.buttonAddFile_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(986, 100);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 48);
            this.button1.TabIndex = 5;
            this.button1.Text = "Generate File";
            this.button1.UseVisualStyleBackColor = false;
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
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightSkyBlue;
            this.buttonBack.Location = new System.Drawing.Point(253, 100);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(91, 35);
            this.buttonBack.TabIndex = 13;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = false;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonAddFile;
        private System.Windows.Forms.Button buttonAddFolder;
        private System.Windows.Forms.TextBox textBox1;
        private FontAwesome.Sharp.IconButton searchbtn;
        private System.Windows.Forms.Panel panelDesktop;
        private System.Windows.Forms.Button buttonBack;
    }
}
