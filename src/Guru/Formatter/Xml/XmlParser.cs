using Guru.Formatter.Internal;
using System;
using System.Threading.Tasks;

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

            var tagName = await stream.ReadBytesUntilAsync(XmlConstants.Gt);

            var xObject = new XObject()
            {
                Key = tagName,
            };

            while (!await xObject.FillAsync(stream, tagName))
            {
                ;
            }

            return xObject;
        }
    }
}