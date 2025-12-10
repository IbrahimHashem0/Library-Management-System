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
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "LoginForm";
            this.FormBorderStyle = FormBorderStyle.Sizable;

        }
    }
}
