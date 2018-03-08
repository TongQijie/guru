using System;
using System.Text;

namespace Guru.Logging
{
    internal class ExceptionWrapper
    {
        public ExceptionWrapper(Exception exception)
        {
            _Exception = exception;
        }

        private Exception _Exception = null;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            var e = _Exception;
            while (e != null)
            {
                stringBuilder.AppendLine($"{e.GetType().Name}: {e.Message}");
                stringBuilder.AppendLine($"{e.StackTrace}");
                e = e.InnerException;
            }

            return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}
