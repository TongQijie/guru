using System;

namespace Guru.ExtensionMethod
{
    public static class LongExtensionMethod
    {
        private static DateTime ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();

        public static DateTime DateTime(this long timestamp)
        {
            return ZeroDateTime.AddMilliseconds(timestamp);
        }
    }
}