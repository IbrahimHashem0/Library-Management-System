using System;
using System.Drawing;
using System.Windows.Forms;
using Library_Management_System.Models;

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

        // Content controls
        private DataGridView bookDataGridView;
        private TextBox searchTextBox;

        // Menu Buttons
        // Renamed fields for clarity
        private Button dashboardBtn, catalogBtn, myLoansBtn, myReservationsBtn, billingBtn;
        
        private const string SearchPlaceholderText = "Search by Title, Author, or ISBN...";


        public MainDashBoard(User user)
        {
            _loggedInUser = user;
            InitializeComponent();
        }

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
                BackColor = Color.White,
                Padding = new Padding(20),
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            // ----------------------------------------------------
            // Sidebar Contents (Reader Menu)
            // ----------------------------------------------------

            Label logoLabel = new Label
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

            Func<string, Button> CreateMenuButton = (text) => new Button
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
                Text = "Hello, "+userLabel.Text+" !",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 90),
                AutoSize = true
            };
            btnY += btnSpacing;
            sidebarPanel.Controls.Add(headerTitleLabel);
            // 1. Dashboard Button (Was 'Home')
            dashboardBtn = CreateMenuButton("Dashboard");
            dashboardBtn.Click += DashboardBtn_Click;
            sidebarPanel.Controls.Add(dashboardBtn);
            btnY += btnSpacing;

            // 2. Book Catalog Button (Was 'My Books')
            catalogBtn = CreateMenuButton("Book Catalog");
            catalogBtn.Click += CatalogBtn_Click;
            sidebarPanel.Controls.Add(catalogBtn);
            btnY += btnSpacing;
            
            // 3. My Loans Button (Was 'Favorites')
            myLoansBtn = CreateMenuButton("My Loans");
            myLoansBtn.Click += MyLoansBtn_Click;
            sidebarPanel.Controls.Add(myLoansBtn);
            btnY += btnSpacing;

            // 4. My Reservations Button (Was 'Notifications')
            myReservationsBtn = CreateMenuButton("My Reservations");
            myReservationsBtn.Click += MyReservationsBtn_Click;
            sidebarPanel.Controls.Add(myReservationsBtn);
            btnY += btnSpacing;
            
            // 5. Billing Button
            billingBtn = CreateMenuButton("Billing"); 
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
            // Header Contents (User Info)
            // ----------------------------------------------------

            
           
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
            
            // Start on the Book Catalog view
            catalogBtn.PerformClick();
        }

        private void MainDashBoard_Load(object sender, EventArgs e)
        {
            // Initial load logic here
        }

        #region UI Setup and Event Handlers

        private void SetupCatalogView()
        {
            contentPanel.Controls.Clear();
            
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 5)
            };
            
            searchTextBox = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width - 120, 30),
                Font = new Font("Segoe UI", 11)
            };
            
            // Manual Placeholder Implementation
            searchTextBox.Text = SearchPlaceholderText;
            searchTextBox.ForeColor = Color.Gray; 
            searchTextBox.GotFocus += SearchTextBox_Enter; 
            searchTextBox.LostFocus += SearchTextBox_Leave; 
            
            searchPanel.Controls.Add(searchTextBox);

            Button searchButton = new Button
            {
                Text = "Search",
                Location = new Point(contentPanel.Width - 110, 0),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(79, 70, 229), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right
            };
            searchButton.Click += SearchButton_Click;
            searchPanel.Controls.Add(searchButton);
            
            contentPanel.Controls.Add(searchPanel);
            searchPanel.BringToFront();

            bookDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, searchPanel.Height),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { 
                    Font = new Font("Segoe UI", 10, FontStyle.Bold), 
                    BackColor = Color.FromArgb(79, 70, 229), 
                    ForeColor = Color.Black 
                },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            bookDataGridView.Columns.Add("Title", "Title");
            bookDataGridView.Columns.Add("Author", "Author");
            bookDataGridView.Columns.Add("ISBN", "ISBN");
            bookDataGridView.Columns.Add("Available", "Available Copies");
            
            contentPanel.Controls.Add(bookDataGridView);
            bookDataGridView.BringToFront();
            
            searchPanel.BringToFront();
        }
        
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

        private void LoadBookCatalog(string searchTerm = "")
        {
            if (bookDataGridView != null)
            {
                bookDataGridView.Rows.Clear();
                // --- Placeholder Data ---
                bookDataGridView.Rows.Add("The Catcher in the Rye", "J.D. Salinger", "978-0316769174", "3");
                bookDataGridView.Rows.Add("To Kill a Mockingbird", "Harper Lee", "978-0061120084", "5");
                bookDataGridView.Rows.Add("1984", "George Orwell", "978-0451524935", "1");
            }
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

                // Replace this line inside BillingDataGridView_CellPainting:
                // e.PaintBackground(e.CellBounds, e.RowState);

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

        private void DashboardBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Dashboard Overview";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Welcome! This is your main Dashboard.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        private void CatalogBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "Book Catalog";
            SetupCatalogView();
            LoadBookCatalog();
        }

        private void MyLoansBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "My Current Loans";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Your currently borrowed books will be shown here.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }

        private void MyReservationsBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "My Active Reservations";
            contentPanel.Controls.Clear();
            Label tempLabel = new Label { Text = "Your active reservations list will be displayed here.", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 50) };
            contentPanel.Controls.Add(tempLabel);
        }
        
        private void BillingBtn_Click(object sender, EventArgs e)
        {
            headerTitleLabel.Text = "My Bills";
            SetupBillingView();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text != SearchPlaceholderText ? searchTextBox.Text : "";
            LoadBookCatalog(searchTerm);
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

        #endregion
    }
}