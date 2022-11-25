using System;

namespace CSVUtility.Exceptions
{
    class HelpDisplayedException : Exception
    {
        public HelpDisplayedException(string message) : base(message) { }
    }
}
