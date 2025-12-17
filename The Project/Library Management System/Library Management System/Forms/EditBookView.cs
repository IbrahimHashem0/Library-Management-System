using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class EditBookView : UserControl
    {
        private int _bookId;
        private BookRepository _repo;
        private Book _currentBook;

        private TextBox titleTxt;
        private TextBox authorTxt;
        private TextBox isbnTxt;
        private TextBox publisherTxt;
        private NumericUpDown totalCopiesNum;
        private Button saveBtn;
        private Button cancelBtn;

        public EditBookView(int bookId)
        {
            _bookId = bookId;
            _repo = new BookRepository();

            InitializeUI();
            LoadBookData();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label header = new Label
            {
                Text = "Edit Book" ,
                Font = new Font("Segoe UI" , 20 , FontStyle.Bold) ,
                Location = new Point(20 , 20) ,
                AutoSize = true
            };
            this.Controls.Add(header);

            titleTxt = CreateTextBox("Title" , 80);
            authorTxt = CreateTextBox("Author" , 130);
            isbnTxt = CreateTextBox("ISBN" , 180);
            publisherTxt = CreateTextBox("Publisher" , 230);

            Label copiesLabel = new Label
            {
                Text = "Total Copies" ,
                Location = new Point(20 , 280) ,
                Font = new Font("Segoe UI" , 10 , FontStyle.Bold) ,
                AutoSize = true
            };
            this.Controls.Add(copiesLabel);

            totalCopiesNum = new NumericUpDown
            {
                Location = new Point(20 , 305) ,
                Width = 300 ,
                Minimum = 0 ,
                Maximum = 10000 ,
                Font = new Font("Segoe UI" , 11)
            };
            this.Controls.Add(totalCopiesNum);

            saveBtn = new Button
            {
                Text = "Save Changes" ,
                Location = new Point(20 , 360) ,
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
                Location = new Point(180 , 360) ,
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
                Font = new Font("Segoe UI" , 10 , FontStyle.Bold) ,
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

        // ================= Load Data =================
        private void LoadBookData()
        {
            _currentBook = _repo.GetBookById(_bookId);

            if (_currentBook == null)
            {
                MessageBox.Show("Book not found." , "Error" ,
                    MessageBoxButtons.OK , MessageBoxIcon.Error);
                return;
            }

            titleTxt.Text = _currentBook.Title;
            authorTxt.Text = _currentBook.Author;
            isbnTxt.Text = _currentBook.ISBN ?? "";
            publisherTxt.Text = _currentBook.Publisher ?? "";
            totalCopiesNum.Value = _currentBook.TotalCopies;
        }

        // ================= Save =================
        private void SaveBtn_Click(object sender , EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleTxt.Text) ||
                string.IsNullOrWhiteSpace(authorTxt.Text))
            {
                MessageBox.Show(
                    "Title and Author are required." ,
                    "Validation" ,
                    MessageBoxButtons.OK ,
                    MessageBoxIcon.Warning
                );
                return;
            }

            Book updatedBook = new Book
            {
                BookID = _bookId ,
                Title = titleTxt.Text.Trim() ,
                Author = authorTxt.Text.Trim() ,
                ISBN = string.IsNullOrWhiteSpace(isbnTxt.Text) ? null : isbnTxt.Text.Trim() ,
                Publisher = string.IsNullOrWhiteSpace(publisherTxt.Text) ? null : publisherTxt.Text.Trim() ,
                CategoryID = _currentBook.CategoryID ,
                TotalCopies = (int)totalCopiesNum.Value ,

                // IMPORTANT: don't break borrowing logic
                AvailableCopies = Math.Min(
                    _currentBook.AvailableCopies ,
                    (int)totalCopiesNum.Value
                )
            };

            _repo.UpdateBook(updatedBook);

            MessageBox.Show(
                "Book updated successfully." ,
                "Success" ,
                MessageBoxButtons.OK ,
                MessageBoxIcon.Information
            );

            ReturnToManageBooks();
        }

        private void CancelBtn_Click(object sender , EventArgs e)
        {
            ReturnToManageBooks();
        }

        // ================= Navigation =================
        private void ReturnToManageBooks()
        {
            Control container = this.Parent;

            // climb up until we find a Panel (contentPanel)
            while (container != null && !(container is Panel))
            {
                container = container.Parent;
            }

            if (container != null)
            {
                container.Controls.Clear();
                container.Controls.Add(new ManageBooksView());
            }
            else
            {
                MessageBox.Show(
                    "Navigation error: container not found." ,
                    "Error" ,
                    MessageBoxButtons.OK ,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
