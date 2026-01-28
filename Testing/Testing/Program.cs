using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var isDebugMode = Environment.GetEnvironmentVariable("DEBUG_MODE");
            if (!string.IsNullOrEmpty(isDebugMode) && isDebugMode.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("DEBUG_MODE is enabled...");

                var prWorkItems = Environment.GetEnvironmentVariable("PRWorkItems") ?? "";
                var scheduledWorkItems = Environment.GetEnvironmentVariable("ScheduledWorkItems") ?? "";
                var noAutoWorkItems = Environment.GetEnvironmentVariable("NoAutoWorkItems") ?? "";
                Console.WriteLine($"PR Work Items: {prWorkItems}");
                Console.WriteLine($"Scheduled Work Items: {scheduledWorkItems}");
                Console.WriteLine($"No Auto Work Items: {noAutoWorkItems}");
            }
            else
            {
                Console.WriteLine("DEBUG_MODE is not enabled.");
            }
            return 0;
        }
    }
}
