using System;
using log4net;

namespace CSVUtility.Core.Helpers
{
    public interface IOutputHandler
    {
        void Log(Exception e);
        void Log(string message, MessageSeverity severity = MessageSeverity.None);
        void WriteToConsole(string message, MessageSeverity severity = MessageSeverity.None);
        void WriteRemainingItems(int initialCount, int remainingCount);
        void EmptyLine();
    }

    public class OutputHandler : IOutputHandler
    {
        private readonly ILog logger = LogManager.GetLogger(nameof(OutputHandler));

        public void Log(Exception e)
        {
            logger.Error(e);
        }

        public void Log(string message, MessageSeverity severity = MessageSeverity.None)
        {
            WriteToConsole(message, severity);

            switch (severity)
            {
                case MessageSeverity.None:
                    logger.Debug(message);
                    break;
                case MessageSeverity.Info:
                    logger.Info(message);
                    break;
                case MessageSeverity.Warning:
                    logger.Warn(message);
                    break;
                case MessageSeverity.Error:
                    logger.Error(message);
                    break;
            }
        }

        public void WriteToConsole(string message, MessageSeverity severity = MessageSeverity.None)
        {
            var originalColor = Console.ForegroundColor;
            var newColor = originalColor;

            switch (severity)
            {
                case MessageSeverity.Info:
                    newColor = ConsoleColor.Green;
                    break;
                case MessageSeverity.Warning:
                    newColor = ConsoleColor.Yellow;
                    break;
                case MessageSeverity.Error:
                    newColor = ConsoleColor.Red;
                    break;
            }

            Console.ForegroundColor = newColor;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        public void WriteRemainingItems(int initialCount, int remainingCount)
        {
            var cursorOffset = initialCount.ToString().Length - remainingCount.ToString().Length + 1;
            Console.Write($"\rRemaining: {remainingCount:N0}{new string(' ', cursorOffset)}");
        }

        public void EmptyLine()
        {
            Console.WriteLine("");
        }
    }


    public enum MessageSeverity
    {
        None,
        Info,
        Warning,
        Error
    }
}
