using Library_Management_System.Data;
using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class LoginForm : Form
    {
        // --- UI Control Declarations (Defined as private fields) ---
        private Panel rightpnl, logInpnl, regPnl;
        private PictureBox logInPicBox;
        private Label headerLabel, emailLabel, passwordLabel;
        private Button logInButton;
        private TextBox userNameTexBox, passwordTextBox;
        private Control activePanel;

        // Registration fields
        private TextBox fullNameTextBox, regEmailTextBox, regPasswordTextBox;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            logInPicBox = new PictureBox();
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "LoginForm";
            this.FormBorderStyle = FormBorderStyle.Sizable;

            this.BackColor = Color.White;
            this.Load += new System.EventHandler(this.LoginForm_Load_1);
            this.ResumeLayout(false);

            // Log In Picture Box Setup
            // You should place your image file (e.g., LoginPic.jpg) in a Resources folder 
            // and link it here, or load it from project resources.
            // logInPicBox.Image = Image.FromFile(@"Resources\\LoginPic.jpg"); 

            logInPicBox.Image = Image.FromFile(@"Resources\\LoginPic.jpg");
            logInPicBox.SizeMode = PictureBoxSizeMode.StretchImage;
            logInPicBox.Dock = DockStyle.Left;
            logInPicBox.Width = 255;
            this.Controls.Add(logInPicBox);

            // Right Panel Setup
            rightpnl = new Panel();
            rightpnl.Dock = DockStyle.Fill;
            rightpnl.BackColor = Color.White;
            this.Controls.Add(rightpnl);
            rightpnl.BringToFront();

            // Resize event to maintain layout
            this.Resize += (s, e) => {
                logInPicBox.Width = this.ClientSize.Width / 2;
                if (activePanel != null)
                {
                    activePanel.Left = (rightpnl.Width - activePanel.Width) / 2;
                    activePanel.Top = (rightpnl.Height - activePanel.Height) / 2;
                }
            };
            this.ResumeLayout(false);
            showLogIn(); // Start on the Login page
            this.OnResize(EventArgs.Empty);
        }

        #region Login Form Implementation

        public void showLogIn()
        {
            Control view = new LoginView(this);
            rightpnl.Controls.Clear();      

            view.Dock = DockStyle.None;     
            activePanel = view;             

            rightpnl.Controls.Add(view);    

            
            activePanel.Left = (rightpnl.Width - activePanel.Width) / 2;
            activePanel.Top = (rightpnl.Height - activePanel.Height) / 2;
        }

        

        #endregion

        public void showRegister()
        {

            Control view = new RegisterView(this);
            rightpnl.Controls.Clear();

            view.Dock = DockStyle.None;
            activePanel = view;

            rightpnl.Controls.Add(view);


            activePanel.Left = (rightpnl.Width - activePanel.Width) / 2;
            activePanel.Top = (rightpnl.Height - activePanel.Height) / 2;
        }
     

        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            // Any initialization code for the form load event
        }
    }
}
