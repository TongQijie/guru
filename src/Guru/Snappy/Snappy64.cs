using System.Runtime.InteropServices;

namespace Guru.Snappy
{
    /// <summary>P/Invoke wrapper for x86 assembly</summary>
    internal static class Snappy64
    {
        [DllImport("Snappy/SnappyDL.x64.dll", EntryPoint = "snappy_compress")]
        public static extern unsafe SnappyStatus Compress(
            byte* input,
            long inputLength,
            byte* compressed,
            ref long compressedLength);

        [DllImport("Snappy/SnappyDL.x64.dll", EntryPoint = "snappy_uncompress")]
        public static extern unsafe SnappyStatus Uncompress(
            byte* compressed,
            long compressedLength,
            byte* uncompressed,
            ref long uncompressedLength);

        [DllImport("Snappy/SnappyDL.x64.dll", EntryPoint = "snappy_max_compressed_length")]
        public static extern long GetMaximumCompressedLength(
            long inputLength);

        [DllImport("Snappy/SnappyDL.x64.dll", EntryPoint = "snappy_uncompressed_length")]
        public static extern unsafe SnappyStatus GetUncompressedLength(
            byte* compressed,
            long compressedLength,
            ref long result);

        [DllImport("Snappy/SnappyDL.x64.dll", EntryPoint = "snappy_validate_compressed_buffer")]
        public static extern unsafe SnappyStatus ValidateCompressedBuffer(
            byte* compressed,
            long compressedLength);
    }
}