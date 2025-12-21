using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Repositories
{
    internal class CategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            var list = new List<Category>();

            // Always add "All" manually because it doesn't exist in the database
            list.Add(new Category { CategoryID = 0, Name = "All" });

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT CategoryID, Name FROM Categories";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Category
                            {
                                CategoryID = (int)reader["CategoryID"],
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
        public static string GetCategoryNameById(int id)
        {
            if (id == 0) return "All";
            string categoryName = "";
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string query = "SELECT Name FROM Categories WHERE CategoryID = @id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        categoryName = result.ToString();
                    }
                }
            }
            return categoryName;
        }
    }
}
