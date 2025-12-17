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
    public partial class ManageBooksView : UserControl
    {
        private DataGridView booksGrid;
        private TextBox searchTextBox;
        private Button addBookBtn;
        public ManageBooksView()
        {
            InitializeUI();
            LoadBooks();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // ===== HEADER =====
            Label title = new Label
            {
                Text = "Manage Books" ,
                Font = new Font("Segoe UI" , 20 , FontStyle.Bold) ,
                Location = new Point(20 , 20) ,
                AutoSize = true
            };
            this.Controls.Add(title);

            // ===== ADD BOOK BUTTON =====
            addBookBtn = new Button
            {
                Text = "+ Add New Book" ,
                Size = new Size(160 , 35) ,
                Location = new Point(this.Width - 190 , 25) ,
                Anchor = AnchorStyles.Top | AnchorStyles.Right ,
                BackColor = Color.FromArgb(79 , 70 , 229) ,
                ForeColor = Color.White ,
                FlatStyle = FlatStyle.Flat ,
                Cursor = Cursors.Hand
            };
            this.Controls.Add(addBookBtn);

            // ===== SEARCH =====
            searchTextBox = new TextBox
            {
                Location = new Point(20 , 80) ,
                Width = this.Width - 40 ,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right ,
                Font = new Font("Segoe UI" , 11) ,
                Text = "Search book by title or author..." ,
                ForeColor = Color.Gray
            };

            // Placeholder behavior
            searchTextBox.GotFocus += (s , e) =>
            {
                if (searchTextBox.Text == "Search book by title or author...")
                {
                    searchTextBox.Text = "";
                    searchTextBox.ForeColor = Color.Black;
                }
            };

            searchTextBox.LostFocus += (s , e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    searchTextBox.Text = "Search book by title or author...";
                    searchTextBox.ForeColor = Color.Gray;
                }
            };

            // ===== HANDLE ENTER KEY FOR SEARCH =====
            searchTextBox.KeyDown += (s , e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    LoadBooks(searchTextBox.Text);
                }
            };

            this.Controls.Add(searchTextBox);

            // ===== DATA GRID =====
            booksGrid = new DataGridView
            {
                Location = new Point(20 , 140) ,
                Size = new Size(this.Width - 40 , this.Height - 180) ,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right ,
                AllowUserToAddRows = false ,
                ReadOnly = true ,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill ,
                BackgroundColor = Color.White ,
                BorderStyle = BorderStyle.None ,
                RowTemplate = { Height = 40 } ,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect ,
                Cursor = Cursors.Hand
            };

            booksGrid.Columns.Add("BookID" , "ID");
            booksGrid.Columns["BookID"].Visible = false;

            booksGrid.Columns.Add("Title" , "Book Title");
            booksGrid.Columns.Add("Author" , "Author");
            booksGrid.Columns.Add("Category" , "Category");
            booksGrid.Columns.Add("Total" , "Total Copies");
            booksGrid.Columns.Add("Available" , "Available Copies");
            booksGrid.Columns.Add("Status" , "Status");

            // Actions
            booksGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Edit" ,
                Text = "Edit" ,
                UseColumnTextForButtonValue = true
            });

            booksGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Delete" ,
                Text = "Delete" ,
                UseColumnTextForButtonValue = true
            });

            this.Controls.Add(booksGrid);
            booksGrid.CellContentClick += BooksGrid_CellContentClick;

        }
        // ===== HANDLE BUTTON CLICKS IN DATA GRID =====
        private void BooksGrid_CellContentClick(object sender , DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Check if Delete button clicked
            if (booksGrid.Columns[e.ColumnIndex].Name == "Delete")
            {
                int bookId = Convert.ToInt32(booksGrid.Rows[e.RowIndex].Cells["BookID"].Value);

                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this book?" ,
                    "Confirm Delete" ,
                    MessageBoxButtons.YesNo ,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    var repo = new BookRepository();
                    repo.DeleteBook(bookId);

                    // Remove from grid
                    booksGrid.Rows.RemoveAt(e.RowIndex);

                    MessageBox.Show(
                        "Book deleted successfully." ,
                        "Success" ,
                        MessageBoxButtons.OK ,
                        MessageBoxIcon.Information
                    );
                }
            }
            // Check if Edit button clicked
            else if (booksGrid.Columns[e.ColumnIndex].Name == "Edit")
            {
                if (booksGrid.Rows[e.RowIndex].Cells["BookID"].Value == null)
                    return;

                int bookId = Convert.ToInt32(booksGrid.Rows[e.RowIndex].Cells["BookID"].Value);
                EditBookView editView = new EditBookView(bookId);

                Control container = this.Parent;

                while (container != null && !(container is Panel))
                {
                    container = container.Parent;
                }

                if (container != null)
                {
                    container.Controls.Clear();
                    container.Controls.Add(editView);
                }
            }
        }

        // ===== load data from database =====
        private void LoadBooks(string searchTerm = "")
        {
            booksGrid.Rows.Clear();

            var repo = new BookRepository();
            List<Book> books;

            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Search book by title or author...")
            {
                books = repo.GetAllBooks();
            }
            else
            {
                books = repo.SearchBooks(searchTerm); 
            }

            foreach (var book in books)
            {
                string status =
                    book.AvailableCopies == 0 ? "Out of Stock" :
                    book.AvailableCopies < 5 ? "Low Stock" :
                    "Available";

                booksGrid.Rows.Add(
                    book.BookID ,
                    book.Title ,
                    book.Author ,
                    book.CategoryID ,
                    book.TotalCopies ,
                    book.AvailableCopies ,
                    status
                );
            }
        }


    }
}
