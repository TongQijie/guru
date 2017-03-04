using System.Threading.Tasks;

namespace Guru.Formatter.Internal
{
    internal interface IWriterStream
    {
        Task WriteAsync(byte b);

        Task WriteAsync(byte[] buffer);

        Task WriteAsync(byte[] buffer, int offset, int count);

        Task EndWrite();
    }
}