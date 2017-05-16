using System;
using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Xml.Internal;

namespace Guru.Formatter.Xml
{
    public class XObject : XBase
    {
        public byte[] Key { get; set; }

        public XBase[] Elements { get; set; }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Key);
        }

        private int _LastByte = -1;

        internal async Task<bool> FillAsync(BufferedReaderStream stream)
        {
            int b = _LastByte;
            if (b == -1)
            {
                b = await stream.SeekBytesUntilVisiableCharAsync();
            }

            if (b == -1)
            {
                throw new Exception("");
            }

            if (b == XmlConstants.Lt)
            {
                var hasAttributes = false;
                var toEnd = false;
                var beginComment = false;
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
                    else if (x == XmlConstants.Slash)
                    {
                        hasAttributes = true;
                        toEnd = true;
                        return true;
                    }
                    else if (x == XmlConstants.Exclamation_Mark)
                    {
                        beginComment = true;
                        return true;
                    }

                    return false;
                });

                if (tagName == null)
                {
                    throw new Exception("");
                }

                if (beginComment)
                {
                    var endByte = 0x00;

                    var f2 = await stream.ReadBytesAsync(2);
                    if (f2.EqualsWith(new byte[] { XmlConstants.Minus, XmlConstants.Minus }))
                    {
                        endByte = XmlConstants.Minus;
                    }
                    else if (f2.EqualsWith(new byte[] { XmlConstants.Left_Square_Bracket, XmlConstants.C }))
                    {
                        var f5 = await stream.ReadBytesAsync(5);
                        if (f5.EqualsWith(new byte[] { XmlConstants.D, XmlConstants.A, XmlConstants.T, XmlConstants.A, XmlConstants.Left_Square_Bracket }))
                        {
                            endByte = XmlConstants.Right_Square_bracket;
                        }
                    }

                    if (endByte == 0x00)
                    {
                        throw new Exception("");
                    }

                    var comment = new byte[0];
                    while (!(comment.Length >= 2 && comment[comment.Length - 1] == endByte && comment[comment.Length - 2] == endByte))
                    {
                        var c = await stream.ReadBytesUntilAsync(XmlConstants.Gt);
                        if (c == null)
                        {
                            throw new Exception("");
                        }

                        comment = comment.Append(c);
                    }

                    if (endByte == XmlConstants.Minus)
                    {
                        Elements = Elements.Append(new XComment()
                        {
                            Value = comment.Subset(0, comment.Length - 2),
                        });
                    }
                    else if (endByte == XmlConstants.Right_Square_bracket)
                    {
                        Elements = Elements.Append(new XData()
                        {
                            Value = comment.Subset(0, comment.Length - 2),
                        });
                    }

                    _LastByte = -1;

                    return false;
                }

                if (toEnd && tagName.Length == 0)
                {
                    tagName = await stream.ReadBytesUntilAsync(XmlConstants.Gt);
                }

                if (tagName.EqualsWith(Key))
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
                        if (k == XmlConstants.Slash)
                        {
                            toEnd = true;
                            continue;
                        }

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

                while (!toEnd && !await xObject.FillAsync(stream))
                {
                    ;
                }

                Elements = Elements.Append(xObject);

                _LastByte = -1;

                return false;
            }
            else
            {
                var value = await stream.ReadBytesUntilAsync(XmlConstants.Lt);
                if (value == null)
                {
                    throw new Exception("");
                }

                var xValue = new XValue()
                {
                    Value = new byte[] { (byte)b }.Append(value),
                };

                Elements = Elements.Append(xValue);

                _LastByte = XmlConstants.Lt;

                return false;
            }
        }
    }
}
