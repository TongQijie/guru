using Guru.ExtensionMethod;
using System;
using System.Collections;
using System.Reflection;

namespace Guru.Formatter.Json
{
    public static class JParser
    {
        public static JType WhichJType(Type targetType)
        {
            if (targetType == typeof(object))
            {
                return JType.Unknown;
            }
            else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(targetType))
            {
                return JType.Map;
            }
            else if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(targetType))
            {
                return JType.Array;
            }
            else if (targetType.GetTypeInfo().IsClass && targetType != typeof(string))
            {
                return JType.Object;
            }
            else
            {
                return JType.Value;
            }
        }

        public static JElement Convert(Type targetType, string key, JElement parent)
        {
            var t = WhichJType(targetType);

            switch (t)
            {
                case JType.Unknown:
                    {
                        return new JElement() { Key = key, Parent = parent, InternalType = targetType };
                    }
                case JType.Value:
                    {
                        return new JValue() { Key = key, Parent = parent, InternalType = targetType };
                    }
                case JType.Map:
                    {
                        return new JObject() { Key = key, Parent = parent, InternalType = targetType };
                    }
                case JType.Array:
                    {
                        return new JArray() { Key = key, Parent = parent, InternalType = targetType };
                    }
                case JType.Object:
                    {
                        var obj = new JObject() { Key = key, Parent = parent, InternalType = targetType };

                        foreach (var propertyInfo in targetType.GetTypeInfo().GetProperties().Subset(x => x.CanRead && x.CanWrite && !x.IsDefined(typeof(JsonIgnoreAttribute))))
                        {
                            var attribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
                            if (attribute == null)
                            {
                                obj.Children = obj.Children.Append(Convert(propertyInfo.PropertyType, propertyInfo.Name, obj));
                            }
                            else
                            {
                                obj.Children = obj.Children.Append(Convert(propertyInfo.PropertyType, attribute.Alias.HasValue() ? attribute.Alias : propertyInfo.Name, obj));
                            }
                        }

                        return obj;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
        }
    }
}
