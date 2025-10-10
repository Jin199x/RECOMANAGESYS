namespace RECOMANAGESYS
{
    partial class UnitsForm
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
            this.btnUnregisterSelected = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.DGVUnits = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DGVUnits)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUnregisterSelected
            // 
            this.btnUnregisterSelected.Location = new System.Drawing.Point(159, 30);
            this.btnUnregisterSelected.Name = "btnUnregisterSelected";
            this.btnUnregisterSelected.Size = new System.Drawing.Size(199, 34);
            this.btnUnregisterSelected.TabIndex = 1;
            this.btnUnregisterSelected.Text = "Unregister Units";
            this.btnUnregisterSelected.UseVisualStyleBackColor = true;
            this.btnUnregisterSelected.Click += new System.EventHandler(this.btnUnregisterSelected_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(485, 30);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 34);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // DGVUnits
            // 
            this.DGVUnits.AllowUserToAddRows = false;
            this.DGVUnits.AllowUserToDeleteRows = false;
            this.DGVUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGVUnits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVUnits.Location = new System.Drawing.Point(12, 79);
            this.DGVUnits.MultiSelect = false;
            this.DGVUnits.Name = "DGVUnits";
            this.DGVUnits.RowHeadersWidth = 51;
            this.DGVUnits.RowTemplate.Height = 24;
            this.DGVUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVUnits.Size = new System.Drawing.Size(776, 338);
            this.DGVUnits.TabIndex = 3;
            // 
            // UnitsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DGVUnits);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUnregisterSelected);
            this.Name = "UnitsForm";
            this.Text = "UnitsForm";
            this.Load += new System.EventHandler(this.UnitsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGVUnits)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnUnregisterSelected;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView DGVUnits;
    }
}