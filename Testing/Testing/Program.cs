using VSEng.Mailer;
using System;
namespace Testing
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string collectionUri = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI");
            string teamProject = Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECT");
            string buildId = Environment.GetEnvironmentVariable("BUILD_BUILDID"); // Use BUILD_BUILDID for build pipelines

            // Construct the full URL
            if (!string.IsNullOrEmpty(collectionUri) && !string.IsNullOrEmpty(teamProject) && !string.IsNullOrEmpty(buildId))
            {
                // Example format for a build pipeline URL
                string pipelineUrl = $"{collectionUri}{teamProject}/_build/results?buildId={buildId}";
                Logger.LogMessage($"Current Pipeline URL: {pipelineUrl}");
            }
            else
            {
                Logger.LogMessage("Could not find all required Azure Pipelines environment variables.", LogType.Error);
            }

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
