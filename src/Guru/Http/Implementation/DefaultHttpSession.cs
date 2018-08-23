using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Abstractions;
using Guru.ExtensionMethod;
using System;
using System.Net;
using Guru.Http.Abstractions;

namespace Guru.Http.Implementation
{
    [Injectable(typeof(IHttpSession), Lifetime.Transient)]
    internal class DefaultHttpSession : IHttpSession
    {
        private readonly ICookieManager _CookieManager;

        private readonly IHttpManager _HttpManager;

        private IHttpRequest _HttpRequest;

        public DefaultHttpSession(IHttpManager httpManager, ICookieManager cookieManager)
        {
            _CookieManager = cookieManager;
            _HttpManager = httpManager;
            _HttpRequest = httpManager.Create();
        }

        public bool LocationEnabled { get; set; }

        public IHttpSession Configure(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout)
        {
            _HttpRequest = _HttpManager.Create(webProxy, ignoredCertificateValidation, timeout);
            return this;
        }

        public async Task<IHttpResponse> GetAsync(string url, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }

            requestParams.Headers = AppendCookies(requestParams.Headers);

            var response = SetCookies(await _HttpRequest.GetAsync(url, requestParams));
            if (LocationEnabled && response != null && response.Location.HasValue())
            {
                requestParams.QueryString = null;
                if (response.Location.StartsWith("/"))
                {
                    return await GetAsync(response.RequestUri.Scheme + "://" + response.RequestUri.Host + response.Location, requestParams);
                }
                else
                {
                    return await GetAsync(response.Location, requestParams);
                }
            }
            return response;
        }

        public async Task<IHttpResponse> PostAsync<TFormatter>(string url, object body, TFormatter formatter, RequestParams requestParams) where TFormatter : ILightningFormatter
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }
            requestParams.Headers = AppendCookies(requestParams.Headers);

            return SetCookies(await _HttpRequest.PostAsync(url, body, formatter, requestParams));
        }

        public async Task<IHttpResponse> PostAsync(string url, IDictionary<string, string> formData, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }
            requestParams.Headers = AppendCookies(requestParams.Headers);

            return SetCookies(await _HttpRequest.PostAsync(url, formData, requestParams));
        }

        public async Task<IHttpResponse> PostAsync(string url, byte[] byteArrayContent, RequestParams requestParams)
        {
            if (requestParams == null)
            {
                requestParams = new RequestParams();
            }
            requestParams.Headers = AppendCookies(requestParams.Headers);

            return SetCookies(await _HttpRequest.PostAsync(url, byteArrayContent, requestParams));
        }

        private IDictionary<string, string> AppendCookies(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }

            var cookies = _CookieManager.GetCookies();
            if (cookies.HasValue())
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
            }
            return headers;
        }

        private IHttpResponse SetCookies(IHttpResponse response)
        {
            if (response != null && response.Headers != null && response.Headers.ContainsKey("Set-Cookie"))
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
