using System.Collections.Generic;

namespace SourceMediator.Example
{
    public class StaticLogger
    {
        public static readonly List<string> LogMessages = new();

        public static void Log(string message)
        {
            LogMessages.Add(message);
        }
    }
}