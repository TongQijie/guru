using System;

namespace Guru.Testing
{
    public class AssertFailureException : Exception
    {
        public AssertFailureException() { }

        public AssertFailureException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
