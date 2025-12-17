using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Repositories
{
    internal class QuoteRepository
    {

        public List<Quote> GetAllQuotes()
        {
            List<Quote> quotes = new List<Quote>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT QuoteText, Author FROM Quotes";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Quote q = new Quote();
                                q.Text = reader["QuoteText"].ToString();
                                q.Author = reader["Author"].ToString();
                                quotes.Add(q);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                     MessageBox.Show("Error loading quotes: " + ex.Message);
                }
            }

            return quotes;
        }
    }
}
