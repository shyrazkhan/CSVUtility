using System;

namespace CSVUtility.Exceptions
{
    class AdministratorRequiredException : Exception
    {
        public AdministratorRequiredException(string message) : base(message) { }
    }
}
