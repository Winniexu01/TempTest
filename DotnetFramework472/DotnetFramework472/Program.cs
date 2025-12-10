using System;

namespace DotnetFramework472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SampleMethod2();
            }
            catch (Exception ex)
            {
                LogFile.LogMessage("Failed", LogFile.LogType.Error);
            }
        }

        private static void SampleMethod()
        {
            System.Console.WriteLine("Test");
            throw new InvalidOperationException("Sample exception");
        }

        private static void SampleMethod2()
        {
            SampleMethod();
        }
    }
}