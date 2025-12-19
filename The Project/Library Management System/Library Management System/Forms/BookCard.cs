using Library_Management_System.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class BookCard : UserControl
    {
        public event EventHandler OnBorrowClicked;
        public event EventHandler OnFavouriteClicked;
        public int CurrentUserID { get; set; }
      
        public BookCard(string title, string author, string imagePath, int bookID, int currentUserID)
        {
            InitializeComponent(title, author, imagePath);
            this.Tag = bookID;
            this.CurrentUserID = currentUserID;
        }
        private void InitializeComponent(string title, string author, string imagePath)
        {
            // --- Card Container Settings ---
            this.Size = new Size(200, 320);
            this.BackColor = Color.White;
            this.Margin = new Padding(15); // Adds space between cards in the FlowPanel

            // --- 1. Cover Image ---
            PictureBox cover = new PictureBox
            {
                Size = new Size(120, 160),
                Location = new Point(40, 15),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray // Fallback color if image fails
            };

            // Safe Image Loading
            try
            {
                if (!string.IsNullOrEmpty(imagePath)) cover.ImageLocation = imagePath;
            }
            catch { }

            this.Controls.Add(cover);

            // --- 2. Title Label ---
            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(10, 190),
                Size = new Size(180, 45), // Height for 2 lines of text
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.TopCenter
            };
            this.Controls.Add(lblTitle);

            // --- 3. Author Label ---
            Label lblAuthor = new Label
            {
                Text = "By: " + author,
                Location = new Point(10, 235),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.TopCenter
            };
            this.Controls.Add(lblAuthor);

            // --- 4. Borrow Button ---
            Button btnBorrow = new Button
            {
                Text = "Borrow",
                Size = new Size(100, 35),
                Location = new Point(20, 270),
                BackColor = Color.FromArgb(79, 70, 229), // Your Theme Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnBorrow.FlatAppearance.BorderSize = 0;

            // Link the button click to our custom event
            btnBorrow.Click += (s, e) => {
                // Check if someone is listening to the event, then trigger it
                OnBorrowClicked?.Invoke(this, EventArgs.Empty);
            };

            this.Controls.Add(btnBorrow);
        
            Button btnFav = new Button
            {
                Text = "♥",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Size = new Size(40, 40),
                Location = new Point(130, 270),
                BackColor = Color.White,
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnFav.FlatAppearance.BorderSize = 1;
            btnFav.FlatAppearance.BorderColor = Color.FromArgb(230, 230, 230);
            btnFav.Click += (s, e) =>
            {
                int bookID = (int)this.Tag; 
                int userID = this.CurrentUserID; 

                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        con.Open();

                        string checkQuery = "SELECT COUNT(*) FROM Favorites WHERE UserID = @UserID AND BookID = @BookID";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@UserID", userID);
                            checkCmd.Parameters.AddWithValue("@BookID", bookID);
                            int count = (int)checkCmd.ExecuteScalar();

                            if (count == 0)
                            {
                                string insertQuery = "INSERT INTO Favorites (UserID, BookID) VALUES (@UserID, @BookID)";
                                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@UserID", userID);
                                    cmd.Parameters.AddWithValue("@BookID", bookID);
                                    cmd.ExecuteNonQuery();
                                }
                                btnFav.ForeColor = Color.Red;
                                MessageBox.Show("Added to favorites ❤️");
                            }
                            else
                            {
                                string deleteQuery = "DELETE FROM Favorites WHERE UserID = @UserID AND BookID = @BookID";
                                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@UserID", userID);
                                    cmd.Parameters.AddWithValue("@BookID", bookID);
                                    cmd.ExecuteNonQuery();
                                }
                                btnFav.ForeColor = Color.Gray;
                                MessageBox.Show("Removed from favorites");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            };
            this.Controls.Add(btnFav);
           

            // Optional: Draw a subtle gray border around the card
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid);
            };
        }

        private void BookCard_Load(object sender, EventArgs e)
        {

        }
    }
}
