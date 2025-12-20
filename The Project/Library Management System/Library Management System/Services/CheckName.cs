using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Library_Management_System.Services
{
    internal class CheckName
    {
        public static bool validName(string name)
        {
            return Regex.IsMatch(name, @"[^a-zA-Z]");
        }
    }

}
