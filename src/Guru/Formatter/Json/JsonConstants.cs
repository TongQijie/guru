namespace Guru.Formatter.Json
{
    public static class JsonConstants
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

        public static byte[] NullValueBytes = new byte[] { 0x6E, 0x75, 0x6C, 0x6C };

        public static byte[] TrueValueBytes = new byte[] { 0x74, 0x72, 0x75, 0x65 };

        public static byte[] FalseValueBytes = new byte[] { 0x66, 0x61, 0x6C, 0x73, 0x65 };
    }
}