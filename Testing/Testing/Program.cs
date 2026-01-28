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

                var prWorkItems = new List<string>();
                var scheduledWorkItems = new List<string>();
                var NoAutoIntegrations = new List<string>();

                scheduledWorkItems.Add(Environment.GetEnvironmentVariable("ScheduledWorkItems"));
                prWorkItems.Add(Environment.GetEnvironmentVariable("PRWorkItems"));
                NoAutoIntegrations.Add(Environment.GetEnvironmentVariable("NoAutoWorkItems"));

                foreach(var item in scheduledWorkItems)
                {
                    Console.WriteLine($"Scheduled Work Item: {item}");
                }
                foreach (var item in prWorkItems)
                {
                    Console.WriteLine($"PR Work Item: {item}");
                }
                foreach (var item in NoAutoIntegrations)
                {
                    Console.WriteLine($"No Auto Integration Work Item: {item}");
                }
            }
            else
            {
                Console.WriteLine("DEBUG_MODE is not enabled.");
            }
            return 0;
        }
    }
}
