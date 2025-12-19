using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Library_Management_System.Data; 
namespace Library_Management_System.Repositories
{
    public class FavouriteRepository
    {

        public void AddToFavourites(int userId, int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Favourites (UserID, BookID) VALUES (@UserID, @BookID)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    try { cmd.ExecuteNonQuery(); }
                    catch { /* Ignore if already exists */ }
                }
            }
        }

        public void RemoveFromFavourites(int userId, int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Favourites WHERE UserID = @UserID AND BookID = @BookID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsFavourite(int userId, int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Favourites WHERE UserID = @UserID AND BookID = @BookID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public HashSet<int> GetUserFavouriteBookIds(int userId)
        {
            HashSet<int> favIds = new HashSet<int>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT BookID FROM Favourites WHERE UserID = @UserID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            favIds.Add((int)reader["BookID"]);
                        }
                    }
                }
            }
            return favIds;
        }
    }
}