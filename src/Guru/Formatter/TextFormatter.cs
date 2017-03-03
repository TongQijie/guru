using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Formatter
{
    [DI(typeof(ITextFormatter), Lifetime = Lifetime.Singleton)]
    public class TextFormatter : ITextFormatter
    {
        public Encoding TextEncoding { get; set; }

        public TextFormatter()
        {
            TextEncoding = Encoding.UTF8;
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
            using(var reader = new StreamReader(stream, Encoding.UTF8))
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
            using(var reader = new StreamReader(stream, Encoding.UTF8))
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
                using (var writer = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    writer.Write(instance.ToString());
                }
            }
        }

        public void WriteObject(object instance, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(instance.ToString());
            }
        }

        public string WriteString(object instance, Encoding encoding)
        {
            return instance.ToString();
        }
    }
}