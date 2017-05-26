using System;
using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Xml
{
    internal static class XmlParser
    {
        public static async Task<XObject> ParseAsync(IReaderStream stream)
        {
            var b = await stream.SeekBytesUntilVisiableCharAsync();
            if (b == -1 || b != XmlConstants.Lt)
            {
                throw new Exception();
            }

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

                    key = new byte[] { (byte)k }.Append(key);

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

            while (!await xObject.FillAsync(stream))
            {
                ;
            }

            return xObject;
        }
    }
}