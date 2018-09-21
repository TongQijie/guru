using System;
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
                return Decompress(inputStream);
            }
        }

        public static byte[] Decompress(Stream inputStream)
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

        public static void Decompress(Stream inputStream, Action<byte[], int, int> handler, int bufferSize = 4096)
        {
            using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                var buffer = new byte[bufferSize];
                var count = 0;
                while ((count = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    handler(buffer, 0, count);
                }
            }
        }

        public static byte[] Compress(Stream inputStream)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    var buffer = new byte[1024 * 8];
                    var count = 0;
                    while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        gzipStream.Write(buffer, 0, count);
                    }
                }

                return compressedStream.ToArray();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    gzipStream.Write(data, 0, data.Length);
                }

                return compressedStream.ToArray();
            }
        }
    }
}