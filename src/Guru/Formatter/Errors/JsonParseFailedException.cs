using System;

namespace Guru.Formatter.Errors
{
    public class JsonParseFailedException : Exception
    {
        public JsonParseFailedException(int position, string description)
            : base(string.Format("error position: {0}{1}description: {2}", position, Environment.NewLine, description))
        {
        }
    }
}
