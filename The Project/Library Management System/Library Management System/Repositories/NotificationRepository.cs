using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Library_Management_System.Data;
using Library_Management_System.Models;

namespace Library_Management_System.Repositories
{
    public class NotificationRepository
    {
        public void AddNotification(int userId, string title, string message)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Notifications (UserID, Title, Message, CreatedAt, IsRead) VALUES (@UserID, @Title, @Message, GETDATE(), 1)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Notification> GetUserNotifications(int userId)
        {
            var list = new List<Notification>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Notifications WHERE UserID = @UserID ORDER BY CreatedAt DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Notification
                            {
                                NotificationID = (int)reader["NotificationID"],
                                UserID = (int)reader["UserID"],
                                Title = reader["Title"].ToString(),
                                Message = reader["Message"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                IsRead = (bool)reader["IsRead"]
                            });
                        }
                    }
                }
            }
            return list;
        }
        public void MarkAsRead(int notificationId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Notifications SET IsRead = 0 WHERE NotificationID = @ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", notificationId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void CheckAndGenerateOverdueNotifications(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. Find overdue books that haven't been returned yet
                string findOverdueQuery = @"
                    SELECT b.Title
                    FROM Borrowings br
                    JOIN Books b ON br.BookID = b.BookID
                    WHERE br.UserID = @UserID
                      AND br.ReturnDate IS NULL
                      AND br.DueDate < GETDATE()";

                var overdueBooks = new List<string>();
                using (var cmd = new SqlCommand(findOverdueQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            overdueBooks.Add(reader["Title"].ToString());
                        }
                    }
                }

                // 2. For each overdue book, check if we already notified the user TODAY to avoid spamming
                foreach (var title in overdueBooks)
                {
                    string message = $"Urgent: The book '{title}' is overdue. Please return it.";

                    // Simple check: Don't add if the exact same message exists since the last 24 hours
                    string duplicateCheck = @"
                        SELECT COUNT(*) FROM Notifications 
                        WHERE UserID = @UserID 
                          AND Message = @Message 
                          AND CreatedAt > DATEADD(day, -1, GETDATE())";

                    bool alreadyNotified = false;
                    using (var cmdCheck = new SqlCommand(duplicateCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@UserID", userId);
                        cmdCheck.Parameters.AddWithValue("@Message", message);
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0) alreadyNotified = true;
                    }

                    if (!alreadyNotified)
                    {
                        if (!alreadyNotified)
                        {
                            AddNotification(userId, "Overdue Alert", message);
                        }
                    }
                }
            }
        }
    }
}
