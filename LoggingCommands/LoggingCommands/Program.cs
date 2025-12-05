namespace LoggingCommands
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogFile logFile = new LogFile();
            logFile.WriteMessage("Information");
            logFile.WriteMessage("Warning", LogFile.LogType.Warning);
            logFile.WriteMessage("Error", logFile.LogType.Error);

        }
    }
}
