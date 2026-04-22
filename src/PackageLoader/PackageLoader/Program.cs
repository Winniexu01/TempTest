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

            var debug = Environment.GetEnvironmentVariable("DEBUG");
            Console.WriteLine($"DEBUG: {debug}");
            Console.WriteLine(debug.GetType());
            var isDebug = bool.TryParse(debug, out var value) ? value : false;
            Console.WriteLine($"Is Debug Mode: {isDebug}");
            Console.WriteLine(isDebug.GetType());
        }
    }
}
