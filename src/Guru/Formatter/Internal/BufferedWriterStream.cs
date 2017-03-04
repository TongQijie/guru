using System;
using System.IO;
using System.Threading.Tasks;

namespace Guru.Formatter.Internal
{
    internal class BufferedWriterStream : IWriterStream
    {
        public BufferedWriterStream(Stream stream, int capacity)
        {
            _InternalStream = stream;
            _InternalBuffer = new byte[capacity];
        }

        private Stream _InternalStream = null;

        private byte[] _InternalBuffer = null;

        private int _Index = 0;

        public async Task WriteAsync(byte[] buffer)
        {
            await WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task WriteAsync(byte b)
        {
            if (1 + _Index > _InternalBuffer.Length)
            {
                await Flush();
                _InternalBuffer[0] = b;
            }
            else
            {
                _InternalBuffer[_Index] = b;
            }

            _Index++;
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            if (count + _Index > _InternalBuffer.Length)
            {
                var copied = _InternalBuffer.Length - _Index;
                Buffer.BlockCopy(buffer, offset, _InternalBuffer, _Index, copied);

                await Flush();
                await WriteAsync(buffer, offset + copied, count - copied);
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, _InternalBuffer, _Index, count);
                _Index += count;
            }
        }

        private async Task Flush()
        {
            await _InternalStream.WriteAsync(_InternalBuffer, 0, _Index);
            _Index = 0;
        }

        public async Task EndWrite()
        {
            await Flush();
        }
    }
}