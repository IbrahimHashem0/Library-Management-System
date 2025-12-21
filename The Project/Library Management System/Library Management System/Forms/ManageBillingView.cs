using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Library_Management_System.Repositories;

namespace Library_Management_System.Forms
{
    public partial class ManageBillingView : UserControl
    {
        private DataGridView billingGrid;
        private BillingRepository _repo = new BillingRepository();

        public ManageBillingView()
        {
            InitializeUI();
            LoadData();
            // Link professional painting for the Status Badge
            billingGrid.CellPainting += BillingGrid_CellPainting;
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 247, 250); 

            // Header Title 
            Label lblHeader = new Label
            {
                Text = "Bill Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(30, 25),
                AutoSize = true
            };
            this.Controls.Add(lblHeader);

            // --- DataGridView Design ---
            billingGrid = new DataGridView
            {
                Location = new Point(30, 110),
                Size = new Size(this.Width - 60, this.Height - 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowTemplate = { Height = 60 }, 
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Custom Header Styling
            billingGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(79, 70, 229);
            billingGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            billingGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            billingGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            billingGrid.ColumnHeadersHeight = 55;

            // Default Cell Styling
            billingGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            billingGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(79, 70, 229);
            billingGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            billingGrid.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            this.Controls.Add(billingGrid);

            // --- Action Button
            Button btnConfirm = new Button
            {
                Text = "Mark as paid",
                Size = new Size(200, 50),
                Location = new Point(30, this.Height - 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += BtnConfirm_Click;
            this.Controls.Add(btnConfirm);
        }

        private void LoadData()
        {
            billingGrid.DataSource = _repo.GetAllPayments();
            if (billingGrid.Columns["PaymentID"] != null)
                billingGrid.Columns["PaymentID"].Visible = false;
        }

        // Custom Drawing for Badges
        private void BillingGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (billingGrid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);
                string status = e.Value.ToString();

                Color badgeColor;
                if (status.ToLower().Contains("paid") && !status.ToLower().Contains("unpaid"))
                {
                    badgeColor = Color.FromArgb(26, 188, 156);
                }
                else
                {
                    badgeColor = Color.FromArgb(241, 196, 15);
                }

                Rectangle rect = new Rectangle(e.CellBounds.X + 30, e.CellBounds.Y + 15, e.CellBounds.Width - 60, 30);

                using (GraphicsPath path = GetRoundedRect(rect, 15))
                using (Brush b = new SolidBrush(badgeColor))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(b, path);
                }

                TextRenderer.DrawText(e.Graphics, status, new Font("Segoe UI", 9, FontStyle.Bold), rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

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

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (billingGrid.SelectedRows.Count > 0)
            {
                if (billingGrid.SelectedRows.Count > 0)
                {
                    string currentStatus = billingGrid.SelectedRows[0].Cells["Status"].Value.ToString();

                    if (currentStatus.ToLower().Contains("paid") && !currentStatus.ToLower().Contains("unpaid"))
                    {
                        MessageBox.Show("This transaction is already paid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    int pID = Convert.ToInt32(billingGrid.SelectedRows[0].Cells["PaymentID"].Value);

                    if (_repo.UpdatePaymentStatus(pID, "Paid"))
                    {
                        MessageBox.Show("Transaction marked as Paid successfully.");
                        LoadData(); 
                    }
                }
                else
                {
                    MessageBox.Show("Please select a record first.");
                }
            }
        }
    }
}