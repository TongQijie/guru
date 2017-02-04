using Microsoft.AspNetCore.Http;

namespace Guru.Middleware
{
    public static class AspNetCoreHttpContext
    {
        private static IHttpContextAccessor _HttpContextAccessor;

        public static HttpContext Current => _HttpContextAccessor.HttpContext;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }
    }
}