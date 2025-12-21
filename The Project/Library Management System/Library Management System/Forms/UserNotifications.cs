using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System;
using System.Collections.Generic;
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
        private int userId;
        public NotificationsUserControl(User user)
        {
            InitializeNotificationPage();

            // Store user for reloading
            this.userId = user.UserID;

            // Load initial data
            LoadNotifications();
        }

        public void LoadNotifications()
        {
            // Clear existing notifications (except title and button if they are in the same container, 
            // but here flowNotifications contains the cards, so we clear THAT)
            flowNotifications.Controls.Clear();

            NotificationRepository rep = new NotificationRepository();
            List<Notification> userNotif = rep.GetUserNotifications(userId);

            foreach (var notif in userNotif)
            {
                AddNotification(notif.NotificationID, notif.Title, notif.Message, notif.CreatedAt.ToShortDateString(), notif.IsRead);
            }

            CheckIfAllRead();
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
                NotificationRepository repository = new NotificationRepository();
                foreach (Control c in flowNotifications.Controls)
                {
                    if (c is Panel card)
                    {
                        var data = (NotificationTagData)c.Tag;
                        if (data.IsUnread)
                        {
                            
                            repository.MarkAsRead(data.ID);
                        }
                    }
                }
                btnMarkRead.Enabled = false;
                btnMarkRead.Text = "All Read";
                CheckIfAllRead();
                LoadNotifications();
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
                    var data = (NotificationTagData)card.Tag;
                    data.IsUnread = false;
                    card.Tag = data;
                    card.BackColor = Color.White;

                    card.Invalidate();
                }
            }
            // Resize handling (important)
            flowNotifications.Resize += (s, e) =>
            {
                foreach (Control c in flowNotifications.Controls)
                    c.Width = flowNotifications.ClientSize.Width - 30;
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnMarkRead);
            this.Controls.Add(flowNotifications);
        }

        public void AddNotification(int notifID,string title, string message, string time, bool isUnread)
        {
            Cursor initialCursor = isUnread ? Cursors.Hand : Cursors.Default;

            int currentWidth = flowNotifications.ClientSize.Width;

           
            if (currentWidth < 400)
            {
                currentWidth = 400;
            } 

            // Calculate card width based on this "safe" width
            int cardWidth = currentWidth - 30;
            // 2. Create the Card
            Panel card = new Panel
            {
                Width = cardWidth,
                Margin = new Padding(0, 0, 0, 15),
                Tag = new NotificationTagData { IsUnread = isUnread, ID = notifID },
                Cursor = initialCursor,
                Height = 100 // Default height, will be adjusted later
            };

            // 3. Title Label
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(25, 12),
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = initialCursor
            };

            // 4. Message Label (With Wrapping Fix)
            Label lblMsg = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.DimGray,
                Location = new Point(25, 40),
                AutoSize = true,

                // This ensures the text wraps properly instead of stacking vertically
                MaximumSize = new Size(cardWidth - 5, 0),

                BackColor = Color.Transparent,
                Cursor = initialCursor
            };

            // Add Msg now to calculate height
            card.Controls.Add(lblMsg);

            // 5. Time Label
            Label lblTime = new Label
            {
                Text = time,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                AutoSize = true,
                BackColor = Color.Transparent,
                Cursor = initialCursor,
                // Position relative to the bottom of the message
                Location = new Point(25, lblMsg.Bottom + 5)
            };

            // 6. Adjust Card Height dynamically
            card.Height = lblTime.Bottom + 15;

            // 7. Paint Logic
            card.Paint += (s, e) =>
            {
                var data = (NotificationTagData)card.Tag;
                bool isCurrentUnread = data.IsUnread;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (Brush bg = new SolidBrush(isCurrentUnread ? Color.FromArgb(235, 242, 255) : Color.White))
                {
                    e.Graphics.FillRectangle(bg, card.ClientRectangle);
                }

                if (isCurrentUnread)
                {
                    Rectangle borderRect = card.ClientRectangle;
                    borderRect.Inflate(-1, -1);
                    DrawRoundedRectangle(e.Graphics, new Pen(Color.FromArgb(100, 149, 237), 1), borderRect, 12);
                }
            };

            // 8. Click Events
            EventHandler markAsReadAction = (sender, e) =>
            {
                Control clickedControl = sender as Control;
                Panel targetCard = (clickedControl is Panel) ? (Panel)clickedControl : (Panel)clickedControl.Parent;
                var data = (NotificationTagData)targetCard.Tag;

                if (data.IsUnread)
                {
                    data.IsUnread = false;
                    targetCard.Tag = data; // Update tag back
                    targetCard.Cursor = Cursors.Default;
                    foreach (Control child in targetCard.Controls) child.Cursor = Cursors.Default;

                    targetCard.Invalidate();
                    CheckIfAllRead();
                    try
                    {
                        var repo = new NotificationRepository();
                        repo.MarkAsRead(data.ID);
                    }
                    catch (Exception ee){ MessageBox.Show(ee.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error); }
                }
            };

            card.Click += markAsReadAction;
            lblTitle.Click += markAsReadAction;
            lblMsg.Click += markAsReadAction;
            lblTime.Click += markAsReadAction;

            card.Controls.Add(lblTitle);
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
        private void CheckIfAllRead()
        {
            bool anyUnread = false;

            // Loop through all panels in the flow layout
            foreach (Control c in flowNotifications.Controls)
            {
                if (c is Panel card)
                {
                    // If we find ONE unread card, stop looking
                    var data = (NotificationTagData)card.Tag;
                    if (data.IsUnread)
                    {
                        anyUnread = true;
                        ///NotificationRepository repository = new NotificationRepository();

                        //repository.MarkAsRead(data.ID);
                        break;
                    }
                }
            }

            // Update the button based on the result
            if (anyUnread)
            {
                btnMarkRead.Enabled = true;
                btnMarkRead.Text = "Mark All as Read";
                btnMarkRead.BackColor = Color.FromArgb(90, 120, 240); // Blue
            }
            else
            {
                btnMarkRead.Enabled = false;
                btnMarkRead.Text = "All Read";
                btnMarkRead.BackColor = Color.LightGray; // Gray to look disabled
            }
        }
    }

    public class NotificationTagData
    {
        public bool IsUnread { get; set; }
        public int ID { get; set; }
    }
}
