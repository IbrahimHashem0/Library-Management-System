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
        private Panel rightpnl, logInpnl, regPnl, activePanel;
        private PictureBox logInPicBox;
        private Label headerLabel, emailLabel, passwordLabel;
        private Button logInButton;
        private TextBox userNameTexBox, passwordTextBox;

        // Registration fields
        private TextBox fullNameTextBox, regEmailTextBox, regPasswordTextBox;
        private ComboBox roleComboBox;

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

        private void showLogIn()
        {
            rightpnl.Controls.Clear();

            logInpnl = new Panel();
            logInpnl.Size = new Size(400, 450);
            logInpnl.Size = new Size(400, 450);
            activePanel = logInpnl;
            logInpnl.Left = (rightpnl.Width - logInpnl.Width) / 2;
            logInpnl.Top = (rightpnl.Height - logInpnl.Height) / 2;

            int innerX = (logInpnl.Width - 280) / 2;
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
            lnkReg.Click += (s, e) => {
                regPnl=  null;
                showRegister(); }; // SWITCH TO REGISTER FORM

            int totalWidth = dontHaveAcc.PreferredWidth + lnkReg.PreferredWidth;
            int startX = (logInpnl.Width - totalWidth) / 2;

            dontHaveAcc.Location = new Point(startX, y);
            lnkReg.Location = new Point(startX + dontHaveAcc.PreferredWidth, y);

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
            rightpnl.Controls.Add(logInpnl);
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

        #endregion

        public void showRegister()
        {
            rightpnl.Controls.Clear();

            regPnl = new Panel();
            regPnl.Size = new Size(400, 450);
            activePanel = regPnl;
            // Center the panel
            regPnl.Left = (rightpnl.Width - regPnl.Width) / 2;
            regPnl.Top = (rightpnl.Height - regPnl.Height) / 2;

            int innerX = (regPnl.Width - 280) / 2;
            int y = 10;

            // Header Label
            Label regHeaderLabel = new Label
            {
                Text = "Register New User",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                AutoSize = false,
                Size = new Size(regPnl.Width, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, y)
            };
            y += 80;

            // --- Full Name ---
            Label fullNameLabel = new Label { Text = "Full Name", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            fullNameTextBox = new TextBox { Name = "fullNameTextBox", Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11) };
            y += 70;

            // --- Email ---
            Label regEmailLabel = new Label { Text = "Email", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            regEmailTextBox = new TextBox { Name = "regEmailTextBox", Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11) };
            y += 70;

            // --- Password ---
            Label regPasswordLabel = new Label { Text = "Password", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(innerX, y) };
            regPasswordTextBox = new TextBox { Name = "regPasswordTextBox", Location = new Point(innerX, y + 25), Size = new Size(280, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            y += 70;

            // --- Role ---
            

            // --- Register Button ---
            Button registerButton = new Button
            {
                Text = "REGISTER",
                Location = new Point(innerX, y),
                Size = new Size(280, 45),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            registerButton.FlatAppearance.BorderSize = 0;
            // The lambda function ensures we call the event handler correctly
            registerButton.Click += (s, e) => Register_Click();
            y += 60;

            // --- Back to Login Link ---
            LinkLabel lnkLogin = new LinkLabel
            {
                Text = "<< Back to Login",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.Gray,
                Cursor = Cursors.Hand,
                Location = new Point(innerX, y)
            };
            lnkLogin.Click += (s, e) => { showLogIn(); };

            // Add controls to the Registration Panel
            regPnl.Controls.Add(regHeaderLabel);
            regPnl.Controls.Add(fullNameLabel);
            regPnl.Controls.Add(fullNameTextBox);
            regPnl.Controls.Add(regEmailLabel);
            regPnl.Controls.Add(regEmailTextBox);
            regPnl.Controls.Add(regPasswordLabel);
            regPnl.Controls.Add(regPasswordTextBox);
            regPnl.Controls.Add(registerButton);
            regPnl.Controls.Add(lnkLogin);

            rightpnl.Controls.Add(regPnl);
            regPnl.BringToFront();
        }
        private void Register_Click()
        {
            string fullName = fullNameTextBox.Text;
            string email = regEmailTextBox.Text;
            string password = regPasswordTextBox.Text;
            string role = "Student";

            // Basic input validation
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill out all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Call the Repository method to insert the user into the database
                UserRepository repo = new UserRepository();

                // --- FIX IS HERE ---
                // Change: bool success = repo.RegisterUser(name, email, plainPassword, role);
                // To:
                bool success = repo.Register(fullName, email, password, role);

                if (success)
                {
                    MessageBox.Show("Registration Successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    showLogIn(); // Switch back to the login screen
                }
                else
                {
                    // This is where database-level failures (like duplicate email) are reported
                    MessageBox.Show("Registration failed. This email might already be in use or there was a database error. Check server logs.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions (e.g., repository not found, connection issues)
                MessageBox.Show($"An unexpected error occurred during registration: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {
            // Any initialization code for the form load event
        }
    }
}
