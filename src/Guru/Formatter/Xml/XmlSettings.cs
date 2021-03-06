﻿using System;
using System.Net;
using System.Text;

namespace Guru.Formatter.Xml
{
    internal class XmlSettings
    {
        private readonly Encoding _CurrentEncoding;

        private readonly bool _OmitDefaultValue;

        public XmlSettings()
        {
            _CurrentEncoding = Encoding.UTF8;
            _OmitDefaultValue = true;
        }

        public XmlSettings(Encoding encoding, bool omitDefaultValue)
        {
            _CurrentEncoding = encoding;
            _OmitDefaultValue = omitDefaultValue;
        }

        public Encoding CurrentEncoding => _CurrentEncoding;

        public bool OmitDefaultValue => _OmitDefaultValue;

        public byte[] SerializeValue(object value, bool cdata)
        {
            var valueType = value.GetType();

            var underlyingType = Nullable.GetUnderlyingType(value.GetType());
            if (underlyingType != null)
            {
                valueType = underlyingType;
            }

            if (valueType == typeof(DateTime))
            {
                // convert to datetime format
                return CurrentEncoding.GetBytes($"{((DateTime)value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}");
            }
            else if (valueType == typeof(string))
            {
                // escape special characters
                if (cdata)
                {
                    return CurrentEncoding.GetBytes("<![CDATA[" + value.ToString() + "]]>");
                }
                else
                {
                    return CurrentEncoding.GetBytes(WebUtility.HtmlEncode(value.ToString()));
                }
            }
            else if (valueType == typeof(bool))
            {
                if ((bool)value)
                {
                    return XmlConstants.TrueValueBytes;
                }
                else
                {
                    return XmlConstants.FalseValueBytes;
                }
            }
            else
            {
                return CurrentEncoding.GetBytes(value.ToString());
            }
        }
    }
}