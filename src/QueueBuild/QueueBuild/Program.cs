using AzureDevOpsAPI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        GlobalSettings.OAuthAccessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN");
        GlobalSettings.CredentialType = CredentialType.VssOAuthCredential;
        QueueBuild(account: "v-wexu0720", project: "Test", definitionId: 13, customParameters: null, queueId: -1);
    }
    public static void QueueBuild(string account, string project, int definitionId, Dictionary<string, string> customParameters, int queueId)
    {
        try
        {
            BuildAPI.QueueBuild(account, project, definitionId, parameters: customParameters, queueId: queueId);
        }
        catch (SimpleHttpResponseException ex)
        {
            if (ex.Content == null || ex.Content.IndexOf("SettingVariablesAtQueueTime", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw;
            }
        }
    }
}