using System;
using System.Linq;
using System.Text.RegularExpressions;
using CSVUtility.Core;
using CSVUtility.Core.Helpers;
using CSVUtility.Exceptions;

namespace CSVUtility.ArgumentStrategies
{
    internal class HelpStrategy : IArgumentStrategy
    {
        private const string TOP_BAR = "╔════════════════════════════════════════════════════════════════════════════╗";
        private const string BOTTOM_BAR = "╚════════════════════════════════════════════════════════════════════════════╝";
        private const string SIDE_BAR = "║";

        public bool CanHandle(string argumentName)
        {
            return (argumentName.Equals("Help", StringComparison.InvariantCultureIgnoreCase) || argumentName.Equals("?"));
        }

        public void Process(IUtilityEngine instance, string argumentValue)
        {
            var otherStrategies = StrategyLoader.LoadStrategies<IArgumentStrategy>(System.Reflection.Assembly.GetExecutingAssembly().FullName)
                                                .Where(m => m.GetType() != GetType());
            var helpText = string.Join("\n", otherStrategies.Select(s => FormatStrategyHelpText(s.GetHelpText())));

            Console.WriteLine(GetHelpText(), helpText);

            throw new HelpDisplayedException("Help Displayed");
        }

        public string GetHelpText()
        {
            const string headerPrefix = "Utility Prefix Header Information";
            const string headerSuffix = "Utility Suffix Header Information";
            const string footerText = "\nSample:\n{0} /argument:value";

            const string helpText = "Valid Arguments:\n    /help or /? This option will display this help text on the screen.\n";

            var header = TOP_BAR + "\n"
                + FormatHeaderText(headerPrefix)
                + "\n" + FormatHeaderText(headerSuffix)
                + "\n" + BOTTOM_BAR + "\n";

            return (header + helpText + "{0}" + string.Format(footerText, AppDomain.CurrentDomain.FriendlyName));
        }


        private string FormatHeaderText(string headerText)
        {
            var headerSplits = Regex.Replace(headerText, @"(.{1," + 74 + @"})(?:\s|$)", "$1\n").Split('\n');
            for (var i = 0; i < headerSplits.Length; i++)
                headerSplits[i] = SIDE_BAR + " " + headerSplits[i].PadRight(75) + SIDE_BAR;

            return string.Join("\n", headerSplits);
        }

        private string FormatStrategyHelpText(string helpText, int length = 58)
        {
            var firstLine = Regex.Replace(helpText, @"(.{1," + 78 + @"})(?:\s|$)", "$1\n").Split('\n')[0];
            var remainingHelpText = helpText.Remove(0, firstLine.Length);
            var remainingHelp = Regex.Replace(remainingHelpText, @"(.{1," + length + @"})(?:\s|$)", "$1\n").Split('\n');
            for (var i = 0; i < remainingHelp.Length; i++)
                remainingHelp[i] = new string(' ', 20) + remainingHelp[i].TrimStart();

            return string.Join("\n", new[] { firstLine }.Union(remainingHelp));
        }
    }
}
