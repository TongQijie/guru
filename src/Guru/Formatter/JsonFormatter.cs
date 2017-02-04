using System;
using System.IO;
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

        public JsonFormatter()
        {
            OmitDefaultValue = true;
        }

        public override object ReadObject(Type targetType, Stream stream)
        {
            return JsonSerializer.GetSerializer(targetType).Deserialize(stream);
        }

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await JsonSerializer.GetSerializer(targetType).DeserializeAsync(stream);
        }

        public object ReadObject(Type targetType, JsonObject jsonObject)
        {
            return JsonSerializer.GetSerializer(targetType).Deserialize(jsonObject);
        }

        public T ReadObject<T>(JsonObject jsonObject)
        {
            return (T)ReadObject(typeof(T), jsonObject);
        }

        public override void WriteObject(object instance, Stream stream)
        {
            JsonSerializer.GetSerializer(instance.GetType()).Serialize(instance, stream, OmitDefaultValue);
        }
    }
}