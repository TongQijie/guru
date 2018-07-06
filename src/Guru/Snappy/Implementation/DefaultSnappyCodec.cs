using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Snappy.Abstractions;
using System;

namespace Guru.Snappy.Implementation
{
    [Injectable(typeof(ISnappyCodec), Lifetime.Singleton)]
    internal class DefaultSnappyCodec : ISnappyCodec
    {
        public DefaultSnappyCodec()
        {
            if (IntPtr.Size == 4)
            {
                // 32bit dll
                if (!"./Snappy/SnappyDL.x86.dll".IsFile())
                {
                    Console.WriteLine("dll cannot found: " + "./Snappy/SnappyDL.x86.dll".FullPath());
                }
            }
            else
            {
                // 64bit dll
                if (!"./Snappy/SnappyDL.x64.dll".IsFile())
                {
                    Console.WriteLine("dll cannot found: " + "./Snappy/SnappyDL.x64.dll".FullPath());
                }
            }
        }

        public byte[] Compress(byte[] input, int inputOffset, int inputLength)
        {
            return SnappyCodec.Compress(input, inputOffset, inputLength);
        }

        public byte[] Uncompress(byte[] compressed, int compressedOffset, int compressedLength, bool ignoreError = false)
        {
            return SnappyCodec.Uncompress(compressed, compressedOffset, compressedLength, ignoreError);
        }
    }
}