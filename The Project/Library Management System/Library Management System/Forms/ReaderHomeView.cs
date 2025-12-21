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
using System.Runtime.InteropServices;


namespace Library_Management_System.Forms
{
    public partial class ReaderHomeView : UserControl
    {
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        // ------------------------------------------

        private FlowLayoutPanel gridPanel;
        private TextBox searchBox; 
        private const string PlaceholderText = "Search book by title or author...";
        private List<Button> categoryButtons = new List<Button>();
        private int currentCategoryId = 0;
        private User _currentUser;

        //public ReaderHomeView(User currentUser)
        //{
        //    InitializeComponent();
        //    Initialize();
        //    LoadBooksFromDatabase("", currentCategoryId); // Load Real Data
        //    _currentUser = currentUser;
        //}
        public ReaderHomeView(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;      
            Initialize();
            LoadBooksFromDatabase("", currentCategoryId); 
        }

        private void Initialize()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;

            // --- 1. Top Section (Search & Filter) ---
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.WhiteSmoke, Padding = new Padding(20) };

            // Search Bar
            Panel searchContainer = new Panel { Size = new Size(600, 40), Location = new Point(20, 20), BackColor = Color.White };

            searchBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12),
                Location = new Point(15, 10),
                Width = 500,

                ForeColor = Color.Black
            };
            searchBox.TextChanged += (s, e) =>
            {
                
                string searchTerm = searchBox.Text == PlaceholderText ? "" : searchBox.Text;

                LoadBooksFromDatabase(searchTerm, currentCategoryId);
            };

            SendMessage(searchBox.Handle, EM_SETCUEBANNER, 1, "Search book by title or author...");

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

            
            Label sectionTitle = new Label {
                Text = "Trending Books",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 130),
                AutoSize = true
            };

            topPanel.Controls.Add(sectionTitle);
            this.Controls.Add(topPanel);

           
            Panel scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true, // This panel handles the scrollbar now
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20) // Add padding here for the edges
            };


            gridPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, // Sticks to the top of the container
                AutoSize = true,      // GROWS vertically as you add books
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.WhiteSmoke
            };

            // Connect them
            scrollContainer.Controls.Add(gridPanel); // Put grid INSIDE scroll container
            this.Controls.Add(scrollContainer);      // Put scroll container on the form
            scrollContainer.BringToFront();
        }

        private string GetCoverPath(string ISBN)
        {
            // 1. Define Default
            string Image = $"{ISBN}.jpg";
            string localFilePath = System.IO.Path.Combine(Application.StartupPath, "book_covers", Image);

            

            // 2. Check if the local file exists
            if (System.IO.File.Exists(localFilePath))
            {
                Image = localFilePath; // Use local file
            }
            else
            {
                // 3. Fallback to OpenLibrary if local file is missing
                
                    Image = $"https://covers.openlibrary.org/b/isbn/{ISBN}-L.jpg";
                
                
            }

            return Image;
        }
        private void LoadBooksFromDatabase(string searchTerm = "", int ID = 0)
        {
            gridPanel.SuspendLayout();
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

                filteredBooks = filteredBooks
            .OrderBy(b => b.Title) // Sorts A -> Z
            .ToList();
                // Create a card for each book
                foreach (var book in filteredBooks)
                {
                    

                    AddCard(book, GetCoverPath(book.ISBN));
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
            gridPanel.ResumeLayout();
        }

        private void AddCard(Book book, string imgUrl)
        {
            // Create the card using the book's real data
           // BookCard card = new BookCard(book.Title, book.Author, imgUrl);
            BookCard card = new BookCard(book.Title, book.Author, imgUrl, book.BookID, _currentUser.UserID);
            if (card.isFav())
                card.btnFav.ForeColor = Color.Red;
            else
                card.btnFav.ForeColor = Color.Gray;
            // LOGIC: Handle the Click Event
            card.OnBorrowClicked += (s, e) => {


                var confirmResult = MessageBox.Show(
                    $"Do you want to borrow '{book.Title}' with Borrowing price {book.BorrowPrice.ToString("c")}?",
                    "Confirm Borrow",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    // 3. Call the Repository
                    BorrowingRepository borrowRepo = new BorrowingRepository();
                    string result = borrowRepo.BorrowBook(_currentUser.UserID, book.BookID);

                    if (result == "Success")
                    {
                        MessageBox.Show("Book borrowed successfully! It is due in 14 days.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 4. Refresh the Grid to show updated copy count (Copies: 5 -> 4)
                        LoadBooksFromDatabase(searchBox.Text);
                    }
                    else
                    {
                        MessageBox.Show(result, "Borrow Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            gridPanel.Controls.Add(card);
        }
        private void CategoryClicked(Button clickedBtn)
        {
            if((int)clickedBtn.Tag == currentCategoryId)
            {
                return;
            }
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

        private void ReaderHomeView_Load(object sender, EventArgs e)
        {

        }
    }
}
