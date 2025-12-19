using Library_Management_System.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class MainDashBoard : Form
    {

        private readonly User _loggedInUser;

        private FavoritesUserControl favoritesView;

        // UI Components
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Label headerTitleLabel;
        private Label userLabel;
        private Label logoLabel;

        // Menu Buttons
        private Button dashboardBtn, catalogBtn, myFavoritesBtn, myReservationsBtn, billingBtn;
        private Button manageBooksBtn, manageUsersBtn;
        private Button borrowingBtn;

        private const string SearchPlaceholderText = "Search by Title, Author, or ISBN...";

        public MainDashBoard(User user)
        {
            _loggedInUser = user;
            InitializeComponent();
            ApplyRoleBasedUI();

            favoritesView = new FavoritesUserControl();
            favoritesView.Dock = DockStyle.Fill;
            favoritesView.Visible = false; 
            contentPanel.Controls.Add(favoritesView); 
        }

        private void ApplyRoleBasedUI()
        {
            string role = _loggedInUser.Role.ToLower();

            userLabel.Text = _loggedInUser?.FullName ?? "Guest User";
            headerTitleLabel.Text = $"Hello, {userLabel.Text}!";

            if (role == "admin" || role == "librarian")
            {
                this.Text = "LMS - Admin Dashboard";
                logoLabel.Text = "LMS Admin";

                catalogBtn.Visible = false;
                myFavoritesBtn.Visible = false;
                myReservationsBtn.Visible = false;

                int btnY = dashboardBtn.Location.Y + dashboardBtn.Height + 10;
                int btnSpacing = 55;

                dashboardBtn.Text = "Dashboard";

                manageBooksBtn = CreateMenuButton("Manage Book");
                manageBooksBtn.Location = new Point(10, btnY);
                manageBooksBtn.Click += ManageBooksBtn_Click;
                sidebarPanel.Controls.Add(manageBooksBtn);
                btnY += btnSpacing;

                manageUsersBtn = CreateMenuButton("Manage User");
                manageUsersBtn.Location = new Point(10, btnY);
                manageUsersBtn.Click += ManageUsersBtn_Click;
                sidebarPanel.Controls.Add(manageUsersBtn);
                btnY += btnSpacing;

                borrowingBtn = CreateMenuButton("Borrowing");
                borrowingBtn.Location = new Point(10, btnY);
                borrowingBtn.Click += BorrowingBtn_Click;
                sidebarPanel.Controls.Add(borrowingBtn);
                btnY += btnSpacing;

                billingBtn.Text = "Bill";
                billingBtn.Location = new Point(10, btnY);
                billingBtn.Visible = true;
            }

            DashboardBtn_Click(this, EventArgs.Empty);
        }

        private Button CreateMenuButton(string text)
        {
            int btnHeight = 45;
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

        public void InitializeComponent()
        {
            this.Text = "LMS - Reader Dashboard";
            this.ClientSize = new Size(1200, 750);
            this.Name = "MainDashBoard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(79, 70, 229);

            this.SuspendLayout();

            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 265,
                BackColor = Color.FromArgb(79, 70, 229),
                Padding = new Padding(10),
            };
            this.Controls.Add(sidebarPanel);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20),
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 80),
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

            Button logoutBtn = new Button
            {
                Text = "LOGOUT",
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

            this.Load += new System.EventHandler(this.MainDashBoard_Load);
            this.Resize += (s, e) =>
            {
                logoutBtn.Location = new Point(10, this.ClientSize.Height - 80);
            };

            this.ResumeLayout(false);

            contentPanel.Controls.Clear();
        }

        private void MainDashBoard_Load(object sender, EventArgs e)
        { }

        #region UI Views Setup
        private void SetupBillingView()
        {
            contentPanel.Controls.Clear();
            Label panelHeader = new Label
            {
                Text = "My Bills",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.BottomLeft
            };
            contentPanel.Controls.Add(panelHeader);

            DataGridView billingDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
            };
            billingDataGridView.Columns.Add("InvoiceID", "Invoice ID");
            billingDataGridView.Columns.Add("BookTitle", "Book Title");
            billingDataGridView.Columns.Add("Date", "Date");
            billingDataGridView.Columns.Add("Price", "Price");
            billingDataGridView.Columns.Add("Status", "Status");

            contentPanel.Controls.Add(billingDataGridView);
            billingDataGridView.BringToFront();
        }

        private void SetupAdminBillingView()
        {
            contentPanel.Controls.Clear();
            Label panelHeader = new Label { Text = "Manage All Bills", Font = new Font("Segoe UI", 24, FontStyle.Bold), Dock = DockStyle.Top, Height = 60 };
            contentPanel.Controls.Add(panelHeader);
        }
        #endregion

        #region Menu Click Event Handlers

        private void DashboardBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Dashboard Overview";
            contentPanel.Controls.Clear();

            if (_loggedInUser.Role.ToLower() == "admin")
            {
                AdminDashboardView dashboard = new AdminDashboardView();
                dashboard.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(dashboard);
                dashboard.BringToFront();
            }
            else
            {
                ReaderHomeView view = new ReaderHomeView(_loggedInUser);
                view.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(view);
                view.BringToFront();
            }
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void BorrowedBooksBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Borrowed Books";
            contentPanel.Controls.Clear();
            MyBorrowedBooksView view = new MyBorrowedBooksView(_loggedInUser);
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            view.BringToFront();
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void FavoriteBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "My Favorite Books";
            contentPanel.Controls.Clear();

            contentPanel.Controls.Add(favoritesView);
            favoritesView.Visible = true;
            favoritesView.BringToFront();

            favoritesView.LoadFavorites(_loggedInUser.UserID);
        }
        private void MyReservationsBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Notifications";
            contentPanel.Controls.Clear();
        }

        private void BillingBtn_Click(object sender, EventArgs e)
        {
            string role = _loggedInUser.Role.ToLower();
            if (role == "admin" || role == "librarian")
            {
                headerTitleLabel.Text = "Manage System Bills";
                SetupAdminBillingView();
            }
            else
            {
                headerTitleLabel.Text = "My Bills";
                SetupBillingView();
            }
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            LoginForm log = new LoginForm();
            this.Hide();
            if (log.ShowDialog() == DialogResult.OK) this.Show();
            else this.Close();
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void ManageBooksBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Manage Books";
            contentPanel.Controls.Clear();
            ManageBooksView view = new ManageBooksView();
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            view.BringToFront();
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void ManageUsersBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Manage Users";
            contentPanel.Controls.Clear();
            ManageUsersView view = new ManageUsersView(_loggedInUser);
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            view.BringToFront();
            if (favoritesView != null) favoritesView.Visible = false;
        }

        private void BorrowingBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Borrowing && Returns";
            contentPanel.Controls.Clear();

            if (favoritesView != null) favoritesView.Visible = false;
        }
        #endregion
    }
}