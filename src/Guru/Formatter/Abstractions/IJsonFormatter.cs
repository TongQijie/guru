using System;
using System.Text;

using Guru.Formatter.Json;

namespace Guru.Formatter.Abstractions
{
    public interface IJsonFormatter : IFormatter
    {
        bool OmitDefaultValue { get; set; }

        Encoding DefaultEncoding { get; set; }

        object ReadObject(Type targetType, JsonObject jsonObject);

        T ReadObject<T>(JsonObject jsonObject);
    }
}