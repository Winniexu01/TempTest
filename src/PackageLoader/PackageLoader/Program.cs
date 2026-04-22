using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("KEYWORDS");
            Console.WriteLine($"KEYWORDS: {env}");
        }
    }
}
