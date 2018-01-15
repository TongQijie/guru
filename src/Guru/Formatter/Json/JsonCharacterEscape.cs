using System.Text.RegularExpressions;

namespace Guru.Formatter.Json
{
    internal static class JsonCharacterEscape
    {
        public static byte Unescape(int source)
        {
            switch (source)
            {
                case JsonConstants.Double_Quotes:
                    {
                        return JsonConstants.Double_Quotes;
                    }
                case JsonConstants.Backslash:
                    {
                        return JsonConstants.Backslash;
                    }
                case JsonConstants.Slash:
                    {
                        return JsonConstants.Slash;
                    }
                case JsonConstants.B:
                    {
                        return JsonConstants.Backspace;
                    }
                case JsonConstants.F:
                    {
                        return JsonConstants.Formfeed;
                    }
                case JsonConstants.N:
                    {
                        return JsonConstants.Newline;
                    }
                case JsonConstants.R:
                    {
                        return JsonConstants.CarriageReturn;
                    }
                case JsonConstants.T:
                    {
                        return JsonConstants.HorizontalTab;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        public static byte[] Unescape(byte[] source)
        {
            if (source == null || source.Length != 4)
            {
                return new byte[0];
            }

            var firstByte = (ConvertHexByte(source[0]) << 4) + ConvertHexByte(source[1]);
            var secondByte = (ConvertHexByte(source[2]) << 4) + ConvertHexByte(source[3]);

            if (firstByte == 0)
            {
                return new byte[] { (byte)secondByte };
            }
            else
            {
                return new byte[] { (byte)firstByte, (byte)secondByte };
            }
        }

        private static int ConvertHexByte(byte byteValue)
        {
            if (byteValue >= 0x30 && byteValue <= 0x39)
            {
                return byteValue - 0x30;
            }
            else if (byteValue >= 0x41 && byteValue <= 0x46)
            {
                return (byteValue - 0x41) + 0x0A;
            }
            else if (byteValue >= 0x61 && byteValue <= 0x66)
            {
                return (byteValue - 0x61) + 0x0A;
            }
            else
            {
                return 0;
            }
        }

        public static string Escape(string source)
        {
            return Regex.Replace(source, "([\\\\\"/]{1})", "\\$1")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}