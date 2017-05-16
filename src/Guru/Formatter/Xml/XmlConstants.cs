namespace Guru.Formatter.Xml
{
    internal static class XmlConstants
    {
        /// <summary>
        /// '<'
        /// </summary>
        public static byte Lt = 0x3C;

        /// <summary>
        /// '>'
        /// </summary>
        public static byte Gt = 0x3E;

        /// <summary>
        /// '/'
        /// </summary>
        public const byte Slash = 0x2F;

        /// <summary>
        /// '='
        /// </summary>
        public const byte Eq = 0x3D;

        /// <summary>
        /// '"'
        /// </summary>
        public const byte Double_Quotes = 0x22;

        /// <summary>
        /// '!'
        /// </summary>
        public const byte Exclamation_Mark = 0x21;

        /// <summary>
        /// '-'
        /// </summary>
        public const byte Minus = 0x2D;

        /// <summary>
        /// '['
        /// </summary>
        public const byte Left_Square_Bracket = 0x5B;

        /// <summary>
        /// ']'
        /// </summary>
        public const byte Right_Square_bracket = 0x5D;

        /// <summary>
        /// 'C'
        /// </summary>
        public const byte C = 0x43;

        /// <summary>
        /// 'D'
        /// </summary>
        public const byte D = 0x44;

        /// <summary>
        /// 'A'
        /// </summary>
        public const byte A = 0x41;

        /// <summary>
        /// 'T'
        /// </summary>
        public const byte T = 0x54;

        public static byte[] TrueValueBytes = new byte[] { 0x74, 0x72, 0x75, 0x65 };

        public static byte[] FalseValueBytes = new byte[] { 0x66, 0x61, 0x6C, 0x73, 0x65 };

        public static bool IsPrintableChar(byte b)
        {
            return b > 0x20 && b <= 0x7E;
        }
    }
}