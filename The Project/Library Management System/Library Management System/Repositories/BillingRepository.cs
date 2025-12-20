using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Library_Management_System.Data;
using Library_Management_System.Models;

namespace Library_Management_System.Repositories
{
    public class BillingRepository
    {
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