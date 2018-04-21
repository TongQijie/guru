using System;
using System.IO;
using System.Threading.Tasks;

namespace Guru.Formatter.Abstractions
{
    // encoding: utf-8
    // read by bytes(stream) asynchronously
    // write to bytes(stream) asynchronously
    public interface ILightningFormatter
    {
        string Tag { get; }

        object ReadObject(Type targetType, Stream stream);

        Task<object> ReadObjectAsync(Type targetType, Stream stream);

        object ReadObject(Type targetType, string stringValue);

        Task<object> ReadObjectAsync(Type targetType, string stringValue);

        T ReadObject<T>(Stream stream);

        Task<T> ReadObjectAsync<T>(Stream stream);

        T ReadObject<T>(string stringValue);

        Task<T> ReadObjectAsync<T>(string stringValue);

        void WriteObject(object instance, Stream stream);

        Task WriteObjectAsync(object instance, Stream stream);

        string WriteObject(object instance);

        Task<string> WriteObjectAsync(object instance);
    }
}