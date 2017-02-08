using System;
using System.Net;
using System.Collections.Generic;

namespace Guru.Network
{
    public interface IHttpClientSettings
    {
         string Id { get; }

         IDictionary<string, string[]> Headers { get; }

         IWebProxy Proxy { get; }

         TimeSpan? Timeout { get; }
    }
}