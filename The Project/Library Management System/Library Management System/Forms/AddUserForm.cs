using System;
using System.Drawing;
using System.Windows.Forms;
using Library_Management_System.Models;
using Library_Management_System.Repositories;
using LibrarySystem.Services;

namespace Library_Management_System.Forms
{
    public partial class AddUserForm : Form
    {
        private string _role;
        private TextBox txtName, txtEmail, txtPassword;
        private Label lblEmail, lblHeader;

        public AddUserForm(string role)
        {
            _role = role;
            this.Size = new Size(400, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;

            InitializeCustomUI();
            ApplyRoleLogic();
        }

        private void InitializeCustomUI()
        {
            lblHeader = new Label { Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(79, 70, 229), Location = new Point(30, 20), AutoSize = true };

            // Name field
            AddLabelAndTextBox("Full Name", 70, out txtName);

            // Email field (We will change its label in ApplyRoleLogic)
            lblEmail = new Label { Location = new Point(30, 150), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtEmail = new TextBox { Location = new Point(30, 175), Width = 320, Font = new Font("Segoe UI", 12) };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);

            // Password field
            AddLabelAndTextBox("Temporary Password", 230, out txtPassword, true);

            Button btnSave = new Button
            {
                Text = "Confirm && Save",
                Location = new Point(30, 340),
                Size = new Size(320, 45),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += btnSave_Click;

            this.Controls.AddRange(new Control[] { lblHeader, btnSave });
        }

        private void ApplyRoleLogic()
        {
            this.Text = "System - Add " + _role;
            lblHeader.Text = "Create " + _role + " Account";

            if (_role == "Reader")
            {
                lblEmail.Text = "Reader Email Address";
            }
            else
            {
                lblEmail.Text = "Official Work Email";
            }
        }

        private void AddLabelAndTextBox(string text, int y, out TextBox tb, bool isPass = false)
        {
            Label lbl = new Label { Text = text, Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            tb = new TextBox { Location = new Point(30, y + 25), Width = 320, Font = new Font("Segoe UI", 12), UseSystemPasswordChar = isPass };
            this.Controls.Add(lbl);
            this.Controls.Add(tb);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("All fields are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new UserRepository();
            string email = txtEmail.Text.Trim();

            if (repo.IsEmailExists(email))
            {
                MessageBox.Show("This user (email) already exists in the system!",
                                "Duplicate User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newUser = new User
            {
                FullName = txtName.Text.Trim(),
                Email = email,
                Password = SecurityService.HashPassword(txtPassword.Text),
                Role = _role,
                Status = "Active"
            };

            bool isSaved = repo.AddUser(newUser);

            if (isSaved)
            {
                MessageBox.Show($"{_role} added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error saving to database. Please check your connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}