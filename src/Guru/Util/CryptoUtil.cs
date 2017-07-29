using System.Security.Cryptography;
using System.Text;

namespace Guru.Util
{
    public static class CryptoUtil
    {
        public static string Md5Crypto(string plainText)
        {
            using (var algorithm = MD5.Create())
            {
                return BytesToString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainText)));
            }
        }

        public static string Sha1Crypto(string plainText)
        {
            using (var algorithm = SHA1.Create())
            {
                return BytesToString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainText)));
            }
        }

        private static string BytesToString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes)
            {
                stringBuilder.Append("x2");
            }
            return stringBuilder.ToString();
        }
    }
}
