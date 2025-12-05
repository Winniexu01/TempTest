using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetFramework472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogFile.LogMessage("Information");
            LogFile.LogMessage("Warning", LogFile.LogType.Warning);
            LogFile.LogMessage("Error", LogFile.LogType.Error);
        }
    }
}
