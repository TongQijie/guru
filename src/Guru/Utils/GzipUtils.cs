using System.IO;
using System.IO.Compression;

namespace Guru.Utils
{
    public static class GzipUtils
    {
        public static byte[] Decompress(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        var buffer = new byte[1024 * 8];
                        var count = 0;
                        while ((count = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            decompressedStream.Write(buffer, 0, count);
                        }
                    }

                    return decompressedStream.ToArray();
                }
            }
        }
    }
}
