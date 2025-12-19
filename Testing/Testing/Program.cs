using VSEng.Mailer;
namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var message = VSEngMailer.CreateMailMessage("v-wexu@microsoft.com", "Test", "This is a test email.");
            VSEngMailer.SendMail(message);
        }
    }
}
