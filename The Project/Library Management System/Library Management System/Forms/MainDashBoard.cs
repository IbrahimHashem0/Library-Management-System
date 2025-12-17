using System;
using System.Drawing;
using System.Windows.Forms;
using Library_Management_System.Models;
using Library_Management_System.Forms; // Needed for LoginForm

namespace Library_Management_System.Forms
{
    public partial class MainDashBoard : Form
    {
        // User object to hold login details
        private readonly User _loggedInUser;

        // UI Components
        private Panel sidebarPanel;
        private Panel headerPanel;
        private Panel contentPanel;
        private Label headerTitleLabel;
        private Label userLabel;
        private Label roleLabel;
        private Label logoLabel; // Field for logo label

        // Content controls
        private DataGridView bookDataGridView;
        private TextBox searchTextBox;

        // Menu Buttons
        // Renamed fields for clarity
        private Button dashboardBtn, catalogBtn, myFavoritesBtn, myReservationsBtn, billingBtn;
        // ***** BUTTONS FOR ADMIN/LIBRARIAN ROLES *****
        private Button manageBooksBtn, manageUsersBtn; // Buttons for Admin/Librarian Management
        private Button borrowingBtn; // New field for Borrowing button

        private const string SearchPlaceholderText = "Search by Title, Author, or ISBN...";


        public MainDashBoard(User user)
        {
            _loggedInUser = user;
            InitializeComponent();
            ApplyRoleBasedUI(); // Call the role customization logic
        }

        // ***************************************************************
        // *** CORE LOGIC: APPLY ROLE-BASED UI CUSTOMIZATION ***
        // ***************************************************************

        private void ApplyRoleBasedUI()
        {
            // Logic to determine roles and add/modify UI elements
            string role = _loggedInUser.Role.ToLower();

            // Update user info display
            userLabel.Text = _loggedInUser?.FullName ?? "Guest User";
            headerTitleLabel.Text = $"Hello, {userLabel.Text}!";

            if (role == "admin" || role == "librarian")
            {
                this.Text = "LMS - Admin Dashboard";
                logoLabel.Text = "LMS Admin";

                // ----------------------------------------------------
                // 1. Hide Reader-Specific Buttons
                // ----------------------------------------------------
                // Hide: My Books (catalogBtn), Favorites (myLoansBtn), Notifications (myReservationsBtn)
                catalogBtn.Visible = false;
                myFavoritesBtn.Visible = false;
                myReservationsBtn.Visible = false;

                // ----------------------------------------------------
                // 2. Add/Position Admin/Librarian Buttons
                // ----------------------------------------------------

                // Calculate the starting Y position for the new buttons (after Dashboard Button)
                int btnY = dashboardBtn.Location.Y + dashboardBtn.Height + 10; // Start after Dashboard button
                int btnSpacing = 55; // Standard spacing

                // Ensure the dashboard button text matches the requested look
                dashboardBtn.Text = "Dashboard";

                // --- BUTTON: Manage Book ---
                manageBooksBtn = CreateMenuButton("Manage Book"); // Updated text
                manageBooksBtn.Location = new Point(10, btnY);
                manageBooksBtn.Click += ManageBooksBtn_Click;
                sidebarPanel.Controls.Add(manageBooksBtn);
                btnY += btnSpacing;

                // --- BUTTON: Manage User ---
                manageUsersBtn = CreateMenuButton("Manage User"); // Updated text
                manageUsersBtn.Location = new Point(10, btnY);
                manageUsersBtn.Click += ManageUsersBtn_Click;
                sidebarPanel.Controls.Add(manageUsersBtn);
                btnY += btnSpacing;

                // --- BUTTON: Borrowing ---
                borrowingBtn = CreateMenuButton("Borrowing"); // New button for loans management
                borrowingBtn.Location = new Point(10, btnY);
                borrowingBtn.Click += BorrowingBtn_Click;
                sidebarPanel.Controls.Add(borrowingBtn);
                btnY += btnSpacing;

                // --- BUTTON: Bill (Repositioning existing billingBtn) ---
                billingBtn.Text = "Bill"; // Updated text to match image
                billingBtn.Location = new Point(10, btnY); // Repositioned
                billingBtn.Visible = true; // Ensure it is visible and positioned last
                DashboardBtn_Click(this, EventArgs.Empty);

            }
        }

        // ***************************************************************
        // *** HELPER METHOD FOR BUTTON CREATION ***
        // ***************************************************************

        private Button CreateMenuButton(string text)
        {
            // Helper method that replicates the button styling used in InitializeComponent
            int btnHeight = 45; // Based on InitializeComponent

            return new Button
            {
                Text = text,
                Size = new Size(sidebarPanel.Width - 20, btnHeight),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, BorderColor = Color.FromArgb(79, 70, 229) },
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = text.Replace(" ", "")
            };
        }

        // ***************************************************************
        // *** INITIALIZE COMPONENT (START OF UI CONSTRUCTION) ***
        // ***************************************************************

