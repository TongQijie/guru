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


        public static byte[] TrueValueBytes = new byte[] { 0x74, 0x72, 0x75, 0x65 };

        public static byte[] FalseValueBytes = new byte[] { 0x66, 0x61, 0x6C, 0x73, 0x65 };

        public static bool IsPrintableChar(byte b)
        {
            return b > 0x20 && b <= 0x7E;
        }
    }
}