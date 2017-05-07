using System;
using System.Threading.Tasks;
using Guru.Formatter.Internal;
using Guru.ExtensionMethod;

namespace Guru.Formatter.Xml
{
    public class XValue : XBase
    {
        public byte[] Buffer { get; set; }

        internal override async Task<bool> FillAsync(IReaderStream stream, byte[] terminators)
        {
            Buffer = Buffer.Append(await stream.ReadBytesUntilAsync(XmlConstants.Lt));

            var endTag = await stream.ReadBytesUntilAsync(XmlConstants.Gt);

            if (!endTag.HasLength() || endTag[0] != XmlConstants.Slash || !endTag.Subset(1).EqualsWith(terminators))
            {
                throw new Exception("");
            }

            return true;
        }
    }
}