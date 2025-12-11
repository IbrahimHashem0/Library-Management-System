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
        // 1. FOR LOCAL TESTING (Use this if the DB is on YOUR computer)
        private static string connectionString = @"Server=DESKTOP-PPKRP4G\SQLEXPRESS;Database=LibraryManagementSystem;Trusted_Connection=True;";

        // 2. FOR ZEROTIER (Use this if connecting to a friend's DB)
        // private static string connectionString = @"Server=10.xxx.xxx.xxx,1433;Database=Library Management System;User Id=ShareUser;Password=YOUR_PASSWORD;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
