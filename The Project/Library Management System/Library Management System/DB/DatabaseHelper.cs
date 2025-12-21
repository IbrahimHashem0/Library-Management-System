using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Data
{
    public class DatabaseHelper
    {

        private static string connectionString = @"Server=.;Database=Library Management System;Trusted_Connection=True;";

        
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
