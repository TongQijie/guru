using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.AspNetCore.WebUtilities;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Network
{
    [DI(typeof(IHttpClientBroker), Lifetime = Lifetime.Singleton)]
    public class DefaultHttpClientBroker : IHttpClientBroker
    {
        private ConcurrentDictionary<string, IHttpClientBroker> _Brokers = new ConcurrentDictionary<string, IHttpClientBroker>();

        private readonly IHttpClientSettings _Settings;

        private readonly HttpClient _Client;

        public DefaultHttpClientBroker(IHttpClientSettings settings)
        {
            _Settings = settings;

            var handler = new HttpClientHandler();

            // set values
            if (_Settings.Proxy != null)
            {
                handler.Proxy = _Settings.Proxy;
                handler.Credentials = _Settings.Proxy.Credentials;
            }

            if (_Settings.Timeout != null)
            {
                _Client.Timeout = (TimeSpan)_Settings.Timeout;
            }

            _Client = new HttpClient(handler);
        }
        
        public IHttpClientBroker Get()
        {
            IHttpClientBroker broker;
            if (!_Brokers.TryGetValue(DefaultHttpClientSettings.DefaultSettingId, out broker))
            {
                broker = new DefaultHttpClientBroker(new DefaultHttpClientSettings(null, null, null, null));
                _Brokers.AddOrUpdate(DefaultHttpClientSettings.DefaultSettingId, broker, (i, b) => broker);
            }

            return broker;
        }

        public IHttpClientBroker Get(string id)
        {
            IHttpClientBroker broker;
            if (!_Brokers.TryGetValue(id, out broker))
            {
                return null;
            }

            return broker;
        }

        public IHttpClientBroker Get(IHttpClientSettings settings)
        {
            IHttpClientBroker broker;
            if (!_Brokers.TryGetValue(settings.Id, out broker))
            {
                broker = new DefaultHttpClientBroker(settings);
                _Brokers.AddOrUpdate(DefaultHttpClientSettings.DefaultSettingId, broker, (i, b) => broker);
            }

            return broker;
        }

        public async Task<IHttpClientResponse> GetAsync(string uri, IDictionary<string, string> queryString)
        {
            if (queryString != null)
            {
                uri = QueryHelpers.AddQueryString(uri, queryString);
            }

            return new DefaultHttpClientResponse(await _Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead));
        }

        public async Task<IHttpClientResponse> PostAsync<TFormatter>(string uri, IDictionary<string, string> queryString, object body) where TFormatter : IFormatter
        {
            if (queryString != null)
            {
                uri = QueryHelpers.AddQueryString(uri, queryString);
            }

            var bytes = ContainerEntry.Resolve<TFormatter>().WriteBytes(body);

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentLength = bytes.Length;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                requestMessage.Content = content;

                return new DefaultHttpClientResponse(await _Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));
            }
        }
    }
}