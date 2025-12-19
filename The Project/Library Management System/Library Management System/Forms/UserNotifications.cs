using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class NotificationsUserControl : UserControl
    {
        private FlowLayoutPanel flowNotifications;
        private Button btnMarkRead;

        public NotificationsUserControl()
        {
            InitializeNotificationPage();

            // Sample Data
            AddNotification("Book Return Reminder",
                "You need to return \"The Midnight Library\" by tomorrow.",
                "2 hours ago", true);

            AddNotification("Book Return Reminder",
                "You need to return \"The Midnight Library\" by tomorrow.",
                "2 hours ago", false);

            AddNotification("Book Return Reminder",
                "You need to return \"The Midnight Library\" by tomorrow.",
                "2 hours ago", true);
        }

        private void InitializeNotificationPage()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Title
            Label lblTitle = new Label
            {
                Text = "Notifications",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 100, 240),
                AutoSize = true,
                Location = new Point(30, 30)
            };

            // Mark All as Read Button
            btnMarkRead = new Button
            {
                Text = "Mark All as Read",
                Size = new Size(160, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(90, 120, 240),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(this.Width - 220, 35),
                Cursor = Cursors.Hand
            };
            btnMarkRead.Click += (s, e) =>
            {
                foreach (Control c in flowNotifications.Controls)
                {
                    if (c is Panel card)
                    {
                       card.Tag = false;

                        card.Invalidate();
                    }
                }

                btnMarkRead.Enabled = false;
                btnMarkRead.Text = "All Read";
            };
            btnMarkRead.FlatAppearance.BorderSize = 0;

            // Notifications Container
            flowNotifications = new FlowLayoutPanel
            {
                Location = new Point(30, 100),
                Size = new Size(this.Width - 60, this.Height - 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            foreach (Control c in flowNotifications.Controls)
            {
                if (c is Panel card)
                {
                    card.Tag = false; // Read
                    card.BackColor = Color.White;

                    card.Invalidate();
                }
            }
            // Resize handling (important)
            flowNotifications.Resize += (s, e) =>
            {
                foreach (Control c in flowNotifications.Controls)
                    c.Width = flowNotifications.ClientSize.Width - 25;
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnMarkRead);
            this.Controls.Add(flowNotifications);
        }

        public void AddNotification(string title, string message, string time, bool isUnread)
        {
            Panel card = new Panel
            {
                Width = flowNotifications.ClientSize.Width - 25,
                Height = 110,
                Margin = new Padding(0, 0, 0, 15),
                Tag = isUnread   // true = Unread, false = Read
            };



            // Rounded Border
            card.Paint += (s, e) =>
            {
                bool isunread = (bool)card.Tag;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Background
                using (Brush bg = new SolidBrush(
                    isunread ? Color.FromArgb(235, 242, 255) : Color.White))
                {
                    e.Graphics.FillRectangle(bg, card.ClientRectangle);
                }

                // Border (Unread بس)
                if (isunread)
                {
                    DrawRoundedRectangle(
                        e.Graphics,
                        new Pen(Color.FromArgb(200, 215, 255), 1),
                        card.ClientRectangle,
                        12
                    );
                }
            };



            // Title
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };

            // Message
            Label lblMsg = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.DimGray,
                Location = new Point(15, 38),
                Size = new Size(card.Width - 30, 35)
            };

            // Time
            Label lblTime = new Label
            {
                Text = time,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Location = new Point(15, 78),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblMsg);
            card.Controls.Add(lblTime);

            flowNotifications.Controls.Add(card);
        }

        // Rounded Rectangle Helpers
        private void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
