using System;
using System.Net;

namespace Guru.Http
{
    public class DefaultWebProxy : IWebProxy
    {
        private readonly string _Uri;

        private readonly int _Port;

        public DefaultWebProxy(string uri, int port, string username, string password, string domain)
        {
            _Uri = uri;
            _Port = port;

            Credentials = new DefaultCredentials(username, password, domain);
        }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return new Uri($"{_Uri}:{_Port}");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}