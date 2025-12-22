// using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            int exitCode = int.Parse(args[0]);
            // var message = VSEngMailer.CreateMailMessage("v-wexu@microsoft.com", "Test", "This is a test email.");
            // VSEngMailer.SendMail(message);
            try
            {
                if (exitCode == 0)
                {
                    Console.WriteLine("##vso[task.complete result=Succeeded;]WARNING! The process ran into an error at some point.");
                }
                else if (exitCode == -1)
                {
                    Console.WriteLine("##vso[task.complete result=partiallySucceeded;]WARNING! The process ran into an error at some point.");
                }
                else
                {
                    Console.WriteLine("##vso[task.complete result=Failed;]WARNING! The process ran into an error at some point.");
                }
                // return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"##vso[task.logissue type=error]{ex.Message}");
                // return 1;
            }
        }
    }
}
