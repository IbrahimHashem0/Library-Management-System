using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Library_Management_System.Data;
using Library_Management_System.Models;

namespace Library_Management_System.Repositories
{
    public class BookRepository
    {
        // Get All Books
        public List<Book> GetAllBooks()
        {
            var list = new List<Book>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Books";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Book
                            {
                                BookID = (int)reader["BookID"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                Publisher = reader["Publisher"].ToString(),
                                Year = reader["Year"] as int?,
                                CategoryID = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : 0,
                                TotalCopies = (int)reader["TotalCopies"],
                                AvailableCopies = (int)reader["AvailableCopies"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        // Add a New Book (For Admins)
        public void AddBook(Book book)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Books (Title, Author, ISBN, Publisher, Year, CategoryID, TotalCopies, AvailableCopies) 
                                 VALUES (@Title, @Author, @ISBN, @Publisher, @Year, @CategoryID, @TotalCopies, @AvailableCopies)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                    cmd.Parameters.AddWithValue("@Publisher", book.Publisher);
                    cmd.Parameters.AddWithValue("@Year", book.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", book.CategoryID);
                    cmd.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
                    cmd.Parameters.AddWithValue("@AvailableCopies", book.TotalCopies); // Initially, available = total

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // delete a Book (For Admins)
        public void DeleteBook(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Books WHERE BookID = @BookID";
                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@BookID" , bookId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // search Books by Title or Author
        public List<Book> SearchBooks(string searchTerm)
        {
            var list = new List<Book>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT * FROM Books 
                         WHERE Title LIKE @term OR Author LIKE @term OR ISBN LIKE @term";

                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@term" , "%" + searchTerm + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Book
                            {
                                BookID = (int)reader["BookID"] ,
                                Title = reader["Title"].ToString() ,
                                Author = reader["Author"].ToString() ,
                                ISBN = reader["ISBN"].ToString() ,
                                Publisher = reader["Publisher"].ToString() ,
                                Year = reader["Year"] as int? ,
                                CategoryID = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : 0 ,
                                TotalCopies = (int)reader["TotalCopies"] ,
                                AvailableCopies = (int)reader["AvailableCopies"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        //update Book Details
        public void UpdateBook(Book book)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = @"
            UPDATE Books 
            SET 
                Title = @Title,
                Author = @Author,
                ISBN = @ISBN,
                Publisher = @Publisher,
                CategoryID = @CategoryID,
                TotalCopies = @TotalCopies,
                AvailableCopies = @AvailableCopies
            WHERE BookID = @BookID";

                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@BookID" , book.BookID);
                    cmd.Parameters.AddWithValue("@Title" , book.Title);
                    cmd.Parameters.AddWithValue("@Author" , book.Author);
                    cmd.Parameters.AddWithValue("@ISBN" , book.ISBN);
                    cmd.Parameters.AddWithValue("@Publisher" , book.Publisher);
                    cmd.Parameters.AddWithValue("@Year" , book.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID" , book.CategoryID);
                    cmd.Parameters.AddWithValue("@TotalCopies" , book.TotalCopies);
                    cmd.Parameters.AddWithValue("@AvailableCopies" , book.AvailableCopies);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get Book by ID
        public Book GetBookById(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM Books WHERE BookID = @BookID";

                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@BookID" , bookId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Book
                            {
                                BookID = (int)reader["BookID"] ,
                                Title = reader["Title"].ToString() ,
                                Author = reader["Author"].ToString() ,
                                ISBN = reader["ISBN"] == DBNull.Value ? null : reader["ISBN"].ToString() ,
                                Publisher = reader["Publisher"] == DBNull.Value ? null : reader["Publisher"].ToString() ,
                                Year = reader["Year"] == DBNull.Value ? (int?)null : (int)reader["Year"] ,
                                CategoryID = reader["CategoryID"] == DBNull.Value ? 0 : (int)reader["CategoryID"] ,
                                TotalCopies = (int)reader["TotalCopies"] ,
                                AvailableCopies = (int)reader["AvailableCopies"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        // get Book by Title and Author
        public Book GetBookByTitleAndAuthor(string title , string author)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT * FROM Books 
                         WHERE LOWER(LTRIM(RTRIM(Title))) = @Title 
                           AND LOWER(LTRIM(RTRIM(Author))) = @Author";
                SqlCommand cmd = new SqlCommand(query , connection);
                cmd.Parameters.AddWithValue("@Title" , title.Trim().ToLower());
                cmd.Parameters.AddWithValue("@Author" , author.Trim().ToLower());

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Book
                    {
                        BookID = (int)reader["BookID"] ,
                        Title = reader["Title"].ToString() ,
                        Author = reader["Author"].ToString() ,
                        ISBN = reader["ISBN"] != DBNull.Value ? reader["ISBN"].ToString() : null ,
                        Publisher = reader["Publisher"] != DBNull.Value ? reader["Publisher"].ToString() : null ,
                        Year = reader["Year"] != DBNull.Value ? (int)reader["Year"] : 0 ,
                        CategoryID = (int)reader["CategoryID"] ,
                        TotalCopies = (int)reader["TotalCopies"] ,
                        AvailableCopies = (int)reader["AvailableCopies"]
                    };
                }
            }
            return null;
        }

        // get Book by ISBN
        public Book GetBookByISBN(string isbn)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Books WHERE ISBN=@ISBN";
                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@ISBN" , isbn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Book
                            {
                                BookID = (int)reader["BookID"] ,
                                Title = reader["Title"].ToString() ,
                                Author = reader["Author"].ToString() ,
                                ISBN = reader["ISBN"] != DBNull.Value ? reader["ISBN"].ToString() : null ,
                                Publisher = reader["Publisher"] != DBNull.Value ? reader["Publisher"].ToString() : null ,
                                Year = reader["Year"] != DBNull.Value ? (int)reader["Year"] : 0 ,
                                CategoryID = (int)reader["CategoryID"] ,
                                TotalCopies = (int)reader["TotalCopies"] ,
                                AvailableCopies = (int)reader["AvailableCopies"]
                            };
                        }
                    }
                }
            }
            return null;
        }
        // is book borrowed
        public bool IsBookBorrowed(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Borrowings WHERE BookID=@BookID AND Status='borrowed'";
                using (var cmd = new SqlCommand(query , conn))
                {
                    cmd.Parameters.AddWithValue("@BookID" , bookId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // true if book is borrowed
                }
            }
        }



    }
}