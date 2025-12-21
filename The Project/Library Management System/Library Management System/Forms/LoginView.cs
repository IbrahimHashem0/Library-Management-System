using Library_Management_System.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class LoginView : UserControl
    {
        private Label headerLabel, emailLabel, passwordLabel;
        private TextBox passwordTextBox, userNameTexBox;
        private LoginForm loginForm;

        private Button logInButton;
        public LoginView(LoginForm loginForm)
        {
            this.loginForm = loginForm;
            InitializeComponent();
        }

        public void InitializeComponent()
        {


            this.Dock = DockStyle.Fill;
            this.Size = new Size(400, 450);
            Panel logInpnl = new Panel();
            logInpnl.Size = new Size(400, 450);

            int innerX = (this.Width - 280) / 2;
            int y = 10;

            // Header
            headerLabel = new Label
            {
                Text = "Library Management System",
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
                Text = "Welcome back! Please login to continue",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(logInpnl.Width, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, y)
            };
            y += 50;

            // Email/Username Field
            emailLabel = new Label { Text = "Email", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            userNameTexBox = new TextBox { Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11) };
            y += 70;

            // Password Field
            passwordLabel = new Label { Text = "Password", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            passwordTextBox = new TextBox { Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            y += 80;

            // Login Button
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

            // Don't have an account / Register Link
            Label dontHaveAcc = new Label
            {
                Text = "Don't have an account?",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
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


            int totalWidth = dontHaveAcc.PreferredWidth + lnkReg.PreferredWidth;
            int startX = (logInpnl.Width - totalWidth) / 2;

            dontHaveAcc.Location = new Point(startX, y);
            lnkReg.Location = new Point(startX + dontHaveAcc.PreferredWidth, y);

            lnkReg.Click += (s, e) => { loginForm.showRegister(); };
            // Add controls to Login Panel
            logInpnl.Controls.Add(headerLabel);
            logInpnl.Controls.Add(welcomeLabel);
            logInpnl.Controls.Add(emailLabel);
            logInpnl.Controls.Add(userNameTexBox);
            logInpnl.Controls.Add(passwordLabel);
            logInpnl.Controls.Add(passwordTextBox);
            logInpnl.Controls.Add(logInButton);
            logInpnl.Controls.Add(dontHaveAcc);
            logInpnl.Controls.Add(lnkReg);
            this.Controls.Add(logInpnl);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            UserRepository repo = new UserRepository();
            var user = repo.Login(userNameTexBox.Text, passwordTextBox.Text);

            if (user != null)
            {
                if(user.Status != "Active")
                {
                    MessageBox.Show("Your Account is suspended. \nContact the mangment to solve the problem", "Account is suspended",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    userNameTexBox.Focus();
                    passwordTextBox.Text = "";
                    return;

                }
                MessageBox.Show($"Welcome, {user.Role} {user.FullName}");

                MainDashBoard dashboard = new MainDashBoard(user);

                this.Hide();
                loginForm.Hide();
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
                    loginForm.Close();
                }
            }
            else
            {
                userNameTexBox.Focus();
                MessageBox.Show("Your Email or Password is Incorrect");
            }
        }
    }
}
