using Library_Management_System.Models;
using Library_Management_System.Repositories;
using Library_Management_System.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Library_Management_System.Forms
{
    public partial class AddBookView : UserControl
    {
        private readonly BookRepository _bookRepo;
        private readonly CategoryRepository _categoryRepo;

        private TextBox titleTxt;
        private TextBox authorTxt;
        private TextBox isbnTxt;
        private TextBox publisherTxt, BorrowingPricetxt;
        private NumericUpDown totalCopiesNum;
        private ComboBox categoryCombo;
        private NumericUpDown yearNum;
        private Button saveBtn;
        private Button cancelBtn;
        public AddBookView()
        {
            _bookRepo = new BookRepository();
            _categoryRepo = new CategoryRepository();

            InitializeUI();
            LoadCategories();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            Label header = new Label
            {
                Text = "Add New Book" ,
                Font = new Font("Segoe UI" , 22 , FontStyle.Bold) ,
                Location = new Point(20 , 20) ,
                ForeColor = Color.FromArgb(63 , 81 , 181) ,
                AutoSize = true
            };
            this.Controls.Add(header);

            titleTxt = CreateTextBox("Title" , 100);
            authorTxt = CreateTextBox("Author" , 170);
            isbnTxt = CreateTextBox("ISBN" , 240);
            publisherTxt = CreateTextBox("Publisher" , 310);
            BorrowingPricetxt= CreateTextBox("Borrowing Price", 370);

            // ===== Year =====
            Label yearLabel = new Label
            {
                Text = "Year" ,
                Location = new Point(20 , 420) , 
                Font = new Font("Segoe UI" , 11 , FontStyle.Bold) ,
                AutoSize = true
            };
            this.Controls.Add(yearLabel);

            yearNum = new NumericUpDown
            {
                Location = new Point(20 , 445) ,
                Width = 300 ,
                Minimum = 1000 ,
                Maximum = DateTime.Now.Year ,
                Font = new Font("Segoe UI" , 11) ,
                Value = DateTime.Now.Year
            };
            this.Controls.Add(yearNum);

            // ===== Total Copies =====
            Label copiesLabel = new Label
            {
                Text = "Total Copies" ,
                Location = new Point(20 , 480) ,
                Font = new Font("Segoe UI" , 11 , FontStyle.Bold) ,
                AutoSize = true
            };
            this.Controls.Add(copiesLabel);

            totalCopiesNum = new NumericUpDown
            {
                Location = new Point(20 , 505) ,
                Width = 300 ,
                Minimum = 1 ,
                Maximum = 10000 ,
                Font = new Font("Segoe UI" , 11) ,
                Value = 1
            };
            this.Controls.Add(totalCopiesNum);

            // ===== Category =====
            Label categoryLabel = new Label
            {
                Text = "Category" ,
                Location = new Point(20 , 535) ,
                Font = new Font("Segoe UI" , 11 , FontStyle.Bold) ,
                AutoSize = true
            };
            this.Controls.Add(categoryLabel);

            categoryCombo = new ComboBox
            {
                Location = new Point(20 , 555) ,
                Width = 300 ,
                Font = new Font("Segoe UI" , 11) ,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(categoryCombo);

            // ===== Buttons =====
            saveBtn = new Button
            {
                Text = "Add Book" ,
                Location = new Point(20 , 600) ,
                Size = new Size(140 , 40) ,
                BackColor = Color.FromArgb(79 , 70 , 229) ,
                ForeColor = Color.White ,
                FlatStyle = FlatStyle.Flat ,
                Cursor = Cursors.Hand
            };
            saveBtn.Click += SaveBtn_Click;

            cancelBtn = new Button
            {
                Text = "Cancel" ,
                Location = new Point(180 , 600) ,
                Size = new Size(140 , 40) ,
                FlatStyle = FlatStyle.Flat ,
                Cursor = Cursors.Hand
            };
            cancelBtn.Click += CancelBtn_Click;

            this.Controls.Add(saveBtn);
            this.Controls.Add(cancelBtn);
        }

        // ================= Helpers =================
        private TextBox CreateTextBox(string labelText , int y)
        {
            Label label = new Label
            {
                Text = labelText ,
                Location = new Point(20 , y - 20) ,
                Font = new Font("Segoe UI" , 11 , FontStyle.Bold) ,
                AutoSize = true
            };
            this.Controls.Add(label);

            TextBox txt = new TextBox
            {
                Location = new Point(20 , y) ,
                Width = 300 ,
                Font = new Font("Segoe UI" , 11)
            };
            this.Controls.Add(txt);

            return txt;
        }

        // ================= Load Categories =================
        private void LoadCategories()
        {
            var categories = _categoryRepo.GetAllCategories();

            // Remove "All" from add page
            categories.RemoveAll(c => c.CategoryID == 0);

            categoryCombo.DataSource = categories;
            categoryCombo.DisplayMember = "Name";
            categoryCombo.ValueMember = "CategoryID";
        }

        // ================= Save =================
        private void SaveBtn_Click(object sender , EventArgs e)
        {
            // ===== Validation =====
            if (string.IsNullOrWhiteSpace(titleTxt.Text))
            {
                MessageBox.Show("Title is required." , "Validation Error" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(authorTxt.Text))
            {
                MessageBox.Show("Author is required." , "Validation Error" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(isbnTxt.Text))
            {
                MessageBox.Show("ISBN is required." , "Validation Error" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(publisherTxt.Text))
            {
                MessageBox.Show("Publisher is required." , "Validation Error" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BorrowingPricetxt.Text))
            {
                MessageBox.Show("Borrowing Price is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Convert.ToDecimal(BorrowingPricetxt.Text) < 1)
            {
                MessageBox.Show("Borrwing Price should be more than or equal to 1.00$.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string title = titleTxt.Text.Trim();
            string author = authorTxt.Text.Trim();
            string isbn = isbnTxt.Text.Trim();
            string publisher = publisherTxt.Text.Trim();
            int year = (int)yearNum.Value;
            int categoryId = (int)categoryCombo.SelectedValue;
            int totalCopies = (int)totalCopiesNum.Value;
            
            if (Regex.IsMatch(author, @"[^a-zA-Z .]"))
            {
                MessageBox.Show("Author Name should conatin only letters", "Invalid Author Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            

            if (isbn.Length!=13 && isbn.Length != 10)
            {
                MessageBox.Show("ISBN should consist of 10 or 13 character", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // ===== Check for duplicates =====
            var existingBookByTitleAuthor = _bookRepo.GetBookByTitleAndAuthor(title , author);
            if (existingBookByTitleAuthor != null)
            {
                MessageBox.Show("This book (Title + Author) already exists." , "Duplicate Book" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            var existingBookByISBN = _bookRepo.GetBookByISBN(isbn);
            if (existingBookByISBN != null)
            {
                MessageBox.Show("This ISBN already exists." , "Duplicate ISBN" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            Book newBook = new Book
            {
                Title = title ,
                Author = author ,
                ISBN = isbn ,
                Publisher = publisher ,
                Year = year ,
                CategoryID = categoryId ,
                TotalCopies = totalCopies ,
                AvailableCopies = totalCopies,
                BorrowPrice = Convert.ToDecimal(BorrowingPricetxt.Text)
            };

            try
            {
                _bookRepo.AddBook(newBook);
                MessageBox.Show("Book added successfully." , "Success" , MessageBoxButtons.OK , MessageBoxIcon.Information);
                ReturnToManageBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the book: " + ex.Message , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
            }
        }
        private void CancelBtn_Click(object sender , EventArgs e)
        {
            ReturnToManageBooks();
        }

        // ================= Navigation =================
        private void ReturnToManageBooks()
        {
            Control container = this.Parent;

            while (container != null && !(container is Panel))
            {
                container = container.Parent;
            }

            if (container != null)
            {
                container.Controls.Clear();
                container.Controls.Add(new ManageBooksView());
            }
        }

        private void AddBookView_Load(object sender, EventArgs e)
        {

        }
    }
}
