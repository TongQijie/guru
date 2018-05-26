using System.Collections.Generic;

namespace Guru.Restful
{
    public static class RequestHeadExtensions
    {
        private const string UserIdExtensionKey = "uid";

        public static void SetUserId(this RequestHead head, string uid)
        {
            if (head == null)
            {
                return;
            }

            if (head.Extensions == null)
            {
                head.Extensions = new Dictionary<string, string>();
            }

            head.Extensions[UserIdExtensionKey] = uid;
        }

        public static string GetUserId(this RequestHead head)
        {
            if (head == null || 
                head.Extensions == null || 
                !head.Extensions.ContainsKey(UserIdExtensionKey))
            {
                return null;
            }

            return head.Extensions[UserIdExtensionKey];
        }
    }
}