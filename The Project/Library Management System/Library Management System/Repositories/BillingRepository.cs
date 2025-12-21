using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Library_Management_System.Repositories
{
    public class BillingRepository
    {
        public DataTable GetAllPayments()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT p.PaymentID, u.FullName AS [User Name], 
                         b.Title AS [Book Title], p.BorrowingPrice, p.Status
                         FROM Payments p
                         JOIN Users u ON p.UserID = u.UserID
                         JOIN Books b ON p.BookID = b.BookID";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        // Update Payment Status 
        public bool UpdatePaymentStatus(int paymentID, string newStatus)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "UPDATE Payments SET Status = @Status WHERE PaymentID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@ID", paymentID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Get all bills for a specific user
        public List<BillItem> GetUserBills(int userId)
        {
            var list = new List<BillItem>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                
                string query = @"
                    SELECT 
                        p.PaymentID, 
                        p.BorrowingPrice, 
                        p.PaymentDate, 
                        p.[Status],
                        b.Title AS BookTitle
                    FROM Payments p
                    INNER JOIN Books b ON p.BookID = b.BookID
                    WHERE p.UserID = @UserID
                    ORDER BY p.PaymentID DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new BillItem
                            {
                                InvoiceID = "#" + reader["PaymentID"].ToString(),

                                // Now reading the actual Title from the joined table
                                BookTitle = reader["BookTitle"].ToString(),

                                Date = Convert.ToDateTime(reader["PaymentDate"]).ToString("dd MMM yyyy"),
                                Price = (decimal)reader["BorrowingPrice"],
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }

    public class BillItem
    {
        public string InvoiceID { get; set; }
        public string BookTitle { get; set; }
        public string Date { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}