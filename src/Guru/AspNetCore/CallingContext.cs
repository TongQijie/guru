using System.IO;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;
using Guru.Foundation;

namespace Guru.AspNetCore
{
    public class CallingContext
    {
        public DictionaryIgnoreCase<ContextParameter> InputParameters { get; set; }

        public DictionaryIgnoreCase<ContextParameter> OutputParameters { get; set; }

        public Stream InputStream { get; set; }

        public string[] RouteData { get; set; }

        public Stream OutputStream { get; set; }

        public SetOutputParameterDelegate SetOutputParameter = null;

        public IApplicationConfiguration ApplicationConfiguration { get; set; }
    }
}