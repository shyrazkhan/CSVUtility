using System;

namespace CSVUtility.Core.Helpers
{
    public static class ConsoleExtensions
    {
        public static void ConsoleWriteLineWithForegroundColor(ConsoleColor color, string messageFormat, params object[] args)
        {
            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                if (args != null && args.Length > 0)
                    Console.WriteLine(messageFormat, args);
                else
                    Console.WriteLine(messageFormat);
            }
            finally { Console.ForegroundColor = originalColor; }
        }

        public static void ConsoleWriteValidationMessage(string validationMessage)
        {
            if (string.IsNullOrWhiteSpace(validationMessage))
                return;

            ConsoleWriteLineWithForegroundColor(ConsoleColor.DarkYellow, validationMessage);
        }
    }
}
