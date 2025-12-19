using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System.Drawing.Drawing2D;

namespace Library_Management_System.Forms
{
    public partial class ManageBorrowingView : UserControl
    {
        private DataGridView borrowingsGrid;
        private TextBox searchBox;
        private BorrowingRepository _repo = new BorrowingRepository();

        public ManageBorrowingView()
        {
            InitializeUI();
            LoadData();

            // Link the professional painting event for the Status badge
            borrowingsGrid.CellPainting += BorrowingsGrid_CellPainting;
        }

        private string placeholder = "Search by User Name or Book title or Author Name";
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // --- 1. Header Title ---
            Label lblTitle = new Label
            {
                Text = "Borrowing Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // --- 2. Search Bar (Professional Design) ---
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
                Font = new Font("Segoe UI", 13),
                Dock = DockStyle.Fill,
                Text = placeholder,
                ForeColor = Color.Gray
            };

            searchBox.Enter += (s, e) => {
                if (searchBox.Text == placeholder)
                {
                    searchBox.Text = "";
                    searchBox.ForeColor = Color.Black;
                }
            };

            searchBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = placeholder;
                    searchBox.ForeColor = Color.Gray;
                }
            };

            searchBox.TextChanged += (s, e) => {
                if (searchBox.Text != placeholder) LoadData();
            };

            searchContainer.Controls.Add(searchBox);
            this.Controls.Add(searchContainer);

            // --- 3. DataGridView Setup ---
            borrowingsGrid = new DataGridView
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
                AllowUserToResizeRows = false,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            };

            // Header Styling
            borrowingsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            borrowingsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            borrowingsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(79, 70, 229);
            borrowingsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            borrowingsGrid.ColumnHeadersHeight = 60;
            borrowingsGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            borrowingsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // --- 4. Columns Definition (Replaced Update with Author)
            borrowingsGrid.Columns.Add("UserName", "User Name");
            borrowingsGrid.Columns.Add("Title", "Book Title");
            borrowingsGrid.Columns.Add("Author", "Author"); // Added Author column
            borrowingsGrid.Columns.Add("BorrowDate", "Borrow Date");
            borrowingsGrid.Columns.Add("ReturnDate", "Return Date");
            borrowingsGrid.Columns.Add("Status", "Status");

            this.Controls.Add(borrowingsGrid);
        }

        private void LoadData()
        {

            string searchText = (searchBox.Text == placeholder || string.IsNullOrWhiteSpace(searchBox.Text))
                                ? ""
                                : searchBox.Text;

            var data = _repo.GetAllBorrowingsForAdmin(searchText);

            borrowingsGrid.Rows.Clear();
            foreach (var b in data)
            {
                borrowingsGrid.Rows.Add(
                    b.UserName,
                    b.Title,
                    b.Author,
                    b.BorrowDate,
                    b.ReturnDate,
                    b.Status
                );
            }
        }

        private void BorrowingsGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // --- Drawing Status Badge (Pill Style) ---
            if (borrowingsGrid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);
                string status = e.Value.ToString();

                // Colors: Returned (Green), Borrowed/Pending (Orange)
                Color badgeColor = status.Equals("Returned", StringComparison.OrdinalIgnoreCase)
                                   ? Color.FromArgb(46, 204, 113)
                                   : Color.FromArgb(243, 156, 18);

                Rectangle rect = new Rectangle(e.CellBounds.X + 15, e.CellBounds.Y + 18, e.CellBounds.Width - 30, 34);
                using (GraphicsPath path = GetRoundedRect(rect, 15))
                using (Brush b = new SolidBrush(badgeColor))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(b, path);
                }
                TextRenderer.DrawText(e.Graphics, status, new Font("Segoe UI", 10, FontStyle.Bold), rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
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