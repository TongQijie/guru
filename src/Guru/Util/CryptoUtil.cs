using System.Security.Cryptography;
using System.Text;

namespace Guru.Util
{
    internal static class CryptoUtil
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

        private static string BytesToString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
