using System;
using System.Net;
using System.Text;
using System.Collections.Generic;

using Guru.ExtensionMethod;
using Guru.Network.Abstractions;

namespace Guru.Network
{
    public class DefaultHttpClientSettings : IHttpClientSettings
    {
        private static string _DefaultSettingId = null;

        public static string DefaultSettingId
        {
            get
            {
                if (_DefaultSettingId == null)
                {
                    var settings = new DefaultHttpClientSettings(null, null, null, null);

                    _DefaultSettingId = settings.Id;
                }

                return _DefaultSettingId;
            }
        }

        private string _Id;

        private IDictionary<string, string[]> _Headers = new Dictionary<string, string[]>();

        private IWebProxy _Proxy;

        private TimeSpan? _Timeout;

        public DefaultHttpClientSettings(string id)
        {
            _Id = id;
        }

        public DefaultHttpClientSettings(string id, IDictionary<string, string[]> headers, IWebProxy proxy, TimeSpan? timeout)
        {
            _Id = id;
            _Headers = headers;
            _Proxy = proxy;
            _Timeout = timeout;
        }

        public string Id
        {
            get
            {
                if (!_Id.HasValue())
                {
                    var stringBuilder = new StringBuilder();
                    
                    if (_Headers != null)
                    {
                        foreach (var header in _Headers)
                        {
                            stringBuilder.AppendLine($"{header.Key}:{string.Join(";", header.Value)}");
                        }
                    }

                    stringBuilder.AppendLine();

                    if (_Proxy != null)
                    {
                        if (_Proxy.Credentials == null)
                        {
                            stringBuilder.AppendLine($"proxy:{_Proxy.GetProxy(null)?.ToString()}");
                        }
                        else
                        {
                            var credentials = _Proxy.Credentials.GetCredential(null, null);
                            stringBuilder.AppendLine($"proxy:{_Proxy.GetProxy(null)?.ToString()};{credentials?.UserName};{credentials?.Password};{credentials?.Domain}");
                        }
                    }

                    stringBuilder.AppendLine();

                    if (_Timeout != null)
                    {
                        stringBuilder.AppendLine($"timeout:{_Timeout?.TotalSeconds}");
                    }

                    _Id = stringBuilder.ToString();
                }

                return _Id;
            }
        }

        public IDictionary<string, string[]> Headers => _Headers;

        public IWebProxy Proxy => _Proxy;

        public TimeSpan? Timeout => _Timeout;
    }
}