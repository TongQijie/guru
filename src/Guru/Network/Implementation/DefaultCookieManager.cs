using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Network.Abstractions;

namespace Guru.Network.Implementation
{
    [Injectable(typeof(ICookieManager), Lifetime.Transient)]
    public class DefaultCookieManager : ICookieManager
    {
        private Dictionary<string, string> _Cookies = new Dictionary<string, string>();

        private readonly ILogger _Logger;

        public DefaultCookieManager(IFileLogger logger)
        {
            _Logger = logger;
        }

        public string GetCookies()
        {
            var stringBuilder = new StringBuilder();
            foreach (var cookie in _Cookies)
            {
                stringBuilder.Append($"{cookie.Key}={cookie.Value};");
            }
            return stringBuilder.ToString().TrimEnd(';');
        }

        public void SetCookie(string cookieString)
        {
            var match = Regex.Match(cookieString, "^\\w+=.+?;");
            if (match.Success)
            {
                var seperator = match.Value.IndexOf('=');
                var key = match.Value.Substring(0, seperator);
                var value = match.Value.Substring(seperator + 1).TrimEnd(';');
                if (_Cookies.ContainsKey(key))
                {
                    _Cookies[key] = value;
                }
                else
                {
                    _Cookies.Add(key, value);
                }

                _Logger.LogEvent(nameof(DefaultCookieManager), Severity.Information, $"SetCookie: {key}={value}");
            }
        }
    }
}