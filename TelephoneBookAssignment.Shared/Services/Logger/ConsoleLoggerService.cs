using System;

namespace TelephoneBookAssignment.Shared.Services.Logger
{
    public class ConsoleLoggerService : ILoggerService
    {
        public void Write(string message)
        {
            Console.WriteLine("[ConsoleLogger] - " + message);
        }
    }
}