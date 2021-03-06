﻿using System;
using System.IO;
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
        public JsonSerializer(Type type, JsonSettings jsonSettings)
        {
            Type = type;
            JsonSettings = jsonSettings;

            JsonType = JsonUtility.GetJsonType(Type);
            Initialize();
        }

        public Type Type { get; private set; }

        public JsonSettings JsonSettings { get; private set; }

        public JType JsonType { get; private set; }

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
                        var jsonType = JsonUtility.GetJsonType(Type);

                        // only parse json dictionary object
                        if (jsonType == JType.Object)
                        {
                            foreach (var propertyInfo in Type.GetTypeInfo().GetProperties().Where(x => x.CanRead && !x.IsDefined(typeof(JsonIgnoreAttribute))))
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

                                if (JsonSettings.ElementKeyIgnoreCase)
                                {
                                    JsonProperties.TryAdd(jsonProperty.Key.ToLower(), jsonProperty);
                                }
                                else
                                {
                                    JsonProperties.TryAdd(jsonProperty.Key, jsonProperty);
                                }
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

        private static string GetSerializerKey(Type type, JsonSettings jsonSettings)
        {
            return $"{type.FullName};{jsonSettings.CurrentEncoding.EncodingName};{jsonSettings.OmitDefaultValue.ToString()};{jsonSettings.DateTimeFormat};{jsonSettings.ElementKeyIgnoreCase};{jsonSettings.OmitNullValue}".Md5();
        }

        public static JsonSerializer GetSerializer(Type targetType, JsonSettings jsonSettings)
        {
            var jsonType = JsonUtility.GetJsonType(targetType);
            if (jsonType == JType.Dynamic || jsonType == JType.Value)
            {
                return null;
            }

            var serializerKey = GetSerializerKey(targetType, jsonSettings);

            JsonSerializer serializer;
            if (!Caches.ContainsKey(serializerKey))
            {
                return Caches.GetOrAdd(serializerKey, new JsonSerializer(targetType, jsonSettings));
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

            if (JsonType == JType.Object)
            {
                SerializeJsonClassObject(stream, instance);
            }
            else if (JsonType == JType.Map)
            {
                SerializeJsonDictionaryObject(stream, instance);
            }
            else if (JsonType == JType.Array)
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
            if (JsonType == JType.Object)
            {
                await SerializeJsonClassObjectAsync(stream, instance);
            }
            else if (JsonType == JType.Map)
            {
                await SerializeJsonDictionaryObjectAsync(stream, instance);
            }
            else if (JsonType == JType.Array)
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

                if (JsonSettings.OmitNullValue && propertyValue == null)
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

                SerializeRegularValue(stream, propertyValue, jsonProperty.JsonType);
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

                if (JsonSettings.OmitNullValue && propertyValue == null)
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

                await SerializeRegularValueAsync(stream, propertyValue, jsonProperty.JsonType);
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

            var dictionary = value as IDictionary;

            var firstElement = true;
            foreach (var key in dictionary.Keys)
            {
                if (JsonSettings.OmitNullValue && dictionary[key] == null)
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

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{key}\":");
                stream.Write(buf, 0, buf.Length);

                SerializeRegularValue(stream, dictionary[key], JsonUtility.GetJsonType(dictionary[key].GetType()));
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

            var dictionary = value as IDictionary;

            var firstElement = true;
            foreach (var key in dictionary.Keys)
            {
                if (JsonSettings.OmitNullValue && dictionary[key] == null)
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

                var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{key}\":");
                await stream.WriteAsync(buf, 0, buf.Length);

                await SerializeRegularValueAsync(stream, dictionary[key], JsonUtility.GetJsonType(dictionary[key].GetType()));
            }

            await stream.WriteAsync(JsonConstants.Right_Brace);
        }

        private void SerializeJsonCollectionObject(Stream stream, object value)
        {
            stream.WriteByte(JsonConstants.Left_Bracket);

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

                SerializeRegularValue(stream, element, JsonUtility.GetJsonType(element.GetType()));
            }

            stream.WriteByte(JsonConstants.Right_Bracket);
        }

        private async Task SerializeJsonCollectionObjectAsync(IWriterStream stream, object value)
        {
            await stream.WriteAsync(JsonConstants.Left_Bracket);
            
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

                await SerializeRegularValueAsync(stream, element, JsonUtility.GetJsonType(element.GetType()));
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
                SerializeRegularValue(stream, value, JsonUtility.GetJsonType(value.GetType()));
            }
            else
            {
                GetSerializer(value.GetType(), JsonSettings).Serialize(value, stream);
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
                await SerializeRegularValueAsync(stream, value, JsonUtility.GetJsonType(value.GetType()));
            }
            else
            {
                await GetSerializer(value.GetType(), JsonSettings).InternalSerializeAsync(value, stream);
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
            if (JsonType == JType.Object)
            {
                var instance = Activator.CreateInstance(Type);

                foreach (var element in dictionaryObject.Elements)
                {
                    var key = JsonSettings.CurrentEncoding.GetString(element.Key);

                    if (JsonSettings.ElementKeyIgnoreCase)
                    {
                        key = key.ToLower();
                    }

                    JsonProperty jsonProperty;
                    JsonProperties.TryGetValue(key, out jsonProperty);
                    if (jsonProperty == null || !jsonProperty.CanWrite)
                    {
                        continue;
                    }

                    if (jsonProperty.IsJsonObject)
                    {
                        jsonProperty.PropertyInfo.SetValue(instance, element, null);
                        continue;
                    }

                    if (jsonProperty.JsonType == JType.Dynamic)
                    {
                        continue;
                    }

                    if (jsonProperty.JsonType == JType.Object || jsonProperty.JsonType == JType.Map)
                    {
                        if (element is JObject)
                        {
                            var value = GetSerializer(jsonProperty.PropertyInfo.PropertyType, JsonSettings).InternalDeserialize(element);
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
                    else if (jsonProperty.JsonType == JType.Array)
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
                    else if (jsonProperty.JsonType == JType.Value && element is JValue)
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
            else if (JsonType == JType.Map)
            {
                var type = typeof(Dictionary<,>);

                var args = Type.GetGenericArguments();

                var instance = Activator.CreateInstance(type.MakeGenericType(args)) as IDictionary;

                foreach (var element in dictionaryObject.Elements)
                {
                    var dictKey = JsonSettings.CurrentEncoding.GetString(element.Key);

                    object dictValue = null;

                    var valueType = JsonUtility.GetJsonType(args[1]);

                    if (valueType == JType.Dynamic)
                    {
                        continue;
                    }

                    if (valueType == JType.Object)
                    {
                        if (element is JObject)
                        {
                            dictValue = GetSerializer(args[1], JsonSettings).InternalDeserialize(element);
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

                    instance.Add(dictKey.ConvertTo(args[0]), dictValue);
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
                        var value = GetSerializer(elementType, JsonSettings).InternalDeserialize(jsonObject);
                        array.SetValue(value, i);
                    }
                    else if (jsonObject is JArray)
                    {
                        var value = GetSerializer(elementType, JsonSettings).InternalDeserialize(jsonObject);
                        array.SetValue(value, i);
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
                var elementType = targetType.GetGenericArguments().FirstOrDefault() ?? (targetType.BaseType.GetGenericArguments().FirstOrDefault() ?? typeof(object));
                var collection = Activator.CreateInstance(targetType) as IList;
                for (int i = 0; i < collectionObject.Elements.Length; i++)
                {
                    var jsonObject = collectionObject.Elements[i];
                    if (jsonObject is JObject)
                    {
                        var value = GetSerializer(elementType, JsonSettings).InternalDeserialize(jsonObject);
                        collection.Add(value);
                    }
                    else if (jsonObject is JArray)
                    {
                        var value = GetSerializer(elementType, JsonSettings).InternalDeserialize(jsonObject);
                        collection.Add(value);
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