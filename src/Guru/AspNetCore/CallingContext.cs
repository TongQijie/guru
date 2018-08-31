using System.IO;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;
using Guru.Foundation;

namespace Guru.AspNetCore
{
    public class CallingContext
    {
        public IgnoreCaseKeyValues<string> RequestHttpParameters { get; set; }

        public IgnoreCaseKeyValues<string> RequestHeaderParameters { get; set; }

        public IgnoreCaseKeyValues<ContextParameter> InputParameters { get; set; }

        public IgnoreCaseKeyValues<string> ResponseHttpParameters { get; set; }

        public IgnoreCaseKeyValues<string> ResponseHeaderParameters { get; set; }

        public Stream InputStream { get; set; }

        public string[] RouteData { get; set; }

        public Stream OutputStream { get; set; }

        public SetOutputParameterDelegate SetOutputParameter = null;

        public IApplicationConfiguration ApplicationConfiguration { get; set; }
    }
}