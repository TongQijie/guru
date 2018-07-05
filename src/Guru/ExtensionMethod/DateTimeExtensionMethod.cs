using System;

namespace Guru.ExtensionMethod
{
    public static class DateTimeExtensionMethod
    {
        private static DateTime ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();

        public static long Timestamp(this DateTime dateTime)
        {
            return (long)((dateTime - ZeroDateTime).TotalMilliseconds);
        }
    }
}