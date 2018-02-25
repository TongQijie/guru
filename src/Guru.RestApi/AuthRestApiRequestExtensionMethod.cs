using System.Collections.Generic;
using System.Linq;
using Guru.ExtensionMethod;

namespace Guru.RestApi
{
    public static class AuthRestApiRequestExtensionMethod
    {
        public const string UserIdExtensionKey = "uid";

        public static string GetAuth(this IAuthRestApiRequest authRestApiRequest)
        {
            if (authRestApiRequest == null ||
                authRestApiRequest.Head == null ||
                !authRestApiRequest.Head.Auth.HasValue())
            {
                return null;
            }

            return authRestApiRequest.Head.Auth;
        }

        public static void SetUid(this IAuthRestApiRequest authRestApiRequest, string uid)
        {
            if (authRestApiRequest == null)
            {
                return;
            }

            if (authRestApiRequest.Head == null)
            {
                authRestApiRequest.Head = new AuthRestApiRequestHead();
            }

            if (authRestApiRequest.Head.Extensions == null)
            {
                authRestApiRequest.Head.Extensions = new Dictionary<string, string>();
            }

            authRestApiRequest.Head.Extensions[UserIdExtensionKey] = uid;
        }

        public static string GetUid(this IAuthRestApiRequest authRestApiRequest)
        {
            if (authRestApiRequest == null || 
                authRestApiRequest.Head == null || 
                authRestApiRequest.Head.Extensions == null || 
                !authRestApiRequest.Head.Extensions.ContainsKey(UserIdExtensionKey))
            {
                return null;
            }

            return authRestApiRequest.Head.Extensions[UserIdExtensionKey];
        }

        public static string GetExtensionValue(this IAuthRestApiRequest authRestApiRequest, string key)
        {
            if (authRestApiRequest == null ||
                authRestApiRequest.Head == null ||
                authRestApiRequest.Head.Extensions == null)
            {
                return null;
            }

            var extensionKey = authRestApiRequest.Head.Extensions.Keys.ToArray().FirstOrDefault(x => x.EqualsIgnoreCase(key));
            if (extensionKey == null)
            {
                return null;
            }

            return authRestApiRequest.Head.Extensions[extensionKey];
        }
    }
}