using Microsoft.AspNetCore.Http;

namespace Guru.AspNetCore
{
    public static class HttpContextUtils
    {
        private static IHttpContextAccessor _HttpContextAccessor;

        public static HttpContext Current => _HttpContextAccessor.HttpContext;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }
    }
}