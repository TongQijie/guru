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
                stringBuilder.AppendLine(string.Format("{0}: {1}", e.GetType().Name, e.Message));
                e = e.InnerException;
            }

            if (_Exception != null)
            {
                stringBuilder.AppendLine(_Exception.StackTrace);
            }

            return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}
