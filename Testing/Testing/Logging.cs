using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class Logger
    {
        public static void LogMessage(object msg, LogType logType = LogType.Information)
        {
            var logissue = string.Empty;
            switch (logType)
            {
                case LogType.Error:
                    ErrorManager.HasError = true;
                    logissue = "##vso[task.logissue type=error]";
                    break;
                case LogType.Warning:
                    logissue = "##vso[task.logissue type=warning]";
                    break;
            }
            Console.WriteLine($"{logissue}{msg}");
        }
    }

    public static class ErrorManager
    {
        public static bool HasError { get; set; } = false;
    }

    public enum LogType
    {
        Information,
        Warning,
        Error
    }
}
