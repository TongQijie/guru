using System.Collections.Generic;
using System.IO;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;

namespace Guru.AspNetCore
{
    public class CallingContext
    {
        public Dictionary<string, ContextParameter> InputParameters { get; set; }

        public Stream InputStream { get; set; }

        public string[] RouteData { get; set; }

        public Dictionary<string, ContextParameter> OutputParameters { get; set; }

        public Stream OutputStream { get; set; }

        public SetOutputParameterDelegate SetOutputParameter = null;

        public IApplicationConfiguration ApplicationConfiguration { get; set; }
    }
}