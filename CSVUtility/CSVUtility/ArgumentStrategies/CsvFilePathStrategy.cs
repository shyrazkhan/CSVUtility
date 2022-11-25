using System;
using System.Configuration;
using System.IO;
using CSVUtility.Configuration;
using CSVUtility.Core;

namespace CSVUtility.ArgumentStrategies
{
    public class CsvFilePathStrategy : IArgumentStrategy
    {
        public bool CanHandle(string argumentName)
        {
            return (argumentName.Equals("csvFilePath", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Process(IUtilityEngine instance, string argumentValue)
        {
            if (Path.HasExtension(argumentValue) == false)
                throw new Exception($"The {argumentValue} doesn't have extension.");

            var extension = Path.GetExtension(argumentValue);
            if(extension == null || extension.Equals(".csv", StringComparison.InvariantCultureIgnoreCase) == false)
                throw new Exception($"The {argumentValue} doesn't have 'CSV' extension.");

            instance.CsvFilePath = argumentValue;
        }

        public string GetHelpText()
        {
            return "/csvFilePath      Set the csv file location path.  ";

        }
    }
}
