using System;
using CSVUtility.Core;

namespace CSVUtility.ArgumentStrategies
{
    class SamplePropertyStrategy 
    {
        public bool CanHandle(string argumentName)
        {
            return argumentName.Equals("SampleProperty", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IUtilityEngine instance, string argumentValue)
        {
            instance.SampleProperty = true;
        }

        public string GetHelpText()
        {
            return "    /sampleProp     Include this help text so that users are provide some help text to let them know how to use this argument.     (Default False)";
        }
    }
}
