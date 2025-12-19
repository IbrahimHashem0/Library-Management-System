using Library_Management_System.Data;
using Library_Management_System.Models;
using LibrarySystem.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Library_Management_System.Repositories
{
    public class UserRepository
    {
        public bool IsEmailExists(string email)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM Users WHERE LOWER(Email) = LOWER(@email)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email.Trim());
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return false;
            }
        }
        private SqlConnection GetConnection()
        {
            
            string connectionString = @"Server=.;Database=Library Management System;Trusted_Connection=True;";
            return new SqlConnection(connectionString);
        }


        public bool AddUser(User user)
        {
            try
            {

                using (var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO Users (FullName, Email, Password, Role, Status) VALUES (@n, @e, @p, @r, @s)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", user.FullName);
                        cmd.Parameters.AddWithValue("@e", user.Email);
                        cmd.Parameters.AddWithValue("@p", user.Password);
                        cmd.Parameters.AddWithValue("@r", user.Role);
                        cmd.Parameters.AddWithValue("@s", user.Status);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
                return false;
            }
        }

        public int GetAdminCount()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Role = 'admin'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public List<User> GetAllUsers(string searchTerm = "", string roleFilter = "All Roles")
        {
            List<User> users = new List<User>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT UserID, FullName, Email, Role, Status FROM Users WHERE 1=1";
                if (!string.IsNullOrEmpty(searchTerm))
                    query += " AND (FullName LIKE @S OR Email LIKE @S)";
                if (roleFilter != "All Roles")
                    query += " AND Role = @R";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@S", "%" + searchTerm + "%");
                cmd.Parameters.AddWithValue("@R", roleFilter);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = (int)reader["UserID"],
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString(),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return users;
        }

        // Delete user logic
        public bool DeleteUser(int id)
        {
            string query = "DELETE FROM Users WHERE UserID = @ID";
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public User Login(string email, string plainPassword)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // 1. Select the stored hash based on the email
                string query = "SELECT * FROM Users WHERE Email = @Email";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get the hash from the database
                            string storedHash = reader["Password"].ToString();

                            // 2. Use your helper to verify
                            if (SecurityService.VerifyPassword(plainPassword, storedHash))
                            {
                                // Login Success! Return the user object.
                                return new User
                                {
                                    UserID = (int)reader["UserID"],
                                    FullName = reader["FullName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    CreatedAt = reader["CreatedAt"] != DBNull.Value
                                                ? (DateTime)reader["CreatedAt"]
                                                : DateTime.MinValue

                                };
                            }
                        }
                    }
                }
            }
            return null; // Login Failed (Wrong password or email not found)
        }

        
        public bool ToggleUserStatus(int userId, string currentStatus)
        {
            // Determine the new status
            string newStatus = (currentStatus == "Active") ? "Suspend" : "Active";

            // SQL Query to update status
            string query = "UPDATE Users SET Status = @Status WHERE UserID = @ID";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@ID", userId);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

    }
    
}