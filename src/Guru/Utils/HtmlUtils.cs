using System.Net;

namespace Guru.Utils
{
    public static class HtmlUtils
    {
        public static string Decode(string rawString)
        {
            return WebUtility.HtmlDecode(rawString);
        }

        public static string Encode(string rawString)
        {
            return WebUtility.HtmlEncode(rawString);
        }
    }
}
