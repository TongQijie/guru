using System;

using Guru.Formatter.Json;

namespace Guru.Formatter.Abstractions
{
    public interface IJsonFormatter : IFormatter
    {
        bool OmitDefaultValue { get; set; }

        object ReadObject(Type targetType, JsonObject jsonObject);

        T ReadObject<T>(JsonObject jsonObject);
    }
}