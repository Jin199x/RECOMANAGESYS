using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace RECOMANAGESYS
{
    public partial class PostAnnouncement : Form
    {
        private Announcement parentControl;
        private int editId = -1;

        // Original constructor (kept)
        public PostAnnouncement(Announcement parent)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            parentControl = parent;

            try
            {
                if (chkNoExpire != null)
                {
                    chkNoExpire.CheckedChanged -= chkNoExpire_CheckedChanged;
                    chkNoExpire.CheckedChanged += chkNoExpire_CheckedChanged;
                    chkNoExpire.Checked = true; // default: No Expiration checked
                }

                if (dtpExpire != null)
                {
                    dtpExpire.ShowCheckBox = true;   // ✅ allow unchecked state
                    dtpExpire.Checked = false;       // default: not picked
                    dtpExpire.Enabled = !(chkNoExpire?.Checked ?? true);
                }
            }
            catch { }
        }

        // Original 4-arg constructor (kept)
        public PostAnnouncement(Announcement parent, int id, string title, string message)
        {
            InitializeComponent();
            parentControl = parent;
            editId = id;
            txtTitle.Text = title;
            txtMessage.Text = message;

            try
            {
                if (chkNoExpire != null)
                {
                    chkNoExpire.CheckedChanged -= chkNoExpire_CheckedChanged;
                    chkNoExpire.CheckedChanged += chkNoExpire_CheckedChanged;
                    chkNoExpire.Checked = true;
                }

                if (dtpExpire != null)
                {
                    dtpExpire.ShowCheckBox = true;   // ✅ allow unchecked state
                    dtpExpire.Checked = false;       // default: not picked
                    dtpExpire.Enabled = !(chkNoExpire?.Checked ?? true);
                }
            }
            catch { }
        }

        // Optional overload: constructor with expiration
        public PostAnnouncement(Announcement parent, int id, string title, string message, DateTime? expireDate)
        {
            InitializeComponent();
            parentControl = parent;
            editId = id;
            txtTitle.Text = title;
            txtMessage.Text = message;

            try
            {
                if (chkNoExpire != null)
                {
                    chkNoExpire.CheckedChanged -= chkNoExpire_CheckedChanged;
                    chkNoExpire.CheckedChanged += chkNoExpire_CheckedChanged;
                }

                if (dtpExpire != null)
                {
                    dtpExpire.ShowCheckBox = true;   // ✅ allow unchecked state
                    if (expireDate.HasValue)
                    {
                        dtpExpire.Value = expireDate.Value;
                        dtpExpire.Checked = true;
                        dtpExpire.Enabled = true;
                    }
                    else
                    {
                        dtpExpire.Checked = false;
                        dtpExpire.Enabled = false;
                    }
                }

                if (expireDate.HasValue)
                {
                    if (chkNoExpire != null) chkNoExpire.Checked = false;
                }
                else
                {
                    if (chkNoExpire != null) chkNoExpire.Checked = true;
                }
            }
            catch { }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // kept as you had it
        }

        private void chkNoExpire_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtpExpire != null && chkNoExpire != null)
                    dtpExpire.Enabled = !chkNoExpire.Checked;
            }
            catch { }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            //Title validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("A title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            //Body/content validation
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show("The announcement body cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMessage.Focus();
                return;
            }

            //Expiration validation
            if (chkNoExpire != null && !chkNoExpire.Checked)
            {
                if (dtpExpire == null || !dtpExpire.Checked) // ✅ must pick a date
                {
                    MessageBox.Show("Please select an expiration date or check 'No Expiration'.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpExpire?.Focus();
                    return;
                }

                // Prevent today or past dates
                if (dtpExpire.Value.Date <= DateTime.Today)
                {
                    MessageBox.Show("Expiration date cannot be today or in the past.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpExpire.Focus();
                    return;
                }
            }

            // Get the "Important" value
            bool isImportant = cbImportant.Checked;

            //Save to database
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd;

                DateTime? expireDate = null;
                try
                {
                    if (chkNoExpire != null && !chkNoExpire.Checked && dtpExpire != null && dtpExpire.Checked)
                        expireDate = dtpExpire.Value;
                }
                catch { }

                if (editId == -1)
                {
                    string query = "INSERT INTO Announcements (Title, Message, DatePosted, ExpirationDate, IsImportant) " +
                                   "VALUES (@title, @msg, @date, @expire, @important)";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@msg", txtMessage.Text.Trim());
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@important", isImportant ? 1 : 0);
                    if (expireDate.HasValue)
                        cmd.Parameters.AddWithValue("@expire", expireDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@expire", DBNull.Value);
                }
                else
                {
                    string query = "UPDATE Announcements SET Title=@title, Message=@msg, ExpirationDate=@expire, IsImportant=@important WHERE Id=@id";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@msg", txtMessage.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", editId);
                    cmd.Parameters.AddWithValue("@important", isImportant ? 1 : 0);
                    if (expireDate.HasValue)
                        cmd.Parameters.AddWithValue("@expire", expireDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@expire", DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }

            if (parentControl != null)
                parentControl.LoadAnnouncement();

            MessageBox.Show(editId == -1 ? "Announcement posted successfully!" : "Announcement updated successfully!");

            // Reset form
            txtTitle.Clear();
            txtMessage.Clear();
            cbImportant.Checked = false; // reset checkbox
            this.Close();
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
