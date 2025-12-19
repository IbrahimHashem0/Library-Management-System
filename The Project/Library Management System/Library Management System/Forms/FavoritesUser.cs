
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Library_Management_System.Data; 

namespace Library_Management_System.Forms
{
    public partial class FavoritesUserControl : UserControl
    {
        private FlowLayoutPanel flowFavorites;

        public FavoritesUserControl()
        {
            BuildControlManually();
        }

        private void BuildControlManually()
        {
            this.BackColor = Color.White;
            this.Padding = new Padding(20);

            Label lblTitle = new Label();
            lblTitle.Text = "My Favorite Books";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 10);

            flowFavorites = new FlowLayoutPanel();
            flowFavorites.AutoScroll = true;
            flowFavorites.FlowDirection = FlowDirection.LeftToRight;
            flowFavorites.WrapContents = true;
            flowFavorites.Padding = new Padding(10);
            flowFavorites.BackColor = Color.Transparent;
            flowFavorites.Location = new Point(0, 70);
            flowFavorites.Size = new Size(this.Width, this.Height - 80);
            flowFavorites.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(lblTitle);
            this.Controls.Add(flowFavorites);

            this.Resize += (s, e) =>
            {
                flowFavorites.Size = new Size(this.Width - 40, this.Height - 100);
            };
        }

        public void LoadFavorites(int userID)
        {
            flowFavorites.Controls.Clear();

            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT b.BookID, b.Title, b.Author, b.ISBN
                        FROM Books b
                        INNER JOIN Favorites f ON b.BookID = f.BookID
                        WHERE f.UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int bookID = reader.GetInt32(reader.GetOrdinal("BookID")); 
                                string title = reader.GetString(reader.GetOrdinal("Title"));
                                string author = reader.GetString(reader.GetOrdinal("Author"));
                                string isbn = reader.IsDBNull(reader.GetOrdinal("ISBN"))
                                    ? ""
                                    : reader.GetString(reader.GetOrdinal("ISBN"));

                                string coverPath = GetCoverPath(isbn);
                                Image bookImage = LoadImageFromPath(coverPath);

                                CreateBookCard(title, author, bookImage, bookID, userID);
                            }
                        }
                    }

                    if (flowFavorites.Controls.Count == 0)
                    {
                        Label lblEmpty = new Label();
                        lblEmpty.Text = "No favorite books yet!\nGo explore and add some ❤️";
                        lblEmpty.Font = new Font("Segoe UI", 14F, FontStyle.Italic);
                        lblEmpty.ForeColor = Color.Gray;
                        lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
                        lblEmpty.AutoSize = false;
                        lblEmpty.Size = new Size(flowFavorites.Width - 20, 200);
                        lblEmpty.Location = new Point(10, 50);
                        flowFavorites.Controls.Add(lblEmpty);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading favorites: " + ex.Message);
                }
            }
        }

        private string GetCoverPath(string isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return null;

            string imageName = $"{isbn}.jpg";
            string localFilePath = Path.Combine(Application.StartupPath, "book_covers", imageName);

            if (File.Exists(localFilePath))
            {
                return localFilePath;
            }
            else
            {
                return $"https://covers.openlibrary.org/b/isbn/{isbn}-L.jpg";
            }
        }

        private Image LoadImageFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            try
            {
                if (path.StartsWith("http"))
                {
                    using (WebClient client = new WebClient())
                    {
                        byte[] data = client.DownloadData(path);
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            return Image.FromStream(ms);
                        }
                    }
                }
                else
                {
                    return Image.FromFile(path);
                }
            }
            catch
            {
                return null;
            }
        }

        private void CreateBookCard(string title, string author, Image bookImage, int bookID, int currentUserID)
        {
            Panel card = new Panel();
            card.Size = new Size(220, 360);
            card.Margin = new Padding(15);
            card.BackColor = Color.White;

            card.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Brush shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                    DrawRoundedRectangle(g, shadow, new Rectangle(8, 8, card.Width - 16, card.Height - 16), 20);
                using (Brush brush = new SolidBrush(Color.White))
                    DrawRoundedRectangle(g, brush, new Rectangle(0, 0, card.Width, card.Height), 20);
            };

            PictureBox pb = new PictureBox();
            pb.Size = new Size(180, 250);
            pb.Location = new Point(20, 15);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.BorderStyle = BorderStyle.None;

            if (bookImage != null)
            {
                pb.Image = bookImage;
            }
            else
            {
                pb.BackColor = Color.LightGray;
                pb.Paint += (s, e) =>
                {
                    Graphics g = e.Graphics;
                    g.Clear(Color.LightGray);
                    string text = "No Cover";
                    Font font = new Font("Segoe UI", 12F, FontStyle.Italic);
                    SizeF size = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.DarkGray, (pb.Width - size.Width) / 2, (pb.Height - size.Height) / 2);
                };
            }

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.MaximumSize = new Size(180, 0);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(20, 275);

            Label lblAuthor = new Label();
            lblAuthor.Text = "Author: " + author;
            lblAuthor.Font = new Font("Segoe UI", 9F);
            lblAuthor.ForeColor = Color.Gray;
            lblAuthor.TextAlign = ContentAlignment.MiddleCenter;
            lblAuthor.Location = new Point(20, 305);

            Button btnBorrow = new Button();
            btnBorrow.Text = "Borrow";
            btnBorrow.Size = new Size(90, 35);
            btnBorrow.Location = new Point(20, 320);
            btnBorrow.BackColor = Color.FromArgb(0, 123, 255);
            btnBorrow.ForeColor = Color.White;
            btnBorrow.FlatStyle = FlatStyle.Flat;
            btnBorrow.FlatAppearance.BorderSize = 0;
            btnBorrow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBorrow.Click += (s, e) => {
                var confirmResult = MessageBox.Show($"Are you sure you want to borrow '{title}'?", "Confirm Borrow", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        Library_Management_System.Repositories.BorrowingRepository borrowRepo = new Library_Management_System.Repositories.BorrowingRepository();

                        string result = borrowRepo.BorrowBook(currentUserID, bookID);

                        MessageBox.Show(result); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred during borrowing: " + ex.Message);
                    }
                }
            };
            Button btnRemove = new Button();
            btnRemove.Text = "Remove";
            btnRemove.Size = new Size(90, 35);
            btnRemove.Location = new Point(110, 320);
            btnRemove.BackColor = Color.FromArgb(220, 53, 69);
            btnRemove.ForeColor = Color.White;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemove.Click += (s, e) =>
            {
                if (MessageBox.Show("Remove this book from favorites?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection con = DatabaseHelper.GetConnection())
                    {
                        try
                        {
                            con.Open();
                            string deleteQuery = "DELETE FROM Favorites WHERE UserID = @UserID AND BookID = @BookID";
                            using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@UserID", currentUserID);
                                cmd.Parameters.AddWithValue("@BookID", bookID);
                                int affected = cmd.ExecuteNonQuery();
                                if (affected > 0)
                                {
                                    flowFavorites.Controls.Remove(card);
                                    MessageBox.Show("Removed successfully! ❤️");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }
            };

            card.Controls.Add(pb);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblAuthor);
            card.Controls.Add(btnBorrow);
            card.Controls.Add(btnRemove);

            flowFavorites.Controls.Add(card);
        }

        private void DrawRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                g.FillPath(brush, path);
            }
        }

        private void FavoritesUserControl_Load(object sender, EventArgs e)
        {

        }
    }
}