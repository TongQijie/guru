using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Guru.Formatter.Abstractions
{
    public interface IFormatter
    {
        object ReadObject(Type targetType, Stream stream);

        Task<object> ReadObjectAsync(Type targetType, Stream stream);

        object ReadObject(Type targetType, string path);

        Task<object> ReadObjectAsync(Type targetType, string path);

        object ReadObject(Type targetType, string stringValue, Encoding encoding);

        Task<object> ReadObjectAsync(Type targetType, string stringValue, Encoding encoding);

        object ReadObject(Type targetType, byte[] byteValues, int offset, int count);

        Task<object> ReadObjectAsync(Type targetType, byte[] byteValues, int offset, int count);

        T ReadObject<T>(Stream stream);

        Task<T> ReadObjectAsync<T>(Stream stream);

        T ReadObject<T>(string path);

        Task<T> ReadObjectAsync<T>(string path);

        T ReadObject<T>(string stringValue, Encoding encoding);

        Task<T> ReadObjectAsync<T>(string stringValue, Encoding encoding);

        T ReadObject<T>(byte[] byteValues, int offset, int count);

        Task<T> ReadObjectAsync<T>(byte[] byteValues, int offset, int count);

        void WriteObject(object instance, Stream stream);

        void WriteObject(object instance, string path);

        string WriteString(object instance, Encoding encoding);

        byte[] WriteBytes(object instance);
    }
}