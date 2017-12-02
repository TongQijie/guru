﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Guru.Formatter.Json;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.Formatter
{
    [Injectable(typeof(IJsonFormatter), Lifetime.Transient)]
    public class JsonFormatter : FormatterBase, IJsonFormatter
    {
        public bool OmitDefaultValue { get; set; }

        public Encoding DefaultEncoding { get; set; }

        public JsonFormatter()
        {
            DefaultEncoding = Encoding.UTF8;
        }

        public override object ReadObject(Type targetType, Stream stream)
        {
            return JsonSerializer.GetSerializer(targetType, DefaultEncoding, OmitDefaultValue, "").Deserialize(stream);
        }

        public override async Task<object> ReadObjectAsync(Type targetType, Stream stream)
        {
            return await JsonSerializer.GetSerializer(targetType, DefaultEncoding, OmitDefaultValue, "").DeserializeAsync(stream);
        }

        public object ReadObject(Type targetType, JBase jsonObject)
        {
            return JsonSerializer.GetSerializer(targetType, DefaultEncoding, OmitDefaultValue, "").Deserialize(jsonObject);
        }

        public T ReadObject<T>(JBase jsonObject)
        {
            return (T)ReadObject(typeof(T), jsonObject);
        }

        public override void WriteObject(object instance, Stream stream)
        {
            JsonSerializer.GetSerializer(instance.GetType(), DefaultEncoding, OmitDefaultValue, "").Serialize(instance, stream);
        }

        public override async Task WriteObjectAsync(object instance, Stream stream)
        {
            await JsonSerializer.GetSerializer(instance.GetType(), DefaultEncoding, OmitDefaultValue, "").SerializeAsync(instance, stream);
        }
    }
}