using System;
using System.Threading.Tasks;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Xml
{
    public class XArray : XBase
    {
        public XBase[] Elements { get; set; }

        internal override Task<bool> FillAsync(IReaderStream stream, byte[] terminators)
        {
            throw new NotImplementedException();
        }
    }
}