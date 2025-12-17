using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Library_Management_System.Repositories
{
    internal class BorrowingRepository
    {
        public string BorrowBook(int userId, int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. --- NEW CHECK: Does user already have this book? ---
                // We look for any record of this book for this user that is NOT 'Returned'
                string duplicateCheckQuery = @"
            SELECT COUNT(*) 
            FROM Borrowings 
            WHERE UserID = @UserID 
              AND BookID = @BookID 
              AND Status IN ('Borrowed', 'Overdue', 'borrowed')"; // Check your DB casing!

                using (var cmdCheckDup = new SqlCommand(duplicateCheckQuery, conn))
                {
                    cmdCheckDup.Parameters.AddWithValue("@UserID", userId);
                    cmdCheckDup.Parameters.AddWithValue("@BookID", bookId);
                    int count = (int)cmdCheckDup.ExecuteScalar();

                    if (count > 0)
                    {
                        return "You already have a copy of this book not returned yet.";
                    }
                }
                // ---------------------------------------------------------

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 2. Check Stock
                        string checkQuery = "SELECT AvailableCopies FROM Books WHERE BookID = @BookID";
                        int available = 0;

                        using (var cmdCheck = new SqlCommand(checkQuery, conn, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue("@BookID", bookId);
                            object result = cmdCheck.ExecuteScalar();
                            if (result != null) available = Convert.ToInt32(result);
                        }

                        if (available <= 0) return "Sorry, this book is currently out of stock.";

                        // 3. Decrease Stock
                        string updateQuery = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID = @BookID";
                        using (var cmdUpdate = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@BookID", bookId);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // 4. Create Borrow Record
                        string insertQuery = @"
                    INSERT INTO Borrowings (UserID, BookID, BorrowDate, DueDate, Status) 
                    VALUES (@UserID, @BookID, GETDATE(), DATEADD(day, 14, GETDATE()), 'Borrowed')";

                        using (var cmdInsert = new SqlCommand(insertQuery, conn, transaction))
                        {
                            cmdInsert.Parameters.AddWithValue("@UserID", userId);
                            cmdInsert.Parameters.AddWithValue("@BookID", bookId);
                            cmdInsert.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Error: " + ex.Message;
                    }
                }
            }
        }
        public List<BorrowHistoryItem> GetUserHistory(int userId)
        {
            var history = new List<BorrowHistoryItem>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // LOGIC: 
                // If ReturnDate exists -> 'Returned'
                // Else If DueDate < Today -> 'Overdue'
                // Else -> 'Borrowed'
                string query = @"
            SELECT 
                br.BorrowingID,
                b.Title, 
                b.Author, 
                br.BorrowDate, 
                br.DueDate,
                CASE 
                    WHEN br.ReturnDate IS NOT NULL THEN 'Returned'
                    WHEN br.DueDate < GETDATE() THEN 'Overdue'
                    ELSE 'Borrowed'
                END AS CalculatedStatus
            FROM Borrowings br
            JOIN Books b ON br.BookID = b.BookID
            WHERE br.UserID = @UserID
            ORDER BY br.BorrowDate DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new BorrowHistoryItem
                            {
                                BorrowingID = (int)reader["BorrowingID"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                BorrowDate = Convert.ToDateTime(reader["BorrowDate"]).ToString("dd MMM yyyy"),
                                DueDate = Convert.ToDateTime(reader["DueDate"]).ToString("dd MMM yyyy"),
                                Status = reader["CalculatedStatus"].ToString() // Uses the calculated logic
                            });
                        }
                    }
                }
            }
            return history;
        }

        // 2. NEW METHOD: RETURN BOOK
        public void ReturnBook(int borrowingId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Get BookID first to update stock
                        int bookId = 0;
                        string getBookIdQuery = "SELECT BookID FROM Borrowings WHERE BorrowingID = @ID";
                        using (var cmdGet = new SqlCommand(getBookIdQuery, conn, transaction))
                        {
                            cmdGet.Parameters.AddWithValue("@ID", borrowingId);
                            bookId = (int)cmdGet.ExecuteScalar();
                        }

                        // B. Update Borrowing Record (Set ReturnDate and Status)
                        string updateBorrowQuery = @"
                    UPDATE Borrowings 
                    SET ReturnDate = GETDATE(), Status = 'Returned' 
                    WHERE BorrowingID = @ID";
                        using (var cmdUpdateBorrow = new SqlCommand(updateBorrowQuery, conn, transaction))
                        {
                            cmdUpdateBorrow.Parameters.AddWithValue("@ID", borrowingId);
                            cmdUpdateBorrow.ExecuteNonQuery();
                        }

                        // C. Increase Book Stock (+1)
                        string updateStockQuery = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookID = @BookID";
                        using (var cmdStock = new SqlCommand(updateStockQuery, conn, transaction))
                        {
                            cmdStock.Parameters.AddWithValue("@BookID", bookId);
                            cmdStock.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
