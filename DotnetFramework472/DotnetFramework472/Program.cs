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
                LogFile.LogMessage(ex.Message, LogFile.LogType.Error);
            }
        }

        private static void SampleMethod()
        {
            try
            {
                throw new InvalidOperationException("Sample exception");
            }
            catch (Exception ex)
            {
                LogFile.LogMessage(ex.Message, LogFile.LogType.Error);
            }
        }

        private static void SampleMethod2()
        {
            try
            {
                SampleMethod();
            }
            catch (Exception ex)
            {
                LogFile.LogMessage(ex.Message, LogFile.LogType.Error);
            }
        }
    }
}