using EventGridEmulator.Contracts;
using System;

namespace EventGridEmulator
{
    public class ConsoleLogger : ILogger
    {
        public void LogError(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine(message);

            Console.ForegroundColor = prevColor;
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogHeading(string heading)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine(heading);
            Console.WriteLine(new string('-', heading.Length));

            Console.ForegroundColor = prevColor;
        }
    }
}
