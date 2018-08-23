using System.Collections.Generic;

namespace Guru.Http
{
    public class RequestParams
    {
        public IDictionary<string, string> QueryString { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}