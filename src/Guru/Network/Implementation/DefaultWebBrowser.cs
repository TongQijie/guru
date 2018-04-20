using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Network.Abstractions;
using Guru.Utils;

namespace Guru.Network.Implementation
{
    [Injectable(typeof(IWebBrowser), Lifetime.Transient)]
    public class DefaultWebBrowser : IWebBrowser
    {
        private readonly ICookieManager _CookieManager;

        private readonly IHttpClientBroker _HttpClientBroker;

        public DefaultWebBrowser(IHttpClientBroker httpClientBroker, ICookieManager cookieManager)
        {
            _CookieManager = cookieManager;
            _HttpClientBroker = httpClientBroker;
        }

        public async Task<IWebBrowser> Browse(string url, WebBrowserSucceedDelegate succeed)
        {
            using (var response = await GetHttpClientRequest().GetAsync(url))
            {
                // set cookies if needed
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

                if (response.StatusCode == 200)
                {
                    if (succeed != null)
                    {
                        await succeed(response);
                    }
                }
                else if (response.StatusCode == 301)
                {
                    // TODO: relocate
                }
                else
                {
                    // 
                }
            }
            return this;
        }

        public async Task<IWebBrowser> Browse(string url, IDictionary<string, string> queryString, WebBrowserSucceedDelegate succeed)
        {
            await Browse(AddQueryString(url, queryString), succeed);
            return this;
        }

        private string AddQueryString(string uri, IDictionary<string, string> queryString)
        {
            //uri = uri.TrimEnd('/', '?');

            if (uri.ContainsIgnoreCase("?"))
            {
                uri = uri + "&";
            }
            else
            {
                uri = uri + "?";
            }

            return uri + string.Join("&", queryString.Where(x => x.Key.HasValue())
                .Select(x => x.Value.HasValue() ? 
                    $"{WebUtils.UrlEncode(x.Key)}={WebUtils.UrlEncode(x.Value)}" : 
                    $"{WebUtils.UrlEncode(x.Key)}"));
        }

        private IHttpClientRequest GetHttpClientRequest()
        {
            return _HttpClientBroker.Get(new DefaultHttpClientSettings("", new Dictionary<string, string[]>()
            {
                { "Cookie", new string[] { _CookieManager.GetCookies() } }
            }, null, null));
        }
    }
}