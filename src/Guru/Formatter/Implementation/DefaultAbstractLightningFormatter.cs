using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;

namespace Guru.Formatter.Implementation
{
    internal abstract class DefaultAbstractLightningFormatter : ILightningFormatter
    {
        public virtual string Tag => "NA";

        public object ReadObject(Type targetType, Stream stream)
        {
            return ReadObjectAsync(targetType, stream).GetAwaiter().GetResult();
        }

        public object ReadObject(Type targetType, string stringValue)
        {
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringValue)))
            {
                return ReadObject(targetType, inputStream);
            }
        }

        public T ReadObject<T>(Stream stream)
        {
            return ReadObject(typeof(T), stream).ConvertTo<T>();
        }

        public T ReadObject<T>(string stringValue)
        {
            return ReadObject(typeof(T), stringValue).ConvertTo<T>();
        }

        public abstract Task<object> ReadObjectAsync(Type targetType, Stream stream);

        public async Task<object> ReadObjectAsync(Type targetType, string stringValue)
        {
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringValue)))
            {
                return await ReadObjectAsync(targetType, inputStream);
            }
        }

        public async Task<T> ReadObjectAsync<T>(Stream stream)
        {
            return (await ReadObjectAsync(typeof(T), stream)).ConvertTo<T>();
        }

        public async Task<T> ReadObjectAsync<T>(string stringValue)
        {
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringValue)))
            {
                return await ReadObjectAsync<T>(inputStream);
            }
        }

        public void WriteObject(object instance, Stream stream)
        {
            WriteObjectAsync(instance, stream).GetAwaiter().GetResult();
        }

        public string WriteObject(object instance)
        {
            using (var outputStream = new MemoryStream())
            {
                WriteObject(instance, outputStream);
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }

        public abstract Task WriteObjectAsync(object instance, Stream stream);

        public async Task<string> WriteObjectAsync(object instance)
        {
            using (var outputStream = new MemoryStream())
            {
                await WriteObjectAsync(instance, outputStream);
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }
    }
}