using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library_Management_System.Models;

namespace Library_Management_System.Forms
{
    public partial class MainDashBoard : Form
    {
        public MainDashBoard(User user)
        {
            InitializeComponent();
        }
        public void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainDashBoard
            // 
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "MainDashBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MainDashBoard_Load);
            this.ResumeLayout(false);

        }

        private void MainDashBoard_Load(object sender, EventArgs e)
        {

        }
    }
}
