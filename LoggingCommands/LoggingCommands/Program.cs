namespace LoggingCommands
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogFile.WriteMessage("Information");
            LogFile.WriteMessage("Warning", LogFile.LogType.Warning);
            LogFile.WriteMessage("Error", LogFile.LogType.Error);

        }
    }
}
