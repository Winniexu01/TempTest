using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static int Main(string[] args)
        {
            Logger.LogMessage("Starting the process...");
            Logger.LogMessage("");// blank line for readability
            Logger.LogMessage("This is a test log message.", LogType.Warning);
            Logger.LogMessage("");// blank line for readability
            Logger.LogMessage("This is a test log message.", LogType.Error);
            if (ErrorManager.HasError)
                Console.WriteLine("##vso[task.complete result=SucceededWithIssues;]WARNING! The process ran into an error at some point.");
            Logger.LogMessage("");// blank line for readability
            Logger.LogMessage("Process finished.");
            return 0;
        }
    }
}
