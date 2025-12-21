using Library_Management_System.Data;
using Library_Management_System.Models;
using Library_Management_System.Repositories;

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
        public List<BorrowHistoryItem> GetAllBorrowingsForAdmin(string searchTerm = "")
        {
            var list = new List<BorrowHistoryItem>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT br.BorrowingID, u.FullName AS UserName, b.Title, b.Author, 
                   br.BorrowDate, br.ReturnDate, br.Status
            FROM Borrowings br
            JOIN Users u ON br.UserID = u.UserID
            JOIN Books b ON br.BookID = b.BookID
            WHERE u.FullName LIKE @search 
               OR b.Title LIKE @search 
               OR b.Author LIKE @search
            ORDER BY br.BorrowDate DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new BorrowHistoryItem
                            {
                                BorrowingID = (int)reader["BorrowingID"],
                                UserName = reader["UserName"].ToString(),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                BorrowDate = Convert.ToDateTime(reader["BorrowDate"]).ToString("dd-MM-yyyy"),
                                ReturnDate = reader["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReturnDate"]).ToString("dd-MM-yyyy") : "---",
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public string BorrowBook(int userId, int bookId)
        {
            // We need the title for the notification later, so let's track it
            string bookTitle = "";

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. --- Check for duplicates ---
                string duplicateCheckQuery = @"
            SELECT COUNT(*) 
            FROM Borrowings 
            WHERE UserID = @UserID 
              AND BookID = @BookID 
              AND Status IN ('Borrowed', 'Overdue', 'borrowed')";

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

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 2. Check Stock AND Get Title AND Price
                        string checkQuery = "SELECT AvailableCopies, Title, BorrowingPrice FROM Books WHERE BookID = @BookID";
                        int available = 0;
                        decimal borrowingPrice = 0m;

                        using (var cmdCheck = new SqlCommand(checkQuery, conn, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue("@BookID", bookId);
                            using (var reader = cmdCheck.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    available = Convert.ToInt32(reader["AvailableCopies"]);
                                    bookTitle = reader["Title"].ToString();
                                    borrowingPrice = reader["BorrowingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["BorrowingPrice"]) : 0m;
                                }
                            }
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

                        // 5. Create Bill (Payment Record)
                        string insertBillQuery = @"
                    INSERT INTO Payments (UserID, BookID, BorrowingPrice, PaymentDate, Status) 
                    VALUES (@UserID, @BookID, @Price, GETDATE(), 'Pending')";

                        using (var cmdBill = new SqlCommand(insertBillQuery, conn, transaction))
                        {
                            cmdBill.Parameters.AddWithValue("@UserID", userId);
                            cmdBill.Parameters.AddWithValue("@BookID", bookId);
                            cmdBill.Parameters.AddWithValue("@Price", borrowingPrice);
                            cmdBill.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        
                        try
                        {
                            var notifRepo = new NotificationRepository();
                            notifRepo.AddNotification(userId, "Borrow Success", $"You successfully borrowed '{bookTitle}'. Due date is in 14 days.");
                        }
                        catch { /* Fail silently on notification error to not confuse user about borrow success */ }

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
            int userId = 0;
            string bookTitle = "";

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Get BookID, TIitle, UserID AND DueDate first (needed for fine calc)
                        int bookId = 0;
                        DateTime dueDate = DateTime.MinValue;

                        string getBookInfoQuery = @"
                            SELECT br.BookID, br.UserID, br.DueDate, b.Title 
                            FROM Borrowings br
                            JOIN Books b ON br.BookID = b.BookID
                            WHERE br.BorrowingID = @ID";

                        using (var cmdGet = new SqlCommand(getBookInfoQuery, conn, transaction))
                        {
                            cmdGet.Parameters.AddWithValue("@ID", borrowingId);
                            using (var reader = cmdGet.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bookId = (int)reader["BookID"];
                                    userId = (int)reader["UserID"];
                                    bookTitle = reader["Title"].ToString();
                                    dueDate = Convert.ToDateTime(reader["DueDate"]);
                                }
                            }
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

                        // --- NEW: Calculate Fine if Overdue ---
                        try
                        {
                            if (DateTime.Now > dueDate)
                            {
                                int overdueDays = (DateTime.Now - dueDate).Days;
                                if (overdueDays > 0)
                                {
                                    decimal fineAmount = overdueDays * 1.0m; // $1 per day

                                    // 1. Create Fine Record
                                    string insertFineQuery = @"
                                INSERT INTO Payments (UserID, BookID, BorrowingPrice, PaymentDate, Status) 
                                VALUES (@UserID, @BookID, @Price, GETDATE(), 'Pending')";

                                    using (var cmdFine = new SqlCommand(insertFineQuery, conn, transaction))
                                    {
                                        cmdFine.Parameters.AddWithValue("@UserID", userId);
                                        cmdFine.Parameters.AddWithValue("@BookID", bookId);
                                        cmdFine.Parameters.AddWithValue("@Price", fineAmount);
                                        cmdFine.ExecuteNonQuery();
                                    }

                                    // 2. Add to Notification parameters
                                    // We'll handle notification below, just modifying the message logic
                                }
                            }
                        }
                        catch { }

                        // --- NEW: Send Notification ---
                        try
                        {
                            if (userId > 0)
                            {
                                var notifRepo = new NotificationRepository();
                                string message = $"You have successfully returned '{bookTitle}'. Thank you!";

                                // Check if overdue again for message context (simple logic since we are outside transaction scope for safe notification)
                                if (DateTime.Now > dueDate)
                                {
                                    int days = (DateTime.Now - dueDate).Days;
                                    if (days > 0)
                                    {
                                        decimal fine = days * 1.0m;
                                        message = $"You returned '{bookTitle}' {days} days late. A fine of {fine:C2} has been applied.";
                                        notifRepo.AddNotification(userId, "Overdue Return Fine", message);
                                    }
                                    else
                                    {
                                        notifRepo.AddNotification(userId, "Return Success", message);
                                    }
                                }
                                else
                                {
                                    notifRepo.AddNotification(userId, "Return Success", message);
                                }
                            }
                        }
                        catch { }
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
