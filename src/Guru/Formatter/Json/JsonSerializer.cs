using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;

namespace Guru.Formatter.Json
{
    internal class JsonSerializer
    {
        public JsonSerializer(Type type)
        {
            Type = type;
            JsonObjectType = JsonUtility.GetJsonObjectType(Type);

            Initialize();
        }

        public Type Type { get; private set; }

        public JsonObjectType JsonObjectType { get; private set; }

        #region Initialization

        private ConcurrentDictionary<string, JsonProperty> _JsonProperties = null;

        private ConcurrentDictionary<string, JsonProperty> JsonProperties
        {
            get { return _JsonProperties ?? (_JsonProperties = new ConcurrentDictionary<string, JsonProperty>()); }
        }

        private object _InitLock = new object();

        private bool _IsInitialized = false;

        private void Initialize()
        {
            if (!_IsInitialized)
            {
                lock (_InitLock)
                {
                    if (!_IsInitialized)
                    {
                        var jsonObjectType = JsonUtility.GetJsonObjectType(Type);
                        
                        // only parse json dictionary object
                        if (jsonObjectType == JsonObjectType.Dictionary)
                        {
                            foreach (var propertyInfo in Type.GetTypeInfo().GetProperties().Where(x => x.CanRead && x.CanWrite && !x.IsDefined(typeof(JsonIngoreAttribute))))
                            {
                                JsonProperty jsonProperty = null;

                                var attribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
                                if (attribute == null)
                                {
                                    jsonProperty = new JsonProperty(propertyInfo, null, false);
                                }
                                else if (attribute is JsonObjectAttribute)
                                {
                                    jsonProperty = new JsonProperty(propertyInfo, attribute.Alias, true);
                                }
                                else
                                {
                                    jsonProperty = new JsonProperty(propertyInfo, attribute.Alias, false);
                                }

                                JsonProperties.TryAdd(jsonProperty.Key, jsonProperty);
                            }
                        }

                        _IsInitialized = true;
                    }
                }
            }
        }

        #endregion

        #region Serializer Cache

        private static ConcurrentDictionary<Type, JsonSerializer> _CachedSerializers = null;

        private static ConcurrentDictionary<Type, JsonSerializer> CachedSerializers
        {
            get { return _CachedSerializers ?? (_CachedSerializers = new ConcurrentDictionary<Type, JsonSerializer>()); }
        }

        public static JsonSerializer GetSerializer(Type targetType)
        {
            var objectType = JsonUtility.GetJsonObjectType(targetType);
            if (objectType == JsonObjectType.Runtime || objectType == JsonObjectType.Value)
            {
                return null;
            }

            if (!CachedSerializers.ContainsKey(targetType))
            {
                CachedSerializers.TryAdd(targetType, new JsonSerializer(targetType));
            }

            JsonSerializer serializer;
            if (CachedSerializers.TryGetValue(targetType, out serializer))
            {
                return serializer;
            }

            return null;
        }

        #endregion

        #region Serialization

        public void Serialize(object instance, Stream stream, bool omitDefaultValue)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (JsonObjectType == JsonObjectType.Dictionary)
            {
                SerializeJsonDictionaryObject(stream, instance, omitDefaultValue);
            }
            else if (JsonObjectType == JsonObjectType.Collection)
            {
                SerializeJsonCollectionObject(stream, instance, omitDefaultValue);
            }
            else
            {
                throw new Exception(string.Format("failed to serialize object '{0}'.", instance));
            }
        }

