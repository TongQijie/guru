using System;
using System.Threading.Tasks;

namespace Guru.Formatter.Internal
{
    internal interface IReaderStream
    {
        /// <summary>
        /// number of bytes that have read.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// reads a byte
        /// </summary>
        /// <returns>if -1, indicate it has no bytes in buffer.</returns>
        int ReadByte();

        Task<int> ReadByteAsync();

        /// <summary>
        /// reads the specified number of bytes
        /// </summary>
        /// <param name="count">the specified number</param>
        /// <returns>if null, indicate buffer does not have specified number of bytes to read.</returns>
        byte[] ReadBytes(int count);

        Task<byte[]> ReadBytesAsync(int count);

        /// <summary>
        /// reads bytes until reading specified terminal byte.
        /// </summary>
        /// <param name="terminator">terminal byte</param>
        /// <returns>byte array that does not contain terminal byte. if null, indicate buffer cannot find terminal byte.</returns>
        byte[] ReadBytesUntil(byte terminator);

        Task<byte[]> ReadBytesUntilAsync(byte terminator);

        /// <summary>
        /// reads bytes until reading one of specified terminal bytes.
        /// </summary>
        /// <param name="terminators">terminal bytes</param>
        /// <returns>byte array that contain terminal byte. if null, indicate buffer cannot find terminal byte.</returns>
        byte[] ReadBytesUntil(byte[] terminators);

        Task<byte[]> ReadBytesUntilAsync(byte[] terminators);

        /// <summary>
        /// seeks to byte that is equal to target byte.
        /// </summary>
        /// <param name="targetByte">target byte</param>
        /// <returns>if false, indicate target byte does not exist in buffer, else true.</returns>
        bool SeekBytesUntilEqual(byte targetByte);

        Task<bool> SeekBytesUntilEqualAsync(byte targetByte);

        /// <summary>
        /// seeks to byte that is visiable excluding whitespace.
        /// </summary>
        /// <returns>return byte that is visiable excluding whitespace. if -1, indicate buffer have no bytes that is not equal to target byte.</returns>
        int SeekBytesUntilVisiableChar();

        Task<int> SeekBytesUntilVisiableCharAsync();

        int SeekBytesUntilText();

        Task<int> SeekBytesUntilTextAsync();

        byte[] ReadBytesUntilMeeting(Predicate<byte> predicate);

        Task<byte[]> ReadBytesUntilMeetingAsync(Predicate<byte> predicate);
    }
}

