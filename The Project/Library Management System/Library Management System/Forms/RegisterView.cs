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
using System.Text.RegularExpressions;

namespace Library_Management_System.Forms
{
    public partial class RegisterView : UserControl
    {

        private Panel regPnl;
        private LoginForm loginForm;
        private TextBox fullNameTextBox, regEmailTextBox, regPasswordTextBox;
        public RegisterView(LoginForm loginForm)
        {
               this.loginForm = loginForm;
            InitializeComponent();
        }

        public void InitializeComponent()
        {

            this.Dock = DockStyle.Fill;
            this.Size = new Size(400, 450);
            Panel regPnl = new Panel();
            regPnl.Size = new Size(400, 450);


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
            lnkLogin.Click += (s, e) => { 
                loginForm.showLogIn(); 
            };

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

            this.Controls.Add(regPnl);
            regPnl.BringToFront();
        }


        public bool IsValidEmail(string email)
        {
            
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, pattern);
        }
        private void Register_Click()
        {
            string fullName = fullNameTextBox.Text.Trim();
            string email = regEmailTextBox.Text.Trim().ToLower();
            string password = regPasswordTextBox.Text;
            string role = "Reader"; 
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill out all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserRepository repo = new UserRepository();

            if (repo.IsEmailExists(email))
            {
                MessageBox.Show("This email is already registered. Please try logging in.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string hashedPassword = LibrarySystem.Services.SecurityService.HashPassword(password);

                var newUser = new Library_Management_System.Models.User
                {
                    FullName = fullName,
                    Email = email,
                    Password = hashedPassword,
                    Role = role,
                    Status = "Active"
                };
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("This email is Invalid. Please enter a valid email in.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    regEmailTextBox.Focus();
                    return;
                }
                bool success = repo.AddUser(newUser);

                if (success)
                {
                    MessageBox.Show("Registration Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loginForm.showLogIn();
                }
                else
                {
                    MessageBox.Show("Registration failed due to a database error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
