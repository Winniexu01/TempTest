namespace LoggingCommands
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##vso[task.setvariable variable=CWResult]HelloWorld!");
        }
    }
}
