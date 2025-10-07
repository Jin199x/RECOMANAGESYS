using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RECOMANAGESYS.loginform;

namespace RECOMANAGESYS
{
    public partial class dashboardControl : UserControl
    {
        public dashboardControl()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void dashboardControl_Load(object sender, EventArgs e)
        {
            // Date and Time
            lblDateTime.Text = DateTime.Now.ToString("dddd, MMM dd yyyy | hh:mm tt");

            // Dynamic Greeting
            int hour = DateTime.Now.Hour;
            string greeting;

            if (hour < 12)
                greeting = "Good Morning";
            else if (hour < 18)
                greeting = "Good Afternoon";
            else
                greeting = "Good Evening";

            lblGreeting.Text = $"{greeting}, {CurrentUser.FullName}!";
        }

    }
}
