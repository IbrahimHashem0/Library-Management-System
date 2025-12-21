using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System.Drawing.Drawing2D;

namespace Library_Management_System.Forms
{
    public partial class MyBillsView : UserControl
    {
        private readonly User _currentUser;
        private DataGridView billsGrid;

        public MyBillsView(User user)
        {
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));

            InitializeUI();
            LoadData();

            // This line was causing the error because the method below was missing
            billsGrid.CellPainting += BillsGrid_CellPainting;
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // 1. Title
            Label lblTitle = new Label
            {
                Text = "My Bills",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. DataGridView
            billsGrid = new DataGridView
            {
                Location = new Point(30, 100),
                Size = new Size(this.Width - 60, this.Height - 150),
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
                AllowUserToResizeRows = false
            };

            // Styling
            billsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            billsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            billsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 125, 245);
            billsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            billsGrid.ColumnHeadersHeight = 60;
            billsGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            billsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Define Columns to match the UI image
            
            billsGrid.Columns.Add("BookID", "Book ID");
            billsGrid.Columns.Add("PaymentDate", "Payment Date");
            billsGrid.Columns.Add("BorrowingPrice", "Amount");
            billsGrid.Columns.Add("Status", "Status");

            this.Controls.Add(billsGrid);
        }

        private void LoadData()
        {
            // Ensure BillingRepository is in your Repositories folder
            var repo = new BillingRepository();
            var bills = repo.GetUserBills(_currentUser.UserID);

            billsGrid.Rows.Clear();
            foreach (var b in bills)
            {
                billsGrid.Rows.Add(b.BookTitle, b.Date, b.Price.ToString("c"), b.Status);
            }
        }

        // --- THE MISSING METHOD THAT FIXES YOUR ERROR ---
        private void BillsGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Draw Rounded Status Badge
            if (billsGrid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);
                string status = e.Value.ToString();

                // Color Logic
                Color pillColor = Color.FromArgb(149, 165, 166); // Pending
                if (status == "Paid") pillColor = Color.FromArgb(46, 204, 113);      // Green
                else if (status == "Overdue") pillColor = Color.FromArgb(231, 76, 60); // Red

                Rectangle rect = new Rectangle(e.CellBounds.X + 20, e.CellBounds.Y + 18, e.CellBounds.Width - 40, 34);

                using (GraphicsPath path = GetRoundedRect(rect, 15))
                using (Brush b = new SolidBrush(pillColor))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(b, path);
                }

                TextRenderer.DrawText(e.Graphics, status, new Font("Segoe UI", 11, FontStyle.Bold),
                    rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

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