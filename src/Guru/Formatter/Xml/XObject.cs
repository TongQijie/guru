using System;
using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Xml.Internal;

namespace Guru.Formatter.Xml
{
    public class XObject : XBase
    {
        public XBase[] Elements { get; set; }

        public byte[] Buffer { get; set; }

        private byte[] _EndTagName = null;

        public byte[] EndTagName
        {
            get
            {
                if (_EndTagName != null)
                {
                    return _EndTagName;
                }

                _EndTagName = new byte[] { XmlConstants.Slash }.Append(Key);

                return _EndTagName;
            }
        }

        internal async Task<bool> FillAsync(BufferedReaderStream stream, byte[] terminators)
        {
            var b = await stream.SeekBytesUntilVisiableCharAsync();
            if (b == -1)
            {
                throw new Exception("");
            }

            if (b == XmlConstants.Lt)
            {
                var hasAttributes = false;
                var tagName = await stream.ReadBytesUntilMeetingAsync(x =>
                {
                    if (x == XmlConstants.Gt)
                    {
                        return true;
                    }
                    else if (!XmlConstants.IsPrintableChar(x))
                    {
                        hasAttributes = true;
                        return true;
                    }

                    return false;
                });

                if (tagName == null)
                {
                    throw new Exception("");
                }

                tagName = tagName.Subset(0, tagName.Length - 1);

                if (tagName.EqualsWith(EndTagName))
                {
                    return true;
                }

                var xObject = new XObject()
                {
                    Key = tagName,
                };

                if (hasAttributes)
                {
                    int k;
                    while ((k = await stream.SeekBytesUntilVisiableCharAsync()) != XmlConstants.Gt)
                    {
                        var key = await stream.ReadBytesUntilAsync(XmlConstants.Eq);
                        if (key == null)
                        {
                            throw new Exception("");
                        }

                        key = key.Append((byte)k);

                        var val = await stream.SeekBytesUntilVisiableCharAsync();
                        if (val != XmlConstants.Double_Quotes)
                        {
                            throw new Exception("");
                        }

                        var value = await stream.ReadBytesUntilAsync(XmlConstants.Double_Quotes);
                        if (value == null)
                        {
                            throw new Exception("");
                        }

                        xObject.Elements = xObject.Elements.Append(new XAttribute()
                        {
                            Key = key,
                            Value = value,
                        });
                    }

                    if (k != XmlConstants.Gt)
                    {
                        throw new Exception("");
                    }
                }

                while (!await xObject.FillAsync(stream, xObject.Key))
                {
                    ;
                }

                Elements = Elements.Append(xObject);

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
