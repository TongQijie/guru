using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Guru.Formatter.Json
{
    internal static class JsonEncoder
    {
        /// <summary>
        /// '{'
        /// </summary>
        public const byte Left_Brace = 0x7B;
        /// <summary>
        /// '}'
        /// </summary>
        public const byte Right_Brace = 0x7D;
        /// <summary>
        /// '['
        /// </summary>
        public const byte Left_Bracket = 0x5B;
        /// <summary>
        /// ']'
        /// </summary>
        public const byte Right_Bracket = 0x5D;
        /// <summary>
        /// ','
        /// </summary>
        public const byte Comma = 0x2C;
        /// <summary>
        /// ':'
        /// </summary>
        public const byte Colon = 0x3A;
        /// <summary>
        /// '"'
        /// </summary>
        public const byte Double_Quotes = 0x22;
        /// <summary>
        /// '\'
        /// </summary>
        public const byte Backslash = 0x5C;
        /// <summary>
        /// '/'
        /// </summary>
        public const byte Slash = 0x2F;
        /// <summary>
        /// ' '
        /// </summary>
        public const byte Whitespace = 0x20;
        /// <summary>
        /// '\b'
        /// </summary>
        public const byte Backspace = 0x08;
        /// <summary>
        /// '\f'
        /// </summary>
        public const byte Formfeed = 0x0C;
        /// <summary>
        /// '\n'
        /// </summary>
        public const byte Newline = 0x0A;
        /// <summary>
        /// '\r'
        /// </summary>
        public const byte CarriageReturn = 0x0D;
        /// <summary>
        /// '\t'
        /// </summary>
        public const byte HorizontalTab = 0x09;
        /// <summary>
        /// 'b'
        /// </summary>
        public const byte B = 0x62;
        /// <summary>
        /// 'f'
        /// </summary>
        public const byte F = 0x66;
        /// <summary>
        /// 'n'
        /// </summary>
        public const byte N = 0x6E;
        /// <summary>
        /// 'r'
        /// </summary>
        public const byte R = 0x72;
        /// <summary>
        /// 't'
        /// </summary>
        public const byte T = 0x74;
        /// <summary>
        /// 'u'
        /// </summary>
        public const byte U = 0x75;
        /// <summary>
        /// "null"
        /// </summary>
        public const string NullValueMark = "null";
        /// <summary>
        /// current encoding: UTF8 is default.
        /// </summary>
        public static Encoding CurrentEncoding = Encoding.UTF8;

        public static string GetString(byte[] byteValues)
        {
            return CurrentEncoding.GetString(byteValues);
        }

        public static byte[] GetBytes(string stringValue)
        {
            return CurrentEncoding.GetBytes(stringValue);
        }

        public static byte[] GetElementName(string elementName)
        {
            var name = GetBytes(elementName);

            var dest = new byte[name.Length + 3];
            dest[0] = Double_Quotes;
            Array.Copy(name, 0, dest, 1, name.Length);
            dest[dest.Length - 2] = Double_Quotes;
            dest[dest.Length - 1] = Colon;
            return dest;
        }
        
        public static byte[] GetPlainValue(object elementValue)
        {
            if (elementValue == null)
            {
                return GetNullValue();
            }
            else if (elementValue is bool)
            {
                return GetBooleanValue((bool)elementValue);
            }
            else if (elementValue is string)
            {
                var byteValues = GetBytes(Doescape(elementValue.ToString()));
                var buf = new byte[2 + byteValues.Length];
                buf[0] = Double_Quotes;
                buf[buf.Length - 1] = Double_Quotes;
                Array.Copy(byteValues, 0, buf, 1, byteValues.Length);
                return buf;
            }
            else if (elementValue is DateTime)
            {
                var byteValues = GetBytes(((DateTime)elementValue).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                var buf = new byte[2 + byteValues.Length];
                buf[0] = Double_Quotes;
                buf[buf.Length - 1] = Double_Quotes;
                Array.Copy(byteValues, 0, buf, 1, byteValues.Length);
                return buf;
            }
            else
            {
                return GetBytes(elementValue.ToString());
            }
        }

        public static byte[] GetNullValue()
        {
            return new byte[] { 0x6E, 0x75, 0x6C, 0x6C };
        }

        public static byte[] GetBooleanValue(bool boolean)
        {
            if (boolean)
            {
                return new byte[] { 0x74, 0x72, 0x75, 0x65 };
            }
            else
            {
                return new byte[] { 0x66, 0x61, 0x6C, 0x73, 0x65 };
            }
        }

        public static bool IsNullValue(object value)
        {
            return string.Equals(value.ToString(), NullValueMark, StringComparison.OrdinalIgnoreCase);
        }

        public static byte DoUnescape(int source)
        {
            switch (source)
            {
                case Double_Quotes:
                    {
                        return Double_Quotes;
                    }
                case Backslash:
                    {
                        return Backslash;
                    }
                case Slash:
                    {
                        return Slash;
                    }
                case B:
                    {
                        return Backspace;
                    }
                case F:
                    {
                        return Formfeed;
                    }
                case N:
                    {
                        return Newline;
                    }
                case R:
                    {
                        return CarriageReturn;
                    }
                case T:
                    {
                        return HorizontalTab;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        public static byte[] DoUnescape(byte[] source)
        {
            if (source == null || source.Length != 4)
            {
                return new byte[0];
            }

            var firstByte = ConvertHexByte(source[0]) * 0x0F + ConvertHexByte(source[1]);
            var secondByte = ConvertHexByte(source[2]) * 0x0F + ConvertHexByte(source[3]);

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

        public static string Doescape(string source)
        {
            return Regex.Replace(source, "([\\\\\"/]{1})", "\\$1")
                .Replace("\b", "\\b").Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
