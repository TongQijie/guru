using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    internal class JsonSerializer
    {
        public JsonSerializer(Type type, Encoding encoding, bool omitDefaultValue)
        {
            Type = type;
            JsonSettings = new JsonSettings(encoding, omitDefaultValue);

            JsonObjectType = JsonUtility.GetJsonObjectType(Type);
            Initialize();
        }

        public Type Type { get; private set; }

        public JsonSettings JsonSettings { get; private set; }

        public JType JsonObjectType { get; private set; }

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
                        if (jsonObjectType == JType.Object)
                        {
                            foreach (var propertyInfo in Type.GetTypeInfo().GetProperties().Where(x => x.CanRead && x.CanWrite && !x.IsDefined(typeof(JsonIgnoreAttribute))))
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

        private static ConcurrentDictionary<string, JsonSerializer> _Caches = null;

        private static ConcurrentDictionary<string, JsonSerializer> Caches
        {
            get { return _Caches ?? (_Caches = new ConcurrentDictionary<string, JsonSerializer>()); }
        }

        private static string GetSerializerKey(Type type, Encoding encoding, bool omitDefaultValue)
        {
            return $"{type.FullName};{encoding.EncodingName};{omitDefaultValue.ToString()}";
        }

        public static JsonSerializer GetSerializer(Type targetType, Encoding encoding, bool omitDefaultValue)
        {
            var objectType = JsonUtility.GetJsonObjectType(targetType);
            if (objectType == JType.Dynamic || objectType == JType.Value)
            {
                return null;
            }

            var serializerKey = GetSerializerKey(targetType, encoding, omitDefaultValue);

            JsonSerializer serializer;
            if (!Caches.ContainsKey(serializerKey))
            {
                return Caches.GetOrAdd(serializerKey, new JsonSerializer(targetType, encoding, omitDefaultValue));
            }
            else if (Caches.TryGetValue(serializerKey, out serializer))
            {
                return serializer;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Serialization

        public void Serialize(object instance, Stream stream)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (JsonObjectType == JType.Object)
            {
                SerializeJsonClassObject(stream, instance);
            }
            else if (JsonObjectType == JType.Map)
            {
                SerializeJsonDictionaryObject(stream, instance);
            }
            else if (JsonObjectType == JType.Array)
            {
                SerializeJsonCollectionObject(stream, instance);
            }
            else
            {
                throw new Exception(string.Format("failed to serialize object '{0}'.", instance));
            }
        }

        public async Task SerializeAsync(object instance, Stream stream)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var bufferedStream = new BufferedWriterStream(stream, 8 * 1024);

            await InternalSerializeAsync(instance, bufferedStream);

            await bufferedStream.EndWrite();
        }

        private async Task InternalSerializeAsync(object instance, IWriterStream stream)
        {
            if (JsonObjectType == JType.Object)
            {
                await SerializeJsonClassObjectAsync(stream, instance);
            }
            else if (JsonObjectType == JType.Map)
            {
                await SerializeJsonDictionaryObjectAsync(stream, instance);
            }
            else if (JsonObjectType == JType.Array)
            {
                await SerializeJsonCollectionObjectAsync(stream, instance);
            }
            else
            {
                throw new Exception(string.Format("failed to serialize object '{0}'.", instance));
            }
        }

        private void SerializeJsonClassObject(Stream stream, object value)
        {
            stream.WriteByte(JsonConstants.Left_Brace);

            var firstElement = true;
            foreach (var jsonProperty in JsonProperties.Values.ToArray())
            {
                var propertyValue = jsonProperty.PropertyInfo.GetValue(value, null);
                if (JsonSettings.OmitDefaultValue && jsonProperty.DefaultValue.EqualsWith(propertyValue))
                {
                    continue;
                }

                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    stream.WriteByte(JsonConstants.Comma);
                }

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{jsonProperty.Key}\":");
                stream.Write(buf, 0, buf.Length);

                SerializeRegularValue(stream, propertyValue, jsonProperty.ObjectType);
            }

            stream.WriteByte(JsonConstants.Right_Brace);
        }

        private async Task SerializeJsonClassObjectAsync(IWriterStream stream, object value)
        {
            await stream.WriteAsync(JsonConstants.Left_Brace);

            var firstElement = true;
            foreach (var jsonProperty in JsonProperties.Values.ToArray())
            {
                var propertyValue = jsonProperty.PropertyInfo.GetValue(value, null);
                if (JsonSettings.OmitDefaultValue && jsonProperty.DefaultValue.EqualsWith(propertyValue))
                {
                    continue;
                }

                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    await stream.WriteAsync(JsonConstants.Comma);
                }

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{jsonProperty.Key}\":");
                await stream.WriteAsync(buf, 0, buf.Length);

                await SerializeRegularValueAsync(stream, propertyValue, jsonProperty.ObjectType);
            }

            await stream.WriteAsync(JsonConstants.Right_Brace);
        }

        private void SerializeJsonDictionaryObject(Stream stream, object value)
        {
            stream.WriteByte(JsonConstants.Left_Brace);

            var args = value.GetType().GetGenericArguments();
            if (args == null || args.Length != 2)
            {
                // TODO: log
                return;
            }

            var objectType = JsonUtility.GetJsonObjectType(args[1]);

            var dictionary = value as IDictionary;

            var firstElement = true;
            foreach (var key in dictionary.Keys)
            {
                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    stream.WriteByte(JsonConstants.Comma);
                }

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{key}\":");
                stream.Write(buf, 0, buf.Length);

                SerializeRegularValue(stream, dictionary[key], JsonUtility.GetJsonObjectType(dictionary[key].GetType()));
            }

            stream.WriteByte(JsonConstants.Right_Brace);
        }

        private async Task SerializeJsonDictionaryObjectAsync(IWriterStream stream, object value)
        {
            await stream.WriteAsync(JsonConstants.Left_Brace);

            var args = value.GetType().GetGenericArguments();
            if (args == null || args.Length != 2)
            {
                // TODO: log
                return;
            }

            var objectType = JsonUtility.GetJsonObjectType(args[1]);

            var dictionary = value as IDictionary;

            var firstElement = true;
            foreach (var key in dictionary.Keys)
            {
                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    await stream.WriteAsync(JsonConstants.Comma);
                }

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{key}\":");
                await stream.WriteAsync(buf, 0, buf.Length);

                await SerializeRegularValueAsync(stream, dictionary[key], JsonUtility.GetJsonObjectType(dictionary[key].GetType()));
            }

            await stream.WriteAsync(JsonConstants.Right_Brace);
        }

        private void SerializeJsonCollectionObject(Stream stream, object value)
        {
            stream.WriteByte(JsonConstants.Left_Bracket);

            JType objectType;
            if (value is Array)
            {
                objectType = JsonUtility.GetJsonObjectType(value.GetType().GetElementType());
            }
            else
            {
                objectType = JsonUtility.GetJsonObjectType(value.GetType().GetGenericArguments()[0]);
            }

            var collection = value as ICollection;

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
                    stream.WriteByte(JsonConstants.Comma);
                }

                SerializeRegularValue(stream, element, objectType);
            }

            stream.WriteByte(JsonConstants.Right_Bracket);
        }

        private async Task SerializeJsonCollectionObjectAsync(IWriterStream stream, object value)
        {
            await stream.WriteAsync(JsonConstants.Left_Bracket);

            JType objectType;
            if (value is Array)
            {
                objectType = JsonUtility.GetJsonObjectType(value.GetType().GetElementType());
            }
            else
            {
                objectType = JsonUtility.GetJsonObjectType(value.GetType().GetGenericArguments()[0]);
            }
            
            var collection = value as ICollection;

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
                    await stream.WriteAsync(JsonConstants.Comma);
                }

                await SerializeRegularValueAsync(stream, element, objectType);
            }

            await stream.WriteAsync(JsonConstants.Right_Bracket);
        }

        private void SerializeRegularValue(Stream stream, object value, JType objectType)
        {
            if (objectType == JType.Value)
            {
                var buf = JsonSettings.SerializeValue(value);
                stream.Write(buf, 0, buf.Length);
            }
            else if (value == null)
            {
                stream.Write(JsonConstants.NullValueBytes, 0, JsonConstants.NullValueBytes.Length);
            }
            else if (objectType == JType.Dynamic)
            {
                SerializeRegularValue(stream, value, JsonUtility.GetJsonObjectType(value.GetType()));
            }
            else
            {
                GetSerializer(value.GetType(), JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).Serialize(value, stream);
            }
        }

        private async Task SerializeRegularValueAsync(IWriterStream stream, object value, JType objectType)
        {
            if (objectType == JType.Value)
            {
                var buf = JsonSettings.SerializeValue(value);
                await stream.WriteAsync(buf, 0, buf.Length);
            }
            else if (value == null)
            {
                await stream.WriteAsync(JsonConstants.NullValueBytes, 0, JsonConstants.NullValueBytes.Length);
            }
            else if (objectType == JType.Dynamic)
            {
                await SerializeRegularValueAsync(stream, value, JsonUtility.GetJsonObjectType(value.GetType()));
            }
            else
            {
                await GetSerializer(value.GetType(), JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalSerializeAsync(value, stream);
            }
        }

        #endregion

        #region Deserialization

        public object Deserialize(Stream stream)
        {
            var args = new JsonParserArgs()
            {
                Stream = new Internal.BufferedReaderStream(stream, 8 * 1024),
            };

            JsonParser.Parse(args);

            return InternalDeserialize(args.InternalObject);
        }

        public async Task<object> DeserializeAsync(Stream stream)
        {
            var args = new JsonParserArgs()
            {
                Stream = new Internal.BufferedReaderStream(stream, 8 * 1024),
            };

            await JsonParser.ParseAsync(args);

            return InternalDeserialize(args.InternalObject);
        }

        public object Deserialize(JBase jsonObject)
        {
            return InternalDeserialize(jsonObject);
        }

        private object InternalDeserialize(JBase jsonObject)
        {
            if (jsonObject is JObject)
            {
                return DeserializeJsonClassObject(jsonObject as JObject);
            }
            else if (jsonObject is JArray)
            {
                return DeserializeJsonCollectionObject(jsonObject as JArray, Type);
            }
            else if (jsonObject is JValue)
            {
                return JsonSettings.DeserializeValue(jsonObject as JValue, Type);
            }

            return null;
        }

        private object DeserializeJsonClassObject(JObject dictionaryObject)
        {
            if (JsonObjectType == JType.Object)
            {
                var instance = Activator.CreateInstance(Type);

                foreach (var element in dictionaryObject.Elements)
                {
                    var key = JsonSettings.CurrentEncoding.GetString(element.Key);

                    JsonProperty jsonProperty;
                    JsonProperties.TryGetValue(key, out jsonProperty);
                    if (jsonProperty == null)
                    {
                        continue;
                    }

                    if (jsonProperty.IsJsonObject)
                    {
                        jsonProperty.PropertyInfo.SetValue(instance, element, null);
                        continue;
                    }

                    if (jsonProperty.ObjectType == JType.Dynamic)
                    {
                        continue;
                    }

                    if (jsonProperty.ObjectType == JType.Object)
                    {
                        if (element is JObject)
                        {
                            var value = GetSerializer(jsonProperty.PropertyInfo.PropertyType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(element);
                            jsonProperty.PropertyInfo.SetValue(instance, value, null);
                        }
                        else if (element is JValue)
                        {
                            var value = element as JValue;
                            jsonProperty.PropertyInfo.SetValue(instance,
                                JsonSettings.DeserializeValue(value, jsonProperty.PropertyInfo.PropertyType),
                                null);
                        }
                        else
                        {
                            throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
                        }
                    }
                    else if (jsonProperty.ObjectType == JType.Array)
                    {
                        if (element is JArray)
                        {
                            var collectionObject = element as JArray;
                            jsonProperty.PropertyInfo.SetValue(instance,
                                DeserializeJsonCollectionObject(collectionObject, jsonProperty.PropertyInfo.PropertyType), null);
                        }
                        else if (element is JValue)
                        {
                            var value = element as JValue;
                            jsonProperty.PropertyInfo.SetValue(instance,
                                JsonSettings.DeserializeValue(value, jsonProperty.PropertyInfo.PropertyType),
                                null);
                        }
                        else
                        {
                            throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
                        }
                    }
                    else if (jsonProperty.ObjectType == JType.Value && element is JValue)
                    {
                        var value = JsonSettings.DeserializeValue(element as JValue, jsonProperty.PropertyInfo.PropertyType);
                        jsonProperty.PropertyInfo.SetValue(instance, value, null);
                    }
                    else
                    {
                        throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
                    }
                }

                return instance;
            }
            else if (JsonObjectType == JType.Map)
            {
                var type = typeof(Dictionary<,>);

                var args = Type.GetGenericArguments();

                var instance = Activator.CreateInstance(type.MakeGenericType(args)) as IDictionary;

                foreach (var element in dictionaryObject.Elements)
                {
                    var dictKey = JsonSettings.CurrentEncoding.GetString(element.Key);

                    object dictValue = null;

                    var valueType = JsonUtility.GetJsonObjectType(args[1]);

                    if (valueType == JType.Dynamic)
                    {
                        continue;
                    }

                    if (valueType == JType.Object)
                    {
                        if (element is JObject)
                        {
                            dictValue = GetSerializer(args[1], JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(element);
                        }
                        else if (element is JValue)
                        {
                            dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
                        }
                        else
                        {
                            throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
                        }
                    }
                    else if (valueType == JType.Array)
                    {
                        if (element is JArray)
                        {
                            dictValue = DeserializeJsonCollectionObject(element as JArray, args[1]);
                        }
                        else if (element is JValue)
                        {
                            dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
                        }
                        else
                        {
                            throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
                        }
                    }
                    else if (valueType == JType.Value && element is JValue)
                    {
                        dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
                    }
                    else
                    {
                        throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
                    }

                    instance.Add(dictKey, dictValue);
                }

                return instance;
            }

            return null;
        }

        private object DeserializeJsonCollectionObject(JArray collectionObject, Type targetType)
        {
            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                var array = Array.CreateInstance(elementType, collectionObject.Elements.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    var jsonObject = collectionObject.Elements[i];
                    if (jsonObject is JObject)
                    {
                        var value = GetSerializer(elementType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(jsonObject);
                        array.SetValue(value, i);
                    }
                    else if (jsonObject is JArray)
                    {
                        // TODO: multi-dimension array
                    }
                    else if (jsonObject is JValue)
                    {
                        var value = JsonSettings.DeserializeValue(jsonObject as JValue, elementType);
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
                    var jsonObject = collectionObject.Elements[i];
                    if (jsonObject is JObject)
                    {
                        var value = GetSerializer(elementType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(jsonObject);
                        collection.Add(value);
                    }
                    else if (jsonObject is JArray)
                    {
                        // TODO: multi-dimension array
                    }
                    else if (jsonObject is JValue)
                    {
                        var value = JsonSettings.DeserializeValue(jsonObject as JValue, elementType);
                        collection.Add(value);
                    }
                }

                return collection;
            }

            return null;
        }

        #endregion
    }
}