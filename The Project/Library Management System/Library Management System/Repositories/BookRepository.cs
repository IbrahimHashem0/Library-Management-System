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
    }
}