        private void SerializeJsonDictionaryObject(Stream stream, object value, bool omitDefaultValue)
        {
            stream.WriteByte(JsonEncoder.Left_Brace);

            var firstElement = true;
            foreach (var jsonProperty in JsonProperties.Values.ToArray())
            {
                var propertyValue = jsonProperty.PropertyInfo.GetValue(value, null);
                if (omitDefaultValue && jsonProperty.DefaultValue.EqualsWith(propertyValue))
                {
                    continue;
                }

                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    stream.WriteByte(JsonEncoder.Comma);
                }

                var buf = JsonEncoder.GetElementName(jsonProperty.Key);
                stream.Write(buf, 0, buf.Length);

                SerializeRegularValue(stream, propertyValue, jsonProperty.ObjectType, omitDefaultValue);
            }

            stream.WriteByte(JsonEncoder.Right_Brace);
        }

        private void SerializeJsonCollectionObject(Stream stream, object value, bool omitDefaultValue)
        {
            stream.WriteByte(JsonEncoder.Left_Bracket);

            var collection = value as ICollection;
            var objectType = JsonUtility.GetJsonObjectType(value.GetType().GetElementType());

            var firstElement = true;
            foreach (var element in collection)
            {
                if (element == null)
                {
                    continue;
                }

                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    stream.WriteByte(JsonEncoder.Comma);
                }

                SerializeRegularValue(stream, element, objectType, omitDefaultValue);
            }

            stream.WriteByte(JsonEncoder.Right_Bracket);
        }

        private void SerializeRegularValue(Stream stream, object value, JsonObjectType objectType, bool omitDefaultValue)
        {
            if (objectType == JsonObjectType.Value)
            {
                var buf = JsonEncoder.GetPlainValue(value);
                stream.Write(buf, 0, buf.Length);
            }
            else if (value == null)
            {
                var buf = JsonEncoder.GetNullValue();
                stream.Write(buf, 0, buf.Length);
            }
            else if (objectType == JsonObjectType.Runtime)
            {
                SerializeRegularValue(stream, value, JsonUtility.GetJsonObjectType(value.GetType()), omitDefaultValue);
            }
            else
            {
                GetSerializer(value.GetType()).Serialize(value, stream, omitDefaultValue);
            }
        }

        #endregion

        #region Deserialization

        public object Deserialize(Stream stream)
        {
            var args = new JsonObjectParseArgs()
            {
                Stream = new Internal.BufferedStream(stream, 8 * 1024),
            };

            JsonObjectParser.Parse(args);

            return InternalDeserialize(args.InternalObject);
        }

        public async Task<object> DeserializeAsync(Stream stream)
        {
            var args = new JsonObjectParseArgs()
            {
                Stream = new Internal.BufferedStream(stream, 8 * 1024),
            };

            await JsonObjectParser.ParseAsync(args);

            return InternalDeserialize(args.InternalObject);
        }

        public object Deserialize(JsonObject jsonObject)
        {
            return InternalDeserialize(jsonObject);
        }

        private object InternalDeserialize(JsonObject jsonObject)
        {
            if (jsonObject is JsonDictionaryObject)
            {
                return DeserializeJsonDictionaryObject(jsonObject as JsonDictionaryObject);
            }
            else if (jsonObject is JsonCollectionObject)
            {
                return DeserializeJsonCollectionObject(jsonObject as JsonCollectionObject, Type);
            }
            else if (jsonObject is JsonValueObject)
            {
                return DeserializeJsonPlainValueObject(jsonObject as JsonValueObject, Type);
            }

            return null;
        }

        private object DeserializeJsonDictionaryObject(JsonDictionaryObject dictionaryObject)
        {
            var instance = Activator.CreateInstance(Type);

            foreach (var element in dictionaryObject.Elements)
            {
                JsonProperty jsonProperty;
                JsonProperties.TryGetValue(element.Key, out jsonProperty);
                if (jsonProperty == null)
                {
                    continue;
                }

                if (jsonProperty.IsJsonObject)
                {
                    jsonProperty.PropertyInfo.SetValue(instance, element.Value, null);
                    continue;
                }

                if (jsonProperty.ObjectType == JsonObjectType.Runtime)
                {
                    continue;
                }

                if (jsonProperty.ObjectType == JsonObjectType.Dictionary)
                {
                    if (element.Value is JsonDictionaryObject)
                    {
                        var value = GetSerializer(jsonProperty.PropertyInfo.PropertyType).InternalDeserialize(element.Value);
                        jsonProperty.PropertyInfo.SetValue(instance, value, null);
                    }
                    else if (element.Value is JsonValueObject && JsonEncoder.IsNullValue(element.Value))
                    {
                        jsonProperty.PropertyInfo.SetValue(instance, null, null);
                    }
                    else
                    {
                        throw new Errors.JsonSerializeFailedException(element.Key, ".net runtime type does not match json type.");
                    }
                }
                else if (jsonProperty.ObjectType == JsonObjectType.Collection)
                {
                    if (element.Value is JsonCollectionObject)
                    {
                        var collectionObject = element.Value as JsonCollectionObject;
                        jsonProperty.PropertyInfo.SetValue(instance,
                            DeserializeJsonCollectionObject(collectionObject, jsonProperty.PropertyInfo.PropertyType), null);
                    }
                    else if (element.Value is JsonValueObject && JsonEncoder.IsNullValue(element.Value))
                    {
                        jsonProperty.PropertyInfo.SetValue(instance, null, null);
                    }
                    else
                    {
                        throw new Errors.JsonSerializeFailedException(element.Key, ".net runtime type does not match json type.");
                    }
                }
                else if (jsonProperty.ObjectType == JsonObjectType.Value && element.Value is JsonValueObject)
                {
                    var value = DeserializeJsonPlainValueObject(element.Value as JsonValueObject, jsonProperty.PropertyInfo.PropertyType);
                    jsonProperty.PropertyInfo.SetValue(instance, value, null);
                }
                else
                {
                    throw new Errors.JsonSerializeFailedException(element.Key, ".net runtime type does not match json type.");
                }
            }

            return instance;
        }

        private object DeserializeJsonCollectionObject(JsonCollectionObject collectionObject, Type targetType)
        {
            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                var array = Array.CreateInstance(elementType, collectionObject.Elements.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    var jsonObject = collectionObject.Elements[i].Value;
                    if (jsonObject is JsonDictionaryObject)
                    {
                        var value = GetSerializer(elementType).InternalDeserialize(jsonObject);
                        array.SetValue(value, i);
                    }
                    else if (jsonObject is JsonCollectionObject)
                    {
                        // TODO: multi-dimension array
                    }
                    else if (jsonObject is JsonValueObject)
                    {
                        var value = DeserializeJsonPlainValueObject(jsonObject as JsonValueObject, elementType);
                        array.SetValue(value, i);
                    }
                }

                return array;
            }
            else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
            {
                var elementType = targetType.GetTypeInfo().GetGenericArguments().FirstOrDefault() ?? typeof(object);
                var collection = Activator.CreateInstance(targetType) as IList;
                for (int i = 0; i < collectionObject.Elements.Length; i++)
                {
                    var jsonObject = collectionObject.Elements[i].Value;
                    if (jsonObject is JsonDictionaryObject)
                    {
                        var value = GetSerializer(elementType).InternalDeserialize(jsonObject);
                        collection.Add(value);
                    }
                    else if (jsonObject is JsonCollectionObject)
                    {
                        // TODO: multi-dimension array
                    }
                    else if (jsonObject is JsonValueObject)
                    {
                        var value = DeserializeJsonPlainValueObject(jsonObject as JsonValueObject, elementType);
                        collection.Add(value);
                    }
                }

                return collection;
            }

            return null;
        }

        private object DeserializeJsonPlainValueObject(JsonValueObject plainValueObject, Type targetType)
        {
            if (JsonEncoder.IsNullValue(plainValueObject))
            {
                return null;
            }
            else
            {
                return plainValueObject.ToString().ConvertTo(targetType);
            }
        }

        #endregion
    }
}