// using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");
            // var message = VSEngMailer.CreateMailMessage("v-wexu@microsoft.com", "Test", "This is a test email.");
            // VSEngMailer.SendMail(message);
            Console.WriteLine("##vso[task.complete result=partiallySucceeded;]WARNING! The process ran into an error at some point.");
        }
    }
}
