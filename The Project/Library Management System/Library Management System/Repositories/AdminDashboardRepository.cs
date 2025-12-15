using Library_Management_System.Data;
using Library_Management_System.Models;
using LibrarySystem.Services;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Library_Management_System.Repositories
{
    internal class AdminDashboardRepository
    {
        public int GetTotalBooks()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                return (int)new SqlCommand("SELECT COUNT(*) FROM Books" , conn).ExecuteScalar();
            }
        }

        public int GetTotalUsers()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                return (int)new SqlCommand("SELECT COUNT(*) FROM Users" , conn).ExecuteScalar();
            }
        }


        public int GetTotalBorrowed()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                return (int)new SqlCommand(
                    "SELECT COUNT(*) FROM Borrowings WHERE ReturnDate IS NULL" , conn
                ).ExecuteScalar();
            }
        }
    }
}
