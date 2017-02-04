using System;
namespace Guru.Monitor.Errors
{
    public class InvalidFilePathException : Exception
    {
        public InvalidFilePathException(string filePath)
            : base(string.Format("'{0}' is invalid file path.", filePath))
        {
        }
    }
}
