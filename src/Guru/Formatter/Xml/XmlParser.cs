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
            byte[] tagName = null;
            var hasAttributes = false;

            var tuple = await ParseAsyncWithHeader(stream);
            if (tuple != null)
            {
                tagName = tuple.Item1;
                hasAttributes = tuple.Item2;
            }
            else
            {
                var b = await stream.SeekBytesUntilVisiableCharAsync();
                if (b == -1 || b != XmlConstants.Lt)
                {
                    throw new Exception();
                }

                hasAttributes = false;
                tagName = await stream.ReadBytesUntilMeetingAsync(x =>
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
            }

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

        private static async Task<Tuple<byte[], bool>> ParseAsyncWithHeader(IReaderStream stream)
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

            if (tagName.EqualsWith(new byte[] { 0x3F, 0x78, 0x6D, 0x6C }))
            {
                // remove xml header
                await stream.ReadBytesUntilAsync(XmlConstants.Gt);
                return null;
            }
            else
            {
                return new Tuple<byte[], bool>(tagName, hasAttributes);
            }
        }
    }
}