using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMMon.ConsoleTool
{
    class Log
    {
        static readonly ConsoleColor OriginalConsoleForegroundColor = Console.ForegroundColor;

        public static void WriteLog(LogLevel level, string messageTemplate, params object[] args)
        {
            string message = string.Format(messageTemplate, args);
            DateTime now = DateTime.Now;

            if (level == LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }            
            else if (level == LogLevel.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (level == LogLevel.High)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("[{0}] {1}", now.ToLogDateString(), message);

            // reset console foreground color
            Console.ForegroundColor = OriginalConsoleForegroundColor;
        }
    }

    enum LogLevel
    {
        Verbose,
        High,
        OK,
        Error,
    }
}
