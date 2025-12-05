namespace LoggingCommands
{
    public class LogFile
    {
        public static void WriteMessage(string msg, LogType logType = LogType.Information)
        {
            var logissue = logType switch
            {
                LogType.Error => "##vso[task.logissue type=error]",
                LogType.Warning => "##vso[task.logissue type=warning]",
                _ => ""
            };
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