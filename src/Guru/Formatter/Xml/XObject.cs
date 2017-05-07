using System;
using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Xml
{
    public class XObject : XBase
    {
        public XBase[] Elements { get; set; }

        public byte[] Buffer { get; set; }

        internal async Task<bool> FillAsync(IReaderStream stream, byte[] terminators)
        {
            var b = await stream.SeekBytesUntilVisiableCharAsync();
            if (b == -1)
            {
                throw new Exception("");
            }

            if (b == XmlConstants.Lt)
            {
                var innerTagName = await stream.ReadBytesUntilAsync(XmlConstants.Gt);
                if (!innerTagName.HasLength())
                {
                    throw new Exception("");
                }

                if (innerTagName[0] == XmlConstants.Slash && innerTagName.Subset(1).EqualsWith(Key))
                {
                    return true;
                }

                var xObject = new XObject()
                {
                    Key = innerTagName,
                };

                Elements = Elements.Append(xObject);

                while (!await xObject.FillAsync(stream, innerTagName))
                {
                    ;
                }

                return false;
            }
            else
            {
                Buffer = new byte[] { (byte)b };

                var value = await stream.ReadBytesUntilAsync(XmlConstants.Lt);

                Buffer = Buffer.Append(value);

                var endTag = await stream.ReadBytesUntilAsync(XmlConstants.Gt);
                if (!endTag.HasLength() || endTag[0] != XmlConstants.Slash || !endTag.Subset(1).EqualsWith(Key))
                {
                    throw new Exception("");
                }

                return true;
            }
        }
    }
}
