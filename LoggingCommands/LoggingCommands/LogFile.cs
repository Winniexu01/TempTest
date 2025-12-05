namespace LoggingCommands
{
    public class LogFile
    {
        public static void WriteMessage(string msg, LogType logType = LogType.Information)
        {
            Console.ForegroundColor = logType switch
            {
                LogType.Error => ConsoleColor.Red,
                LogType.Warning => ConsoleColor.Yellow,
                _ => ConsoleColor.Blue
            };
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public enum LogType
        {
            Information,
            Warning,
            Error
        }
    }
}