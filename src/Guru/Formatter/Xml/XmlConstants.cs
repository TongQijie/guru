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


        public static byte[] TrueValueBytes = new byte[] { 0x74, 0x72, 0x75, 0x65 };

        public static byte[] FalseValueBytes = new byte[] { 0x66, 0x61, 0x6C, 0x73, 0x65 };
    }
}