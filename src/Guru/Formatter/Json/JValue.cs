using Guru.ExtensionMethod;
using Guru.Formatter.Internal;
using System;
using System.Text;

namespace Guru.Formatter.Json
{
    public class JValue : JElement
    {
        private bool? _EncompassedByQuote = null;

        private bool EncompassedByQuote
        {
            get
            {
                if (_EncompassedByQuote == null)
                {
                    _EncompassedByQuote = InternalType == typeof(string) || InternalType == typeof(DateTime);
                }

                return (bool)_EncompassedByQuote;
            }
        }

        internal object ReadValue(IReaderStream reader, Encoding encoding)
        {
            var b = reader.SeekBytesUntilVisiableChar();
            if (b == -1)
            {
                throw new Exception();
            }

            if (b == JsonConstants.Double_Quotes)
            {
                return encoding.GetString(reader.GetStringValue()).ConvertTo(InternalType);
            }
            else
            {
                byte[] terminators = new byte[] { JsonConstants.Comma, 0x00 };
                if (Parent != null)
                {
                    if (Parent is JObject)
                    {
                        terminators[1] = JsonConstants.Right_Brace;
                    }
                    else if (Parent is JArray)
                    {
                        terminators[1] = JsonConstants.Right_Bracket;
                    }
                }

                var buf = reader.ReadBytesUntil(terminators);
                if (buf == null || buf.Length == 0)
                {
                    throw new Errors.JsonParseFailedException(reader.Position, "plain value cannot be empty.");
                }

                var stringValue = encoding.GetString(buf, 0, buf.Length - 1).Trim();

                switch (stringValue)
                {
                    case "null": return null;
                    case "true": return true;
                    case "false": return false;
                    default: return stringValue.ConvertTo(InternalType);
                }
            }
        }
    }
}
