using System;
using System.Linq;
using CSVUtility.ArgumentStrategies;
using CSVUtility.Core;
using CSVUtility.Core.Helpers;

namespace CSVUtility.Helpers
{
    public static class ArgumentHelper
    {
        public static void ParseArguments(IUtilityEngine controller, string[] arguments)
        {
            var filter = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            var strategies = StrategyLoader.LoadStrategies<IArgumentStrategy>(filter);
            foreach (var argument in arguments)
            {
                var argSplit = argument.Split(':');
                var argumentName = argSplit[0].TrimStart('/');
                var argumentValue = (argSplit.Length > 1) ? argument.Substring(argumentName.Length + 2).Trim() : string.Empty;

                var argStrategy = strategies.FirstOrDefault(strategy => strategy.CanHandle(argumentName));
                if (argStrategy == null)
                    throw new ArgumentException("Invalid argument passed to the program.  Please use /help to get the allowable options.", argumentName);

                argStrategy.Process(controller, argumentValue);
            }
        }
    }
}
