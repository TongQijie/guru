using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Guru.Formatter.Json;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Formatter
{
    [DI(typeof(IJsonFormatter), Lifetime = Lifetime.Singleton)]
    public class JsonFormatter : FormatterBase, IJsonFormatter
    {
        public bool OmitDefaultValue { get; set; }

        public Encoding DefaultEncoding { get; set; }

        public JsonFormatter()
        {
            OmitDefaultValue = true;
            DefaultEncoding = Encoding.UTF8;
        }

        public override object ReadObject(Type targetType, Stream stream)
        {
            return JsonSerializer.GetSerializer(targetType, DefaultEncoding).Deserialize(stream);
        }

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await JsonSerializer.GetSerializer(targetType, DefaultEncoding).DeserializeAsync(stream);
        }

        public object ReadObject(Type targetType, JsonObject jsonObject)
        {
            return JsonSerializer.GetSerializer(targetType, DefaultEncoding).Deserialize(jsonObject);
        }

        public T ReadObject<T>(JsonObject jsonObject)
        {
            return (T)ReadObject(typeof(T), jsonObject);
        }

        public override void WriteObject(object instance, Stream stream)
        {
            JsonSerializer.GetSerializer(instance.GetType(), DefaultEncoding).Serialize(instance, stream, OmitDefaultValue);
        }
    }
}