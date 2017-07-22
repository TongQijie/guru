using System.Collections.Generic;
using System.IO;

namespace Guru.AspNetCore
{
    public class CallingContext
    {
        public Dictionary<string, string> InputParameters { get; set; }

        public Stream InputStream { get; set; }

        public Dictionary<string, string> OutputParameters { get; set; }

        public Stream OutputStream { get; set; }
    }
}