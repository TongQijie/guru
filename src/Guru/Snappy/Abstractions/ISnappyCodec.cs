namespace Guru.Snappy.Abstractions
{
    public interface ISnappyCodec
    {
        byte[] Compress(byte[] input, int inputOffset, int inputLength);

        byte[] Uncompress(byte[] compressed, int compressedOffset, int compressedLength, bool ignoreError = false);
    }
}