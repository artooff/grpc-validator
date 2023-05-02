using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegularExpressionsTest
{
    class Programm
    {
        static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(ValidatePhone("+79771705781"));
        }

        public static  bool ValidatePhone(string phoneNumber)
        {
            string pattern = @"^((8|\+7)[\-]?)?(\(?\d{3}\)?[\-]?)?[\d\-]{7,10}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
    
}

