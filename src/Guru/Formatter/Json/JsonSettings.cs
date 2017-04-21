using System;
using System.Text;

using Guru.ExtensionMethod;

namespace Guru.Formatter.Json
{
    internal class JsonSettings
    {
        private readonly Encoding _CurrentEncoding;

        private readonly bool _OmitDefaultValue;

        public JsonSettings()
        {
            _CurrentEncoding = Encoding.UTF8;
            _OmitDefaultValue = true;
        }

        public JsonSettings(Encoding encoding, bool omitDefaultValue)
        {
            _CurrentEncoding = encoding;
            _OmitDefaultValue = omitDefaultValue;
        }

        public Encoding CurrentEncoding => _CurrentEncoding;

        public bool OmitDefaultValue => _OmitDefaultValue;

        public object DeserializeValue(JValue value, Type targetType)
        {
            var stringValue = CurrentEncoding.GetString(value.Buffer);

            if (value.EncompassedByQuote)
            {
                // only support string type or datetime type
                return stringValue.ConvertTo(targetType);
            }
            else
            {
                stringValue = stringValue.Trim();

                if ("null".EqualsIgnoreCase(stringValue))
                {
                    // null
                    return null;
                }
                else if ("true".EqualsIgnoreCase(stringValue))
                {
                    // true
                    return true;
                }
                else if ("false".EqualsIgnoreCase(stringValue))
                {
                    // false
                    return false;
                }
                else
                {
                    // number
                    return stringValue.ConvertTo(targetType);
                }
            }
        }

        public byte[] SerializeValue(object value)
        {
            if (value == null)
            {
                return JsonConstants.NullValueBytes;
            }

            var valueType = value.GetType();

            var underlyingType = Nullable.GetUnderlyingType(value.GetType());
            if (underlyingType != null)
            {
                valueType = underlyingType;
            }

            if (valueType == typeof(DateTime))
            {
                // convert to datetime format
                return CurrentEncoding.GetBytes($"\"{((DateTime)value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\"");
            }
            else if (valueType == typeof(string))
            {
                // escape special characters
                return CurrentEncoding.GetBytes($"\"{JsonCharacterEscape.Escape(value.ToString())}\"");
            }
            else if (valueType == typeof(bool))
            {
                if ((bool)value)
                {
                    return JsonConstants.TrueValueBytes;
                }
                else
                {
                    return JsonConstants.FalseValueBytes;
                }
            }
            else
            {
                return CurrentEncoding.GetBytes(value.ToString());
            }
        }
    }
}