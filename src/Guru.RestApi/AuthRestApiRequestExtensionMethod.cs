using System.Collections.Generic;
using Guru.ExtensionMethod;

namespace Guru.RestApi
{
    public static class AuthRestApiRequestExtensionMethod
    {
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

            authRestApiRequest.Head.Extensions["uid"] = uid;
        }

        public static string GetUid(this IAuthRestApiRequest authRestApiRequest)
        {
            if (authRestApiRequest == null || 
                authRestApiRequest.Head == null || 
                authRestApiRequest.Head.Extensions == null || 
                !authRestApiRequest.Head.Extensions.ContainsKey("uid"))
            {
                return null;
            }

            return authRestApiRequest.Head.Extensions["uid"];
        }
    }
}