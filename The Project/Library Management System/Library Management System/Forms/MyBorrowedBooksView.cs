using Library_Management_System.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Library_Management_System.Models
{
    public partial class MyBorrowedBooksView : UserControl
    {
        private DataGridView grid;
        private User _CurrentUser;

        public MyBorrowedBooksView(User currentUser)
        {
            this._CurrentUser = currentUser;
            InitializeComponent();
            Initialize();
            LoadData();
        }

        private void Initialize()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(30);

            // 1. Page Title
            Label lblTitle = new Label
            {
                Text = "My Borrowed Books",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(63, 81, 181),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. Grid Setup
            grid = new DataGridView
            {
                Location = new Point(30, 80),
                Size = new Size(this.Width - 60, this.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(230, 230, 230),
                Font = new Font("Segoe UI", 10),
            };

            // 3. Styling
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(67, 97, 238);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 50;
            grid.RowTemplate.Height = 60;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 245, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;


            // 4. Events
            grid.CellPainting += Grid_CellPainting;
            grid.CellContentClick += Grid_CellContentClick;
            grid.CellMouseEnter += Grid_CellMouseEnter; // Triggers Hand Cursor
            grid.CellMouseLeave += Grid_CellMouseLeave; // Resets Cursor

            this.Controls.Add(grid);
        }

        private void LoadData()
        {
            try
            {
                if (_CurrentUser.UserID == 0) return;

                BorrowingRepository repo = new BorrowingRepository();
                var data = repo.GetUserHistory(_CurrentUser.UserID);
                data = data.OrderBy(b => Convert.ToDateTime(b.BorrowDate)) // Convert string to Date for correct sorting
           .ThenBy(b => b.Title)
           .ThenBy(b => b.Author)
           .ToList();
                grid.Columns.Clear();
                grid.DataSource = data;

                if (grid.Columns["BorrowingID"] != null) grid.Columns["BorrowingID"].Visible = false;
                if (grid.Columns["UserName"] != null) grid.Columns["UserName"].Visible = false;
                if (grid.Columns["BorrowDate"] != null) grid.Columns["BorrowDate"].HeaderText = "Borrow Date";
                if (grid.Columns["DueDate"] != null) grid.Columns["DueDate"].HeaderText = "Return Date";

                // Add "Action" Column (We will custom paint it to look like a button)
                DataGridViewButtonColumn btnReturn = new DataGridViewButtonColumn();
                btnReturn.Name = "Action";
                btnReturn.HeaderText = "Action";
                grid.Columns.Add(btnReturn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // --- 1. MOUSE CURSOR LOGIC ---
        private void Grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "Action")
            {
                string status = grid.Rows[e.RowIndex].Cells["Status"].Value.ToString();
                if (status != "Returned") // Only show Hand if clickable
                {
                    grid.Cursor = Cursors.Hand;
                }
            }
        }

        private void Grid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            grid.Cursor = Cursors.Default;
        }

        // --- 2. CLICK LOGIC ---
        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "Action")
            {
                string currentStatus = grid.Rows[e.RowIndex].Cells["Status"].Value.ToString();

                if (currentStatus == "Returned") return;

                if (MessageBox.Show("Return this book?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int borrowingId = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BorrowingID"].Value);
                        BorrowingRepository repo = new BorrowingRepository();
                        repo.ReturnBook(borrowingId);
                        MessageBox.Show("Book returned!");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        // --- 3. PAINTING LOGIC (Exact Sizes) ---
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = grid.Columns[e.ColumnIndex].Name;

            // Common Size Logic for BOTH Status and Button
            // This ensures they are identical in size
            Rectangle rect = new Rectangle(e.CellBounds.X + 20, e.CellBounds.Y + 15, e.CellBounds.Width - 40, 30);

            if (colName == "Status")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string status = e.Value?.ToString() ?? "";
                Color backColor = status == "Borrowed" ? Color.FromArgb(16, 185, 129) :
                                  status == "Overdue" ? Color.FromArgb(239, 68, 68) :
                                  Color.Gray;

                using (Brush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                TextRenderer.DrawText(e.Graphics, status, e.CellStyle.Font, rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            else if (colName == "Action")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string status = grid.Rows[e.RowIndex].Cells["Status"].Value.ToString();
                bool isReturned = status == "Returned";

                // Button Colors
                Color btnColor = isReturned ? Color.LightGray : Color.FromArgb(79, 70, 229);
                string btnText = isReturned ? "Returned" : "Return";

                // Draw Button (Exact same rect as Status)
                using (Brush brush = new SolidBrush(btnColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                TextRenderer.DrawText(e.Graphics, btnText, new Font("Segoe UI", 9, FontStyle.Bold), rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }
}