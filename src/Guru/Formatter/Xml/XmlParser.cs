using System;
using System.Threading.Tasks;

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

            var startTag = await stream.ReadBytesUntilAsync(XmlConstants.Gt);

            var xObject = new XObject()
            {
                Key = startTag,
            };

            while (!await xObject.FillAsync(stream, startTag))
            {
                ;
            }

            return xObject;
        }
    }
}