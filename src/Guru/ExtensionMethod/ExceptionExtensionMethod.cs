using System;
using System.Text;

namespace Guru.ExtensionMethod
{
    public static class ExceptionExtensionMethod
    {
        public static string GetInfo(this Exception source)
        {
            var stringBuilder = new StringBuilder();

            var e = source;
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