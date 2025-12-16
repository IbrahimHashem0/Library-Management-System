using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class ReaderHomeView : UserControl
    {
        private FlowLayoutPanel gridPanel;
        private TextBox searchBox; // Keep reference to read text later
        private const string PlaceholderText = "Search book by title or author...";
        private List<Button> categoryButtons = new List<Button>();
        private int currentCategoryId = 0;
        public ReaderHomeView()
        {
            InitializeComponent();
            Initialize();
            LoadBooksFromDatabase("", currentCategoryId); // Load Real Data
        }

        private void Initialize()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- 1. Top Section (Search & Filter) ---
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.White, Padding = new Padding(20) };

            // Search Bar
            Panel searchContainer = new Panel { Size = new Size(600, 40), Location = new Point(20, 20), BackColor = Color.White };

            searchBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12),
                Location = new Point(15, 10),
                Width = 500,
                Text = PlaceholderText,
                ForeColor = Color.Gray
            };
            searchBox.TextChanged += (s, e) => {
                // 1. If the text matches the placeholder, treat it as empty (Show All)
                // 2. Otherwise, search for whatever the user typed
                string searchTerm = searchBox.Text == PlaceholderText ? "" : searchBox.Text;

                LoadBooksFromDatabase(searchTerm, currentCategoryId);
            };
            // Search Placeholder Logic
            searchBox.GotFocus += (s, e) => { if (searchBox.Text == PlaceholderText) { searchBox.Text = ""; searchBox.ForeColor = Color.Black; } };
            
            // Search Button Logic
            Button searchIcon = new Button { Text = "🔍", Dock = DockStyle.Right, Width = 50, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(79, 70, 229), ForeColor = Color.White, Cursor = Cursors.Hand };
            searchIcon.Click += (s, e) => LoadBooksFromDatabase(searchBox.Text == PlaceholderText ? "" : searchBox.Text, currentCategoryId);

            searchContainer.Controls.Add(searchBox);
            searchContainer.Controls.Add(searchIcon);
            topPanel.Controls.Add(searchContainer);

            // Filter Pills (Buttons) - Visual Only for now
            FlowLayoutPanel filterPanel = new FlowLayoutPanel { Location = new Point(20, 75), Size = new Size(800, 50), AutoSize = true };
            try
            {
                CategoryRepository catRepo = new CategoryRepository();
                List<Category> categories = catRepo.GetAllCategories(); // Fetch from DB

                foreach (var cat in categories)
                {
                    Button pill = new Button
                    {
                        Text = cat.Name,
                        Tag = cat.CategoryID, // Store the real DB ID
                        AutoSize = true,
                        Padding = new Padding(15, 5, 15, 5),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Font = new Font("Segoe UI", 10),
                        // "All" (ID 0) is selected by default
                        BackColor = cat.CategoryID == 0 ? Color.FromArgb(79, 70, 229) : Color.White,
                        ForeColor = cat.CategoryID == 0 ? Color.White : Color.FromArgb(79, 70, 229)
                    };
                    pill.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);
                    pill.FlatAppearance.BorderSize = 1;

                    pill.Click += (s, e) => CategoryClicked(pill);

                    categoryButtons.Add(pill);
                    filterPanel.Controls.Add(pill);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load categories: " + ex.Message);
            }
            topPanel.Controls.Add(filterPanel);
            this.Controls.Add(topPanel);

            // --- 2. Grid Section (Flow Layout) ---
            Label sectionTitle = new Label { Text = "Trending Books", Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(20, 130), AutoSize = true };
            topPanel.Height += 40; // Adjust height for title
            topPanel.Controls.Add(sectionTitle);

            gridPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.White
            };
            this.Controls.Add(gridPanel);
            gridPanel.BringToFront();
        }


        private void LoadBooksFromDatabase(string searchTerm = "", int ID = 0)
        {
            gridPanel.Controls.Clear(); // Clear old cards
            
            try
            {
                BookRepository repo = new BookRepository();
                List<Book> allBooks = repo.GetAllBooks(); // Get everything from DB

                // Filter Logic (In Memory)
                var filteredBooks = allBooks;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    filteredBooks = allBooks
                        .Where(b => b.Title.ToLower().Contains(searchTerm) || b.Author.ToLower().Contains(searchTerm))
                        .ToList();
                }
                if (ID != 0)
                {
                    filteredBooks = filteredBooks
                            .Where(b => b.CategoryID == ID)
                            .ToList();
                }
                // Create a card for each book
                foreach (var book in filteredBooks)
                {
                    // Since DB has no Image column, use a placeholder based on ID
                    // This creates a consistent fake image URL or you can use a local resource
                    string placeholderImg = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=200";

                    AddCard(book, placeholderImg);
                }

                if (filteredBooks.Count == 0)
                {
                    Label lbl = new Label { Text = "No books found.", AutoSize = true, Font = new Font("Segoe UI", 14), ForeColor = Color.Gray };
                    gridPanel.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message);
            }
        }

        private void AddCard(Book book, string imgUrl)
        {
            // Create the card using the book's real data
            BookCard card = new BookCard(book.Title, book.Author, imgUrl);

            // Handle Borrow Click
            card.OnBorrowClicked += (s, e) => {
                // Here we have access to the specific 'book' object from the DB
                if (book.AvailableCopies > 0)
                {
                    MessageBox.Show($"Requesting to borrow: {book.Title}\n(ID: {book.BookID})");
                    // TODO: Call BorrowRepository logic here
                }
                else
                {
                    MessageBox.Show("Sorry, this book is currently out of stock.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            gridPanel.Controls.Add(card);
        }
        private void CategoryClicked(Button clickedBtn)
        {
            // 1. Update the ID
            currentCategoryId = (int)clickedBtn.Tag;

            // 2. Update Visuals (Purple for active, White for inactive)
            foreach (var btn in categoryButtons)
            {
                if (btn == clickedBtn)
                {
                    btn.BackColor = Color.FromArgb(79, 70, 229);
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.FromArgb(79, 70, 229);
                }
            }

            // 3. Reload Data
            LoadBooksFromDatabase("", currentCategoryId);
        }
    }
}
