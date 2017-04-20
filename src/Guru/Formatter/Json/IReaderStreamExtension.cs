using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    internal static class IReaderStreamExtension
    {
        public static byte[] GetStringValue(this IReaderStream reader)
        {
            var buffer = new byte[0];

            var buf = new byte[0];
            while ((buf = reader.ReadBytesUntil(new byte[] { JsonConstants.Double_Quotes, JsonConstants.Backslash })) != null)
            {
                if (buf[buf.Length - 1] == JsonConstants.Double_Quotes)
                {
                    return Concat(buffer, 0, buffer.Length, buf, 0, buf.Length - 1);
                }

                if (buf[buf.Length - 1] == JsonConstants.Backslash)
                {
                    var b = reader.ReadByte();
                    if (b == -1)
                    {
                        return null;
                    }

                    if (b == JsonConstants.U)
                    {
                        var escape = JsonCharacterEscape.Unescape(reader.ReadBytes(4));
                        buf = Concat(buf, 0, buf.Length - 1, escape, 0, escape.Length);
                    }
                    else
                    {
                        buf[buf.Length - 1] = JsonCharacterEscape.Unescape(b);
                    }

                    buffer = Concat(buffer, 0, buffer.Length, buf, 0, buf.Length);
                }
            }

            return buffer;
        }

        private static byte[] Concat(byte[] firstArray, int firstStart, int firstCount, byte[] secondArray, int secondStart, int secondCount)
        {
            var buf = new byte[firstCount + secondCount];

            System.Buffer.BlockCopy(firstArray, firstStart, buf, 0, firstCount);

            System.Buffer.BlockCopy(secondArray, secondStart, buf, firstCount, secondCount);

            return buf;
        }
    }
}
