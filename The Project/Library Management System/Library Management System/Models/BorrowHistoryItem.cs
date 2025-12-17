using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    internal class BorrowHistoryItem
    {
        public int BorrowingID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string BorrowDate { get; set; } // Storing as string for easy formatting
        public string DueDate { get; set; }
        public string Status { get; set; }
    }
}
