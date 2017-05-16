﻿namespace Guru.Formatter.Xml
{
    public class XValue : XBase
    {
        public byte[] Value { get; set; }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Value);
        }
    }
}