using Microsoft.AspNetCore.Http;
using Guru.ExtensionMethod;

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

        public static string GetHeaderValue(string key)
        {
            if (Current != null &&
                Current.Request != null &&
                Current.Request.Headers != null &&
                Current.Request.Headers.ContainsKey(key))
            {
                return string.Join(";", Current.Request.Headers[key]);
            }

            return string.Empty;
        }

        public static void SetValue(object key, object value)
        {
            if (Current.Items.ContainsKey(key))
            {
                Current.Items[key] = value;
            }
            else
            {
                Current.Items.Add(key, value);
            }
        }

        public static T GetValue<T>(object key)
        {
            if (Current.Items.ContainsKey(key))
            {
                return Current.Items[key].ConvertTo<T>(default(T));
            }
            else
            {
                return default(T);
            }
        }
    }
}