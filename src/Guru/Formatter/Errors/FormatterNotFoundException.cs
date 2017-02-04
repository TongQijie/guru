using System;

namespace Guru.Formatter.Errors
{
    public class FormatterNotFoundException : Exception
    {
        public FormatterNotFoundException()
            : base(string.Format("object formatter cannot be found."))
        {
        }

        public FormatterNotFoundException(string formatterString)
            : base(string.Format("object formatter '{0}' cannot be found.", formatterString))
        {
        }
    }
}