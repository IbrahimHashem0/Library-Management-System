using Library_Management_System.Data;
using Library_Management_System.Models;
using LibrarySystem.Services;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Library_Management_System.Repositories
{
    public class UserRepository
    {

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

        public bool Register(string fullName, string email, string plainPassword, string role)
        {
            string hashedPassword = SecurityService.HashPassword(plainPassword);
            string query = "INSERT INTO Users (FullName, Email, Password, Role) VALUES (@Name, @Email, @Pass, @Role)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", fullName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Pass", hashedPassword);
                cmd.Parameters.AddWithValue("@Role", role);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected == 1;
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    // Error 2627: Unique constraint violation (Email already exists)
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
    
}