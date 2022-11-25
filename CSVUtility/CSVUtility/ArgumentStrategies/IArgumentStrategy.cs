using CSVUtility.Core;

namespace CSVUtility.ArgumentStrategies
{
    interface IArgumentStrategy
    {
        bool CanHandle(string argumentName);
        void Process(IUtilityEngine instance, string argumentValue);
        string GetHelpText();
    }
}
