using System;
using System.Text;

namespace Guru.Util
{
    public static class WebUtil
    {
        #region UrlEncode implementation

        private static void GetEncodedBytes(byte[] originalBytes, int offset, int count, byte[] expandedBytes)
        {
            int pos = 0;
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                byte b = originalBytes[i];
                char ch = (char)b;
                if (IsUrlSafeChar(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }
        }

        #endregion

        #region UrlEncode public methods

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            int safeCount = 0;
            int spaceCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (IsUrlSafeChar(ch))
                {
                    safeCount++;
                }
                else if (ch == ' ')
                {
                    spaceCount++;
                }
            }

            int unexpandedCount = safeCount + spaceCount;
            if (unexpandedCount == value.Length)
            {
                if (spaceCount != 0)
                {
                    // Only spaces to encode
                    return value.Replace(' ', '+');
                }

                // Nothing to expand
                return value;
            }

            int byteCount = Encoding.UTF8.GetByteCount(value);
            int unsafeByteCount = byteCount - unexpandedCount;
            int byteIndex = unsafeByteCount * 2;

            // Instead of allocating one array of length `byteCount` to store
            // the UTF-8 encoded bytes, and then a second array of length 
            // `3 * byteCount - 2 * unexpandedCount`
            // to store the URL-encoded UTF-8 bytes, we allocate a single array of
            // the latter and encode the data in place, saving the first allocation.
            // We store the UTF-8 bytes to the end of this array, and then URL encode to the
            // beginning of the array.
            byte[] newBytes = new byte[byteCount + byteIndex];
            Encoding.UTF8.GetBytes(value, 0, value.Length, newBytes, byteIndex);

            GetEncodedBytes(newBytes, byteIndex, byteCount, newBytes);
            return Encoding.UTF8.GetString(newBytes);
        }

        public static byte[] UrlEncodeToBytes(byte[] value, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(value, offset, count))
            {
                return null;
            }

            bool foundSpaces = false;
            int unsafeCount = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)value[offset + i];

                if (ch == ' ')
                    foundSpaces = true;
                else if (!IsUrlSafeChar(ch))
                    unsafeCount++;
            }

            // nothing to expand?
            if (!foundSpaces && unsafeCount == 0)
            {
                var subarray = new byte[count];
                Buffer.BlockCopy(value, offset, subarray, 0, count);
                return subarray;
            }

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + unsafeCount * 2];
            GetEncodedBytes(value, offset, count, expandedBytes);
            return expandedBytes;
        }

        #endregion

        #region UrlDecode implementation

        private static string UrlDecodeInternal(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            int count = value.Length;
            UrlDecoder helper = new UrlDecoder(count, encoding);

            // go through the string's chars collapsing %XX and
            // appending each char as char, with exception of %XX constructs
            // that are appended as bytes
            bool needsDecodingUnsafe = false;
            bool needsDecodingSpaces = false;
            for (int pos = 0; pos < count; pos++)
            {
                char ch = value[pos];

                if (ch == '+')
                {
                    needsDecodingSpaces = true;
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    int h1 = HexToInt(value[pos + 1]);
                    int h2 = HexToInt(value[pos + 2]);

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        byte b = (byte)((h1 << 4) | h2);
                        pos += 2;

                        // don't add as char
                        helper.AddByte(b);
                        needsDecodingUnsafe = true;
                        continue;
                    }
                }

                if ((ch & 0xFF80) == 0)
                    helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                else
                    helper.AddChar(ch);
            }

            if (!needsDecodingUnsafe)
            {
                if (needsDecodingSpaces)
                {
                    // Only spaces to decode
                    return value.Replace('+', ' ');
                }

                // Nothing to decode
                return value;
            }

            return helper.GetString();
        }

        private static byte[] UrlDecodeInternal(byte[] bytes, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(bytes, offset, count))
            {
                return null;
            }

            int decodedBytesCount = 0;
            byte[] decodedBytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                int pos = offset + i;
                byte b = bytes[pos];

                if (b == '+')
                {
                    b = (byte)' ';
                }
                else if (b == '%' && i < count - 2)
                {
                    int h1 = HexToInt((char)bytes[pos + 1]);
                    int h2 = HexToInt((char)bytes[pos + 2]);

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        b = (byte)((h1 << 4) | h2);
                        i += 2;
                    }
                }

                decodedBytes[decodedBytesCount++] = b;
            }

            if (decodedBytesCount < decodedBytes.Length)
            {
                Array.Resize(ref decodedBytes, decodedBytesCount);
            }

            return decodedBytes;
        }

        #endregion

        #region UrlDecode public methods

        public static string UrlDecode(string encodedValue)
        {
            return UrlDecodeInternal(encodedValue, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(byte[] encodedValue, int offset, int count)
        {
            return UrlDecodeInternal(encodedValue, offset, count);
        }

        #endregion

        #region Helper methods
        
        private static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }

        private static char IntToHex(int n)
        {
            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'A');
        }

        private static bool IsUrlSafeChar(char ch)
        {
            // Set of safe chars, from RFC 1738.4 minus '+'
            /*
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
                return true;
            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }
            return false;
            */
            // Optimized version of the above:

            int code = (int)ch;

            const int safeSpecialCharMask = 0x03FF0000 | // 0..9
                1 << ((int)'!' - 0x20) | // 0x21
                1 << ((int)'(' - 0x20) | // 0x28
                1 << ((int)')' - 0x20) | // 0x29
                1 << ((int)'*' - 0x20) | // 0x2A
                1 << ((int)'-' - 0x20) | // 0x2D
                1 << ((int)'.' - 0x20); // 0x2E

            unchecked
            {
                return ((uint)(code - 'a') <= (uint)('z' - 'a')) ||
                       ((uint)(code - 'A') <= (uint)('Z' - 'A')) ||
                       ((uint)(code - 0x20) <= (uint)('9' - 0x20) && ((1 << (code - 0x20)) & safeSpecialCharMask) != 0) ||
                       (code == (int)'_');
            }
        }

        private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
        {
            if (bytes == null && count == 0)
                return false;
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0 || offset + count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return true;
        }

        #endregion

        // Internal struct to facilitate URL decoding -- keeps char buffer and byte buffer, allows appending of either chars or bytes
        private struct UrlDecoder
        {
            private int _bufferSize;

            // Accumulate characters in a special array
            private int _numChars;
            private char[] _charBuffer;

            // Accumulate bytes for decoding into characters in a special array
            private int _numBytes;
            private byte[] _byteBuffer;

            // Encoding to convert chars to bytes
            private Encoding _encoding;

            private void FlushBytes()
            {
                if (_charBuffer == null)
                    _charBuffer = new char[_bufferSize];

                _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
                _numBytes = 0;
            }

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                _bufferSize = bufferSize;
                _encoding = encoding;

                _charBuffer = null; // char buffer created on demand

                _numChars = 0;
                _numBytes = 0;
                _byteBuffer = null; // byte buffer created on demand
            }

            internal void AddChar(char ch)
            {
                if (_numBytes > 0)
                    FlushBytes();

                if (_charBuffer == null)
                    _charBuffer = new char[_bufferSize];

                _charBuffer[_numChars++] = ch;
            }

            internal void AddByte(byte b)
            {
                if (_byteBuffer == null)
                    _byteBuffer = new byte[_bufferSize];

                _byteBuffer[_numBytes++] = b;
            }

            internal String GetString()
            {
                if (_numBytes > 0)
                    FlushBytes();

                return new string(_charBuffer, 0, _numChars);
            }
        }
    }
}
