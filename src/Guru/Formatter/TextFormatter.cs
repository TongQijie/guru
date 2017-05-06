using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(ITextFormatter), Lifetime.Transient)]
    public class TextFormatter : ITextFormatter
    {
        public Encoding DefaultEncoding { get; set; }

        public TextFormatter()
        {
            DefaultEncoding = Encoding.UTF8;
        }

        public object ReadObject(Type targetType, string path)
        {
            using (var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ReadObject(targetType, inputStream);
            }
        }

        public object ReadObject(Type targetType, Stream stream)
        {
            using(var reader = new StreamReader(stream, DefaultEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        public object ReadObject(Type targetType, string stringValue, Encoding encoding)
        {
            return stringValue;
        }

        public object ReadObject(Type targetType, byte[] byteValues, int offset, int count)
        {
            return Encoding.UTF8.GetString(byteValues, offset, count);
        }

        public T ReadObject<T>(string path)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)ReadObject(typeof(T), path);
        }

        public T ReadObject<T>(Stream stream)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)ReadObject(typeof(T), stream);
        }

        public T ReadObject<T>(string stringValue, Encoding encoding)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)ReadObject(typeof(T), stringValue, encoding);
        }

        public T ReadObject<T>(byte[] byteValues, int offset, int count)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)ReadObject(typeof(T), byteValues, offset, count);
        }

        public async Task<object> ReadObjectAsync(Type targetType, string path)
        {
             using (var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return await ReadObjectAsync(targetType, inputStream);
            }
        }

        public async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            using(var reader = new StreamReader(stream, DefaultEncoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public Task<object> ReadObjectAsync(Type targetType, string stringValue, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public Task<object> ReadObjectAsync(Type targetType, byte[] byteValues, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public async Task<T> ReadObjectAsync<T>(string path)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)(await ReadObjectAsync(typeof(T), path));
        }

        public async Task<T> ReadObjectAsync<T>(Stream stream)
        {
            if (typeof(T) != typeof(string))
            {
                throw new Exception("text formatter only support string type.");
            }

            return (T)(await ReadObjectAsync(typeof(T), stream));
        }

        public Task<T> ReadObjectAsync<T>(string stringValue, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public Task<T> ReadObjectAsync<T>(byte[] byteValues, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public byte[] WriteBytes(object instance)
        {
            return Encoding.UTF8.GetBytes(instance.ToString());
        }

        public void WriteObject(object instance, string path)
        {
            using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(outputStream, DefaultEncoding))
                {
                    writer.Write(instance.ToString());
                }
            }
        }

        public void WriteObject(object instance, Stream stream)
        {
            using (var writer = new StreamWriter(stream, DefaultEncoding))
            {
                writer.Write(instance.ToString());
            }
        }

        public string WriteString(object instance, Encoding encoding)
        {
            return instance.ToString();
        }

        public async Task WriteObjectAsync(object instance, Stream stream)
        {
            using (var writer = new StreamWriter(stream, DefaultEncoding))
            {
                await writer.WriteAsync(instance.ToString());
            }
        }

        public async Task WriteObjectAsync(object instance, string path)
        {
            using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(outputStream, DefaultEncoding))
                {
                    await writer.WriteAsync(instance.ToString());
                }
            }
        }

        public Task<string> WriteStringAsync(object instance, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> WriteBytesAsync(object instance)
        {
            throw new NotImplementedException();
        }
    }
}