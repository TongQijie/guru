using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;

namespace Guru.Utils
{
    public static class CryptoUtils
    {
        public static string Md5(string plainText)
        {
            using (var algorithm = MD5.Create())
            {
                return BytesToString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainText)));
            }
        }

        public static string Sha1(string plainText)
        {
            using (var algorithm = SHA1.Create())
            {
                return BytesToString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainText)));
            }
        }

        public static byte[] RsaX509(byte[] data, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                var key = Md5(Convert.ToBase64String(publicKey));
                if (CachedRsaParameters.ContainsKey(key))
                {
                    rsa.ImportParameters(CachedRsaParameters[key]);
                }
                else
                {
                    var rsaParameters = CreateRsaParametersFromX509(publicKey);
                    CachedRsaParameters.TryAdd(key, rsaParameters);
                    rsa.ImportParameters(rsaParameters);
                }

                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        private static string BytesToString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        private static ConcurrentDictionary<string, RSAParameters> CachedRsaParameters = new ConcurrentDictionary<string, RSAParameters>();

        #region RFC 2459: Internet X.509 Public Key Infrastructure Certificate and CRL Profile

        private static RSAParameters CreateRsaParametersFromX509(byte[] publicKey)
        {
            using (var memoryStream = new MemoryStream(publicKey))
            {
                using (var reader = new BinaryReader(memoryStream))
                {
                    SeekX509x30(reader);

                    reader.ReadBytes(15);

                    SeekX509x03(reader);

                    reader.ReadByte();

                    SeekX509x30(reader);

                    var modulusSize = GetModulusSize(reader);
                    var modulus = reader.ReadBytes(modulusSize);

                    reader.ReadByte();

                    var exponentSize = (int)reader.ReadByte();
                    var exponent = reader.ReadBytes(exponentSize);

                    return new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                }
            }
        }

        private static void SeekX509x30(BinaryReader reader)
        {
            var integer = reader.ReadUInt16();
            if (integer == 0x8130)
            {
                reader.ReadByte();
            }
            else if (integer == 0x8230)
            {
                reader.ReadInt16();
            }
        }

        private static void SeekX509x03(BinaryReader reader)
        {
            var integer = reader.ReadUInt16();
            if (integer == 0x8103)
            {
                reader.ReadByte();
            }
            else if (integer == 0x8203)
            {
                reader.ReadInt16();
            }
        }

        private static int GetModulusSize(BinaryReader reader)
        {
            var integer = reader.ReadUInt16();
            var size = 0;
            if (integer == 0x8102)
            {
                size = reader.ReadByte();
            }
            else if (integer == 0x8202)
            {
                size += reader.ReadByte() << 8;
                size += reader.ReadByte();
            }
            if (reader.PeekChar() == 0x00)
            {
                reader.ReadByte();
                size -= 1;
            }
            return size;
        }

        #endregion
    }
}