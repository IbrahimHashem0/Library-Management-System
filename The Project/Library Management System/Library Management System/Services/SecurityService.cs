using System;
using System.Security.Cryptography; // Needed for Hashing
using System.Text;

namespace LibrarySystem.Services
{
    public static class SecurityService
    {
        // This function takes a plain password (e.g., "123456") and turns it into a Hash
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the string to bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert bytes back to a string (Hex format)
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // This function checks if a login attempt matches the stored hash
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;
        }
    }
}