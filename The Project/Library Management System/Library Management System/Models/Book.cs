using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public int? Year { get; set; } // Nullable because SQL allows NULL
        public int CategoryID { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public decimal BorrowPrice { get; set; }

    }
}
