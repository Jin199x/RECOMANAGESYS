using System;
using System.Drawing;
using System.Windows.Forms;

namespace RECOMANAGESYS
{
    public partial class AnnouncementViewForm : Form
    {
        public AnnouncementViewForm(string title, string message, bool important)
        {
            InitializeComponent();

            // Title
            lblTitle.Text = title;

            if (important)
            {
                lblTitle.ForeColor = Color.Red;
                titleBar.BackColor = Color.FromArgb(255, 255, 230);
            }
            else
            {
                lblTitle.ForeColor = Color.Black;
                titleBar.BackColor = SystemColors.ActiveCaption;
            }

            // Message
            txtMessage.Text = message;
            txtMessage.ReadOnly = true;
            txtMessage.Multiline = true;
            txtMessage.WordWrap = true;
            txtMessage.ScrollBars = ScrollBars.Vertical;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
