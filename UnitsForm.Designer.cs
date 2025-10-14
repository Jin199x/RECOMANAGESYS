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
            this.DGVTCunregister = new System.Windows.Forms.DataGridView();
            this.btnUnregisterTC = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGVUnits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGVTCunregister)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUnregisterSelected
            // 
            this.btnUnregisterSelected.Location = new System.Drawing.Point(179, 38);
            this.btnUnregisterSelected.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnregisterSelected.Name = "btnUnregisterSelected";
            this.btnUnregisterSelected.Size = new System.Drawing.Size(224, 42);
            this.btnUnregisterSelected.TabIndex = 1;
            this.btnUnregisterSelected.Text = "Unregister Units";
            this.btnUnregisterSelected.UseVisualStyleBackColor = true;
            this.btnUnregisterSelected.Click += new System.EventHandler(this.btnUnregisterSelected_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(546, 38);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 42);
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
            this.DGVUnits.Location = new System.Drawing.Point(14, 97);
            this.DGVUnits.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DGVUnits.MultiSelect = false;
            this.DGVUnits.Name = "DGVUnits";
            this.DGVUnits.RowHeadersWidth = 51;
            this.DGVUnits.RowTemplate.Height = 24;
            this.DGVUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVUnits.Size = new System.Drawing.Size(1206, 303);
            this.DGVUnits.TabIndex = 3;
            // 
            // DGVTCunregister
            // 
            this.DGVTCunregister.AllowUserToAddRows = false;
            this.DGVTCunregister.AllowUserToDeleteRows = false;
            this.DGVTCunregister.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGVTCunregister.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVTCunregister.Location = new System.Drawing.Point(14, 481);
            this.DGVTCunregister.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DGVTCunregister.MultiSelect = false;
            this.DGVTCunregister.Name = "DGVTCunregister";
            this.DGVTCunregister.RowHeadersWidth = 51;
            this.DGVTCunregister.RowTemplate.Height = 24;
            this.DGVTCunregister.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVTCunregister.Size = new System.Drawing.Size(1206, 303);
            this.DGVTCunregister.TabIndex = 4;
            // 
            // btnUnregisterTC
            // 
            this.btnUnregisterTC.Location = new System.Drawing.Point(179, 431);
            this.btnUnregisterTC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnregisterTC.Name = "btnUnregisterTC";
            this.btnUnregisterTC.Size = new System.Drawing.Size(224, 42);
            this.btnUnregisterTC.TabIndex = 5;
            this.btnUnregisterTC.Text = "Unregister Tenant/Caretaker";
            this.btnUnregisterTC.UseVisualStyleBackColor = true;
            this.btnUnregisterTC.Click += new System.EventHandler(this.btnUnregisterTC_Click);
            // 
            // UnitsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 804);
            this.Controls.Add(this.btnUnregisterTC);
            this.Controls.Add(this.DGVTCunregister);
            this.Controls.Add(this.DGVUnits);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUnregisterSelected);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UnitsForm";
            this.Text = "UnitsForm";
            this.Load += new System.EventHandler(this.UnitsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGVUnits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGVTCunregister)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnUnregisterSelected;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView DGVUnits;
        private System.Windows.Forms.DataGridView DGVTCunregister;
        private System.Windows.Forms.Button btnUnregisterTC;
    }
}