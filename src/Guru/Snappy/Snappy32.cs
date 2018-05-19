using System.Runtime.InteropServices;

namespace Guru.Snappy
{
    /// <summary>P/Invoke wrapper for x86 assembly</summary>
	internal static class Snappy32
    {
        [DllImport("Snappy/SnappyDL.x86.dll", EntryPoint = "snappy_compress")]
        public static extern unsafe SnappyStatus Compress(
            byte* input,
            int inputLength,
            byte* compressed,
            ref int compressedLength);

        [DllImport("Snappy/SnappyDL.x86.dll", EntryPoint = "snappy_uncompress")]
        public static extern unsafe SnappyStatus Uncompress(
            byte* compressed,
            int compressedLength,
            byte* uncompressed,
            ref int uncompressedLength);

        [DllImport("Snappy/SnappyDL.x86.dll", EntryPoint = "snappy_max_compressed_length")]
        public static extern int GetMaximumCompressedLength(
            int inputLength);

        [DllImport("Snappy/SnappyDL.x86.dll", EntryPoint = "snappy_uncompressed_length")]
        public static extern unsafe SnappyStatus GetUncompressedLength(
            byte* compressed,
            int compressedLength,
            ref int result);

        [DllImport("Snappy/SnappyDL.x86.dll", EntryPoint = "snappy_validate_compressed_buffer")]
        public static extern unsafe SnappyStatus ValidateCompressedBuffer(
            byte* compressed,
            int compressedLength);
    }
}
