using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.Network.Abstractions;
using Guru.ExtensionMethod;

namespace Guru.Network.Implementation
{
    [Injectable(typeof(IHttpSession), Lifetime.Transient)]
    internal class DefaultHttpSession : IHttpSession
    {
        private readonly IHttpManager _HttpManager;

        private readonly ICookieManager _CookieManager;

        public DefaultHttpSession(IHttpManager httpManager, ICookieManager cookieManager)
        {
            _HttpManager = httpManager;
            _CookieManager = cookieManager;
        }

        public async Task<IHttpResponse> GetAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> headers = null)
        {
            return SetCookies(await _HttpManager.Create().GetAsync(url, queryString, AppendCookies(headers)));
        }

        public async Task<IHttpResponse> PostAsync<TFormatter>(string url, IDictionary<string, string> queryString, object body, TFormatter formatter, IDictionary<string, string> headers = null) where TFormatter : ILightningFormatter
        {
            return SetCookies(await _HttpManager.Create().PostAsync(url, queryString, body, formatter, AppendCookies(headers)));
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, IDictionary<string, string> formData, IDictionary<string, string> headers = null)
        {
            return SetCookies(await _HttpManager.Create().PostAsync(url, queryString, formData, AppendCookies(headers)));
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> queryString, byte[] byteArrayContent, IDictionary<string, string> headers = null)
        {
            return SetCookies(await _HttpManager.Create().PostAsync(url, queryString, byteArrayContent, AppendCookies(headers)));
        }

        private IDictionary<string, string> AppendCookies(IDictionary<string, string> headers)
        {
            var cookieKey = headers.Keys.FirstOrDefault(x => x.EqualsIgnoreCase("Cookie"));
            if (cookieKey == null)
            {
                headers.Add("Cookie", _CookieManager.GetCookies());
            }
            else
            {
                headers["Cookie"] = headers["Cookie"].TrimEnd(';') + ";" + _CookieManager.GetCookies();
            }
            return headers;
        }

        private IHttpResponse SetCookies(IHttpResponse response)
        {
            if (response.Headers != null && response.Headers.ContainsKey("Set-Cookie"))
            {
                var setCookes = response.Headers["Set-Cookie"];
                if (setCookes.HasLength())
                {
                    foreach (var cookieString in setCookes)
                    {
                        _CookieManager.SetCookie(cookieString);
                    }
                }
            }
            return response;
        }
    }
}
