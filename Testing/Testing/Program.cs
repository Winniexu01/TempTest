using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static int Main(string[] args)
        {
            System.Console.WriteLine(Environment.GetEnvironmentVariable("SYSTEM_DEFINITIONNAME"));
            return 0;
        }
    }
}
