using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System.Drawing.Drawing2D;

namespace Library_Management_System.Forms
{
    public partial class ManageUsersView : UserControl
    {
        // Store the currently logged-in user for security checks
        private readonly User _loggedInUser;

        private DataGridView usersGrid;
        private TextBox searchBox;
        private ComboBox roleCombo;

        public ManageUsersView(User user)
        {
            // Ensure session user is provided
            _loggedInUser = user ?? throw new ArgumentNullException(nameof(user));

            InitializeUI();
            LoadData();

            // Link required events
            usersGrid.CellPainting += UsersGrid_CellPainting;
            usersGrid.CellMouseClick += UsersGrid_CellMouseClick;
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // --- 1. Header Title ---
            Label lblTitle = new Label
            {
                Text = "Manage Users",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // --- 2. Add User Button with Dropdown Logic ---
            Button btnAddUser = new Button
            {
                Text = "+ Add New User",
                Size = new Size(180, 45),
                Location = new Point(this.Width - 210, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddUser.FlatAppearance.BorderSize = 0;

            // Create Context Menu for Role Selection
            ContextMenuStrip addMenu = new ContextMenuStrip();
            addMenu.Font = new Font("Segoe UI", 11);
            addMenu.Items.Add("New Librarian", null, (s, e) => OpenAddUserForm("Librarian"));
            addMenu.Items.Add("New Reader (Student)", null, (s, e) => OpenAddUserForm("Reader"));

            // Show menu on button click
            btnAddUser.Click += (s, e) => {
                addMenu.Show(btnAddUser, new Point(0, btnAddUser.Height));
            };
            this.Controls.Add(btnAddUser);

            // --- 3. Search Bar ---
            Panel searchContainer = new Panel
            {
                Location = new Point(30, 90),
                Size = new Size(450, 50),
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10)
            };
            searchBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 14),
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };
            searchBox.TextChanged += (s, e) => LoadData();
            searchContainer.Controls.Add(searchBox);
            this.Controls.Add(searchContainer);

            // --- 4. Role Filter ---
            roleCombo = new ComboBox
            {
                Location = new Point(500, 90),
                Width = 180,
                Font = new Font("Segoe UI", 13),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleCombo.Items.AddRange(new string[] { "All Roles", "Admin", "Librarian", "Reader" });
            roleCombo.SelectedIndex = 0;
            roleCombo.SelectedIndexChanged += (s, e) => LoadData();
            this.Controls.Add(roleCombo);

            // --- 5. DataGridView Setup ---
            usersGrid = new DataGridView
            {
                Location = new Point(30, 170),
                Size = new Size(this.Width - 60, this.Height - 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 70 },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                AllowUserToResizeRows = false
            };

            usersGrid.DefaultCellStyle.Font = new Font("Segoe UI", 13);
            usersGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            usersGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 125, 245);
            usersGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            usersGrid.ColumnHeadersHeight = 60;
            usersGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            usersGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Define Columns
            usersGrid.Columns.Add("UserID", "ID");
            usersGrid.Columns["UserID"].Visible = false;
            usersGrid.Columns.Add("FullName", "User Name");
            usersGrid.Columns.Add("Email", "Email");
            usersGrid.Columns.Add("Role", "Role");
            usersGrid.Columns.Add("Status", "Status");
            usersGrid.Columns.Add("Actions", "Actions");

            usersGrid.Columns["FullName"].FillWeight = 160;
            usersGrid.Columns["Email"].FillWeight = 220;
            usersGrid.Columns["Role"].FillWeight = 90;
            usersGrid.Columns["Status"].FillWeight = 110;
            usersGrid.Columns["Actions"].FillWeight = 200;

            this.Controls.Add(usersGrid);
        }

        private void LoadData()
        {
            var repo = new UserRepository();
            string role = roleCombo.SelectedItem?.ToString() ?? "All Roles";
            var users = repo.GetAllUsers(searchBox.Text, role);

            usersGrid.Rows.Clear();
            foreach (var u in users)
            {
                usersGrid.Rows.Add(u.UserID, u.FullName, u.Email, u.Role, u.Status);
            }
        }

