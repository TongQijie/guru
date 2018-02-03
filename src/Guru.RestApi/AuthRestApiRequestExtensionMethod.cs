namespace Guru.RestApi
{
    public static class AuthRestApiRequestExtensionMethod
    {
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