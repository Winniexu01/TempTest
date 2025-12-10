using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetFramework472
{
    public class LogFile
    {
        public static void LogMessage(object msg, LogType logType = LogType.Information)
        {
            var logissue = "";
            switch (logType)
            {
                case LogType.Error:
                    logissue = "##vso[task.logissue type=error]";
                    throw new Exception(msg.ToString());
                case LogType.Warning:
                    logissue = "##vso[task.logissue type=warning]";
                    break;
                default:
                    logissue = "";
                    break;
            }
            Console.WriteLine($"{logissue}{msg}");
        }

        public enum LogType
        {
            Information,
            Warning,
            Error
        }
    }
}
