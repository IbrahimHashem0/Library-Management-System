using Library_Management_System.Data;
using Library_Management_System.Repositories;
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

namespace Library_Management_System.Forms
{
    public partial class LoginForm : Form
    {
        private Panel rightpnl,logInpnl,regPnl;
        private PictureBox logInPicBox;
        private Label headerLabel , emailLabel,passwordLabel;
        private Button logInButton;
        private TextBox userNameTexBox,passwordTextBox;

        
        public LoginForm()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            logInPicBox =  new PictureBox();
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "LoginForm";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            
            this.BackColor = Color.White;
            this.Load += new System.EventHandler(this.LoginForm_Load_1);
            this.ResumeLayout(false);


            logInPicBox.Image = Image.FromFile(@"Resources\\LoginPic.jpg");
            logInPicBox.SizeMode = PictureBoxSizeMode.StretchImage;
            
            logInPicBox.Dock = DockStyle.Left;
            logInPicBox.Width = 255;

            this.Controls.Add(logInPicBox);

            



            rightpnl = new Panel();
            rightpnl.Dock = DockStyle.Fill;
            rightpnl.BackColor = Color.White;
            this.Controls.Add(rightpnl);

            rightpnl.BringToFront();

            
            this.Resize += (s, e) => {
                logInPicBox.Width = this.ClientSize.Width / 2;
                logInpnl.Left = (logInpnl.Parent.Width - logInpnl.Width) / 2;
                logInpnl.Top = (logInpnl.Parent.Height - logInpnl.Height) / 2;
            };
            this.ResumeLayout(false);
            showLogIn();
            this.OnResize(EventArgs.Empty);
        }
        private void showLogIn()
        {
            rightpnl.Controls.Clear();

            logInpnl = new Panel();
            logInpnl.Size = new Size(400, 450);

            int innerX = (logInpnl.Width - 280) / 2;
            int y = 10;


            headerLabel = new Label { Text = "Library Management System",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                AutoSize = false,
                Size = new Size(logInpnl.Width, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, y) 
            };
            y += 80;
            Label welcomeLabel = new Label
            {
                Text = "Weclome back! Please login to continue",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(logInpnl.Width, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, y)
            };
            y += 50;

            emailLabel = new Label { Text = "Email", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            userNameTexBox = new TextBox { Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11) };

            y += 70;

            passwordLabel = new Label { Text = "Password", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            passwordTextBox = new TextBox { Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            y += 80;

            logInButton = new Button
            {
                Text = "LOGIN",
                Location = new Point(innerX, y),
                Size = new Size(280, 45),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                
                Cursor = Cursors.Hand
            };
            logInButton.FlatAppearance.BorderSize = 0;
            logInButton.Click += BtnLogin_Click;
            y += 60;

            Label dontHaveAcc = new Label
            {
                Text = "Don't have an account?",
                Location = new Point(50, y),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,

                Font = new Font("Segoe UI", 10),
               
               // Cursor = Cursors.Hand
            };

            LinkLabel lnkReg = new LinkLabel
            {
                Text = "Register",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(28, 200, 138),
                Cursor = Cursors.Hand
            };
            lnkReg.Click += (s, e) => { showRegister(); };

            int totalWidth = dontHaveAcc.PreferredWidth + lnkReg.PreferredWidth;

            int startX = (logInpnl.Width - totalWidth) / 2;

            dontHaveAcc.Location = new Point(startX, y);
            lnkReg.Location = new Point(startX + dontHaveAcc.PreferredWidth, y);


            logInpnl.Controls.Add(headerLabel);
            logInpnl.Controls.Add(welcomeLabel);
            logInpnl.Controls.Add(emailLabel);
            logInpnl.Controls.Add(userNameTexBox);
            logInpnl.Controls.Add(passwordLabel);
            logInpnl.Controls.Add(passwordTextBox);
            logInpnl.Controls.Add(logInButton);
            logInpnl.Controls.Add(dontHaveAcc);
            logInpnl.Controls.Add(lnkReg);
            rightpnl.Controls.Add(logInpnl);
        }


        public void showRegister()
        {
            rightpnl.Controls.Clear();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {

            UserRepository repo = new UserRepository();

            var user = repo.Login(userNameTexBox.Text, passwordTextBox.Text);

            if (user != null)
            {
                MessageBox.Show($"Welcome, {user.Role} {user.FullName}");
                MainDashBoard dashboard = new MainDashBoard(user);
                this.Hide();
                var result = dashboard.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.Show();
                    userNameTexBox.Text = "";
                    passwordTextBox.Text = "";
                    userNameTexBox.Focus();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Your Email or Password is Incorrect");
            }

        }
        private void LoginForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