        private void OpenAddUserForm(string role)
        {
            using (AddUserForm addUserForm = new AddUserForm(role))
            {
                if (addUserForm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void UsersGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Draw Status Badge
            if (usersGrid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);
                string status = e.Value.ToString();
                Color pillColor = status == "Active" ? Color.FromArgb(46, 204, 113) : Color.FromArgb(241, 196, 15);
                Rectangle rect = new Rectangle(e.CellBounds.X + 15, e.CellBounds.Y + 18, e.CellBounds.Width - 30, 34);

                using (GraphicsPath path = GetRoundedRect(rect, 15))
                using (Brush b = new SolidBrush(pillColor))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(b, path);
                }
                TextRenderer.DrawText(e.Graphics, status, new Font("Segoe UI", 11, FontStyle.Bold), rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }

            // Draw Action Buttons
            if (usersGrid.Columns[e.ColumnIndex].Name == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);
                string currentStatus = usersGrid.Rows[e.RowIndex].Cells["Status"].Value.ToString();
                int btnWidth = (e.CellBounds.Width / 2) - 15;

                Rectangle deleteRect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 18, btnWidth, 34);
                Rectangle actionRect = new Rectangle(e.CellBounds.X + btnWidth + 20, e.CellBounds.Y + 18, btnWidth, 34);

                using (GraphicsPath path = GetRoundedRect(deleteRect, 10))
                using (Brush b = new SolidBrush(Color.FromArgb(231, 76, 60)))
                {
                    e.Graphics.FillPath(b, path);
                    TextRenderer.DrawText(e.Graphics, "Delete", new Font("Segoe UI", 11, FontStyle.Bold), deleteRect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                string btnText = (currentStatus == "Active") ? "Suspend" : "Activate";
                Color btnColor = (currentStatus == "Active") ? Color.FromArgb(241, 196, 15) : Color.FromArgb(46, 204, 113);

                using (GraphicsPath path = GetRoundedRect(actionRect, 10))
                using (Brush b = new SolidBrush(btnColor))
                {
                    e.Graphics.FillPath(b, path);
                    TextRenderer.DrawText(e.Graphics, btnText, new Font("Segoe UI", 11, FontStyle.Bold), actionRect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                e.Handled = true;
            }
        }

        private void UsersGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (usersGrid.Columns[e.ColumnIndex].Name == "Actions")
            {
                int btnWidth = (usersGrid.Columns[e.ColumnIndex].Width / 2) - 15;
                int relativeX = e.X;

                if (relativeX > 10 && relativeX < 10 + btnWidth) { HandleDelete(e.RowIndex); }
                else if (relativeX > btnWidth + 20 && relativeX < (btnWidth * 2) + 20) { HandleSuspend(e.RowIndex); }
            }
        }

        private void HandleDelete(int rowIndex)
        {
            int userId = Convert.ToInt32(usersGrid.Rows[rowIndex].Cells["UserID"].Value);
            string userEmail = usersGrid.Rows[rowIndex].Cells["Email"].Value.ToString().ToLower();
            string userRole = usersGrid.Rows[rowIndex].Cells["Role"].Value.ToString();
            var repo = new UserRepository();

            if (userEmail == "admin@library.com")
            {
                MessageBox.Show("The Main Admin account cannot be deleted.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (_loggedInUser.Role == "Admin" && _loggedInUser.Email.ToLower() != "admin@library.com")
            {
                if (userRole == "Admin")
                {
                    MessageBox.Show("Only the Master Admin can delete other Admin accounts.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (_loggedInUser.Role == "Librarian")
            {
                if (userRole == "Admin" || userRole == "Librarian")
                {
                    MessageBox.Show("Librarians can only delete Readers.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (userEmail == _loggedInUser.Email.ToLower())
            {
                MessageBox.Show("You cannot delete your own account!", "Action Denied", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (userRole == "Admin" && repo.GetAdminCount() <= 1)
            {
                MessageBox.Show("Cannot delete the only remaining Admin.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to PERMANENTLY delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (repo.DeleteUser(userId)) { LoadData(); }
            }
        }

        private void HandleSuspend(int rowIndex)
        {
            int userId = Convert.ToInt32(usersGrid.Rows[rowIndex].Cells["UserID"].Value);
            string userEmail = usersGrid.Rows[rowIndex].Cells["Email"].Value.ToString().ToLower();
            string currentStatus = usersGrid.Rows[rowIndex].Cells["Status"].Value.ToString();
            string userRole = usersGrid.Rows[rowIndex].Cells["Role"].Value.ToString();
            var repo = new UserRepository();

            if (userEmail == "admin@library.com" || userEmail == _loggedInUser.Email.ToLower())
            {
                MessageBox.Show("This account cannot be suspended.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (_loggedInUser.Role == "Librarian")
            {
                if (userRole == "Admin" || userRole == "Librarian")
                {
                    MessageBox.Show("Librarians can only suspend Reader accounts.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }

            if (_loggedInUser.Role == "Admin" && _loggedInUser.Email.ToLower() != "admin@library.com" && userRole == "Admin")
            {
                MessageBox.Show("Only the Master Admin can suspend other Admin accounts.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (userRole == "Admin" && currentStatus == "Active" && repo.GetAdminCount() <= 1)
            {
                MessageBox.Show("Cannot suspend the only active Admin!", "Security", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (repo.ToggleUserStatus(userId, currentStatus)) { LoadData(); }
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}