        public void InitializeComponent()
        {
            this.Text = "LMS - Reader Dashboard";
            this.ClientSize = new Size(1200, 750);
            this.Name = "MainDashBoard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(79, 70, 229);

            this.SuspendLayout();

            // 1. Sidebar Panel (Left Side - Green/Teal)
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 265,
                BackColor = Color.FromArgb(79, 70, 229),
                Padding = new Padding(10),
            };
            this.Controls.Add(sidebarPanel);



            // 3. Content Panel (Main Body - WhiteSmoke)
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20),
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            // ----------------------------------------------------
            // Sidebar Contents (Reader Menu)
            // ----------------------------------------------------

            // *** FIX: Assigning to the Field variable logoLabel ***
            logoLabel = new Label
            {
                Text = "LMS Reader",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(logoLabel);

            int btnY = 120;
            int btnHeight = 45;
            int btnSpacing = 55;

            Func<string, Button> LocalCreateMenuButton = (text) => new Button
            {
                Text = text,
                Location = new Point(10, btnY),
                Size = new Size(sidebarPanel.Width - 20, btnHeight),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, BorderColor = Color.FromArgb(79, 70, 229) },
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = text.Replace(" ", "")
            };

            userLabel = new Label
            {
                Text = _loggedInUser?.FullName ?? "Guest User",
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // Slightly smaller font
                ForeColor = Color.White,
                Location = new Point(10, 80), // Positioned below the logo
                AutoSize = true
            };

            headerTitleLabel = new Label
            {
                Text = "Hello, " + userLabel.Text + " !",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 90),
                AutoSize = true
            };
            btnY += btnSpacing;
            sidebarPanel.Controls.Add(headerTitleLabel);

            dashboardBtn = LocalCreateMenuButton("Home");
            dashboardBtn.Click += DashboardBtn_Click;
            sidebarPanel.Controls.Add(dashboardBtn);
            btnY += btnSpacing;

            catalogBtn = LocalCreateMenuButton("My Books");
            catalogBtn.Click += BorrowedBooksBtn_Click;
            sidebarPanel.Controls.Add(catalogBtn);
            btnY += btnSpacing;

            myFavoritesBtn = LocalCreateMenuButton("Favorites");
            myFavoritesBtn.Click += FavoriteBtn_Click;
            sidebarPanel.Controls.Add(myFavoritesBtn);
            btnY += btnSpacing;

            myReservationsBtn = LocalCreateMenuButton("Notifications");
            myReservationsBtn.Click += MyReservationsBtn_Click;
            sidebarPanel.Controls.Add(myReservationsBtn);
            btnY += btnSpacing;

     
            billingBtn = LocalCreateMenuButton("Billing");
            billingBtn.Click += BillingBtn_Click;
            sidebarPanel.Controls.Add(billingBtn);
            btnY += btnSpacing;


            // Logout Button
            Button logoutBtn = new Button
            {
                Text = "LOGOUT", // Using uppercase for consistency with the LOGIN button in the form
                Location = new Point(10, this.ClientSize.Height - 80),
                Size = new Size(sidebarPanel.Width - 20, 40),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            logoutBtn.Click += LogoutBtn_Click;
            sidebarPanel.Controls.Add(logoutBtn);
            logoutBtn.BringToFront();

            // ----------------------------------------------------
            // Finalization
            // ----------------------------------------------------

            this.Load += new System.EventHandler(this.MainDashBoard_Load);
            this.Resize += (s, e) => {
                logoutBtn.Location = new Point(10, this.ClientSize.Height - 80);
                if (searchTextBox != null && searchTextBox.Parent != null)
                {
                    searchTextBox.Size = new Size(searchTextBox.Parent.Width - 120, 30);
                }
            };

            this.ResumeLayout(false);

            if (_loggedInUser.Role != "admin")
            {
                QuoteView view = new QuoteView();
                contentPanel.Controls.Clear();
                contentPanel.Controls.Add(view);
                view.Dock = DockStyle.Fill;
            }
                // Start on the Book Catalog view
                catalogBtn.PerformClick();
        }

        private void MainDashBoard_Load(object sender, EventArgs e)
        {
            // Initial load logic here
        }

        #region UI Setup and Event Handlers

       


        private void SetupBillingView()
        {
            contentPanel.Controls.Clear();

            Label panelHeader = new Label
            {
                Text = "My Bills",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 60,
                TextAlign = ContentAlignment.BottomLeft
            };
            contentPanel.Controls.Add(panelHeader);

            DataGridView billingDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,

                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 0, 0, 0)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(8, 0, 0, 0)
                },
                RowTemplate = { Height = 40 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            billingDataGridView.Columns.Add("InvoiceID", "Invoice ID");
            billingDataGridView.Columns.Add("BookTitle", "Book Title");
            billingDataGridView.Columns.Add("Date", "Date");
            billingDataGridView.Columns.Add("Price", "Price");

            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            billingDataGridView.Columns.Add(statusColumn);

            contentPanel.Controls.Add(billingDataGridView);
            billingDataGridView.BringToFront();


            billingDataGridView.CellPainting += BillingDataGridView_CellPainting;
        }

        private void SetupAdminBillingView()
        {
            contentPanel.Controls.Clear();

            Label panelHeader = new Label
            {
                Text = "Manage All Bills", // ADMIN TITLE (Different from "My Bills")
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 60,
                TextAlign = ContentAlignment.BottomLeft
            };
            contentPanel.Controls.Add(panelHeader);

            DataGridView billingDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,

                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 0, 0, 0)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(8, 0, 0, 0)
                },
                RowTemplate = { Height = 40 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            billingDataGridView.Columns.Add("InvoiceID", "Invoice ID");
            billingDataGridView.Columns.Add("BookTitle", "Book Title");
            billingDataGridView.Columns.Add("Date", "Date");
            billingDataGridView.Columns.Add("Price", "Price");

            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            billingDataGridView.Columns.Add(statusColumn);

            contentPanel.Controls.Add(billingDataGridView);
            billingDataGridView.BringToFront();


            billingDataGridView.CellPainting += BillingDataGridView_CellPainting;
        }




        #endregion

        #region Custom Placeholder Handlers

        private void SearchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == SearchPlaceholderText)
            {
                searchTextBox.Text = "";
                searchTextBox.ForeColor = Color.Black;
            }
        }

        private void SearchTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = SearchPlaceholderText;
                searchTextBox.ForeColor = Color.Gray;
            }
        }

        #endregion

        #region Custom Status Cell Painting

        private void BillingDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                string status = e.Value?.ToString();
                Color backColor;
                Color foreColor = Color.White;

                switch (status)
                {
                    case "Paid":
                        backColor = Color.FromArgb(40, 167, 69); // Green
                        break;
                    case "Overdue":
                        backColor = Color.FromArgb(220, 53, 69); // Red
                        break;
                    case "Pending":
                        backColor = Color.FromArgb(108, 117, 125); // Gray/Secondary
                        break;
                    default:
                        e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                        e.Handled = true;
                        return;
                }

                // With this line:
                e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);


                int horizontalPadding = 10;
                int verticalPadding = 6;
                Rectangle pillRect = new Rectangle(
                    e.CellBounds.X + horizontalPadding,
                    e.CellBounds.Y + verticalPadding,
                    e.CellBounds.Width - (horizontalPadding * 2),
                    e.CellBounds.Height - (verticalPadding * 2)
                );

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, pillRect);
                }

                using (SolidBrush textBrush = new SolidBrush(foreColor))
                using (StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    e.Graphics.DrawString(status, new Font(e.CellStyle.Font.FontFamily, 9, FontStyle.Bold), textBrush, pillRect, format);
                }

                e.Handled = true;
            }
        }

        #endregion

        #region Menu Click Event Handlers

        private void DashboardBtn_Click(object sender , EventArgs e)
        {
            headerTitleLabel.Text = "Dashboard Overview";

            contentPanel.Controls.Clear();

            if (_loggedInUser.Role.ToLower() == "admin")
            {
                AdminDashboardView dashboard = new AdminDashboardView();
                contentPanel.Controls.Add(dashboard);
                dashboard.Dock = DockStyle.Fill;
            }
            else
            {
                ReaderHomeView view = new ReaderHomeView(_loggedInUser);
                contentPanel.Controls.Clear();
                contentPanel.Controls.Add(view);
                view.Dock = DockStyle.Fill;
            }
        }

        private void BorrowedBooksBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Borrowed Books";

            MyBorrowedBooksView view = new MyBorrowedBooksView(_loggedInUser);
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(view);
            view.Dock = DockStyle.Fill;
        }

        private void FavoriteBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "My Favorite Books";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Your favorite books will be shown here.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        private void MyReservationsBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Notifications";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Your active reservations list will be displayed here.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        private void BillingBtn_Click(object sender, EventArgs e)
        {
            string role = _loggedInUser.Role.ToLower();

            if (role == "admin" || role == "librarian")
            {
                // Admin/Librarian view: Manage all bills
                headerTitleLabel.Text = "Manage System Bills";
                SetupAdminBillingView(); // NEW Admin view setup (See implementation below)
            }
            else
            {
                // Reader/Student view: View personal bills
                headerTitleLabel.Text = "My Bills";
                SetupBillingView(); // Existing Reader view setup
            }
        }



        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            LoginForm log = new LoginForm();

            this.Hide();

            var result = log.ShowDialog();

            if (result == DialogResult.OK)

            {
                this.Show();


            }
            else
            {
                this.Close();
            }


        }

        // ***** START OF ADMIN EVENT HANDLERS *****

        private void ManageBooksBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Manage Books";
            contentPanel.Controls.Clear();
            ManageBooksView view = new ManageBooksView();
            contentPanel.Controls.Add(view);
        }

        private void ManageUsersBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Manage Users";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Form for managing user roles/accounts goes here.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        private void BorrowingBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Borrowing && Returns";
            //headerTitleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            contentPanel.Controls.Clear();
            // This view will contain the UI for issuing and returning books.
            Label tempLabel = new Label { Text = "Borrowing and Return Management Form is ready!", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        // ***** END OF ADMIN EVENT HANDLERS *****

        #endregion
    }
}