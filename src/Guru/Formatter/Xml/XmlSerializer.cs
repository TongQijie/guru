using Guru.ExtensionMethod;
using Guru.Formatter.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Guru.Formatter.Xml
{
    internal class XmlSerializer
    {
        public XmlSerializer(Type type, Encoding encoding, bool omitDefaultValue)
        {
            Type = type;
            XmlSettings = new XmlSettings(encoding, omitDefaultValue);

            XmlType = XmlUtility.GetXmlType(Type);
            Initialize();
        }

        public Type Type { get; private set; }

        public XType XmlType { get; private set; }

        public XmlSettings XmlSettings { get; private set; }

        #region Initialization

        private ConcurrentDictionary<string, XmlProperty> _XmlProperties = null;

        private ConcurrentDictionary<string, XmlProperty> XmlProperties
        {
            get { return _XmlProperties ?? (_XmlProperties = new ConcurrentDictionary<string, XmlProperty>()); }
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
                        var jsonType = XmlUtility.GetXmlType(Type);

                        // only parse json dictionary object
                        if (jsonType == XType.Object)
                        {
                            foreach (var propertyInfo in Type.GetTypeInfo().GetProperties().Where(x => x.CanRead && x.CanWrite && !x.IsDefined(typeof(XmlIgnoreAttribute))))
                            {
                                XmlProperty xmlProperty = null;

                                var attribute = propertyInfo.GetCustomAttribute<XmlPropertyAttribute>();
                                if (attribute == null)
                                {
                                    xmlProperty = new XmlProperty(propertyInfo, null, false, false, false, null);
                                }
                                else
                                {
                                    xmlProperty = new XmlProperty(propertyInfo, attribute.Alias, attribute.Attribute, attribute.Array, attribute.ArrayItem, attribute.ArrayItemAlias);
                                }

                                XmlProperties.TryAdd(xmlProperty.Key, xmlProperty);
                            }
                        }

                        _IsInitialized = true;
                    }
                }
            }
        }

        #endregion

        #region Serializer Cache

        private static ConcurrentDictionary<string, XmlSerializer> _Caches = null;

        private static ConcurrentDictionary<string, XmlSerializer> Caches
        {
            get { return _Caches ?? (_Caches = new ConcurrentDictionary<string, XmlSerializer>()); }
        }

        private static string GetSerializerKey(Type type, Encoding encoding, bool omitDefaultValue)
        {
            return $"{type.FullName};{encoding.EncodingName};{omitDefaultValue.ToString()}";
        }

        public static XmlSerializer GetSerializer(Type targetType, Encoding encoding, bool omitDefaultValue)
        {
            var jsonType = XmlUtility.GetXmlType(targetType);
            if (jsonType == XType.Dynamic || jsonType == XType.Value)
            {
                return null;
            }

            var serializerKey = GetSerializerKey(targetType, encoding, omitDefaultValue);

            XmlSerializer serializer;
            if (!Caches.ContainsKey(serializerKey))
            {
                return Caches.GetOrAdd(serializerKey, new XmlSerializer(targetType, encoding, omitDefaultValue));
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

        //public void Serialize(object instance, Stream stream)
        //{
        //    if (instance == null)
        //    {
        //        throw new ArgumentNullException("instance");
        //    }

        //    if (XmlType == XType.Object)
        //    {
        //        SerializeJsonClassObject(stream, instance);
        //    }
        //    else if (XmlType == XType.Map)
        //    {
        //        SerializeJsonDictionaryObject(stream, instance);
        //    }
        //    else if (XmlType == XType.Array)
        //    {
        //        SerializeJsonCollectionObject(stream, instance);
        //    }
        //    else
        //    {
        //        throw new Exception(string.Format("failed to serialize object '{0}'.", instance));
        //    }
        //}

        public async Task SerializeAsync(object instance, Stream stream)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var bufferedStream = new BufferedWriterStream(stream, 8 * 1024);

            await InternalSerializeAsync(instance, bufferedStream, instance.GetType().Name);

            await bufferedStream.EndWrite();
        }

        private async Task InternalSerializeAsync(object instance, IWriterStream stream, string tagName)
        {
            await stream.WriteAsync(XmlSettings.StartTag(tagName));

            if (XmlType == XType.Object)
            {
                await SerializeJsonClassObjectAsync(stream, instance);
            }
            else if (XmlType == XType.Array)
            {
                await SerializeJsonCollectionObjectAsync(stream, instance);
            }
            else
            {
                throw new Exception(string.Format("failed to serialize object '{0}'.", instance));
            }

            await stream.WriteAsync(XmlSettings.EndTag(tagName));
        }

        //private void SerializeJsonClassObject(Stream stream, object value)
        //{


        //    stream.WriteByte(XmlSettings.StartTag();

        //    var firstElement = true;
        //    foreach (var jsonProperty in JsonProperties.Values.ToArray())
        //    {
        //        var propertyValue = jsonProperty.PropertyInfo.GetValue(value, null);
        //        if (JsonSettings.OmitDefaultValue && jsonProperty.DefaultValue.EqualsWith(propertyValue))
        //        {
        //            continue;
        //        }

        //        if (firstElement)
        //        {
        //            firstElement = false;
        //        }
        //        else
        //        {
        //            stream.WriteByte(JsonConstants.Comma);
        //        }

        //        var buf = JsonSettings.CurrentEncoding.GetBytes($"\"{jsonProperty.Key}\":");
        //        stream.Write(buf, 0, buf.Length);

        //        SerializeRegularValue(stream, propertyValue, jsonProperty.JsonType);
        //    }

        //    stream.WriteByte(JsonConstants.Right_Brace);
        //}

        private async Task SerializeJsonClassObjectAsync(IWriterStream stream, object value)
        {
            foreach (var xmlProperty in XmlProperties.Values)
            {
                var propertyValue = xmlProperty.PropertyInfo.GetValue(value, null);
                if (propertyValue == null)
                {
                    continue;
                }

                if (XmlSettings.OmitDefaultValue && xmlProperty.DefaultValue.EqualsWith(propertyValue))
                {
                    continue;
                }

                await SerializeRegularValueAsync(stream, propertyValue, xmlProperty.XmlType, xmlProperty.Key);
            }
        }

        //private void SerializeJsonCollectionObject(Stream stream, object value)
        //{
        //    stream.WriteByte(JsonConstants.Left_Bracket);

        //    var collection = value as ICollection;

        //    var firstElement = true;
        //    foreach (var element in collection)
        //    {
        //        if (element == null)
        //        {
        //            continue;
        //        }

        //        if (firstElement)
        //        {
        //            firstElement = false;
        //        }
        //        else
        //        {
        //            stream.WriteByte(JsonConstants.Comma);
        //        }

        //        SerializeRegularValue(stream, element, JsonUtility.GetJsonType(element.GetType()));
        //    }

        //    stream.WriteByte(JsonConstants.Right_Bracket);
        //}

        private async Task SerializeJsonCollectionObjectAsync(IWriterStream stream, object value)
        {
            var collection = value as ICollection;

            foreach (var element in collection)
            {
                if (element == null)
                {
                    continue;
                }

                await SerializeRegularValueAsync(stream, element, XmlUtility.GetXmlType(element.GetType()), element.GetType().Name);
            }
        }

        //private void SerializeRegularValue(Stream stream, object value, JType objectType)
        //{
        //    if (objectType == JType.Value)
        //    {
        //        var buf = JsonSettings.SerializeValue(value);
        //        stream.Write(buf, 0, buf.Length);
        //    }
        //    else if (value == null)
        //    {
        //        stream.Write(JsonConstants.NullValueBytes, 0, JsonConstants.NullValueBytes.Length);
        //    }
        //    else if (objectType == JType.Dynamic)
        //    {
        //        SerializeRegularValue(stream, value, JsonUtility.GetJsonType(value.GetType()));
        //    }
        //    else
        //    {
        //        GetSerializer(value.GetType(), JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).Serialize(value, stream);
        //    }
        //}

        private async Task SerializeRegularValueAsync(IWriterStream stream, object value, XType xmlType, string tagName)
        {
            if (xmlType == XType.Value)
            {
                await stream.WriteAsync(XmlSettings.StartTag(tagName));

                await stream.WriteAsync(XmlSettings.SerializeValue(value));

                await stream.WriteAsync(XmlSettings.EndTag(tagName));
            }
            else if (xmlType == XType.Dynamic)
            {
                await SerializeRegularValueAsync(stream, value, XmlUtility.GetXmlType(value.GetType()), tagName);
            }
            else
            {
                await GetSerializer(value.GetType(), XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalSerializeAsync(value, stream, tagName);
            }
        }

        #endregion

        #region Deserialization

        //public object Deserialize(Stream stream)
        //{
        //    var args = new JsonParserArgs()
        //    {
        //        Stream = new Internal.BufferedReaderStream(stream, 8 * 1024),
        //    };

        //    JsonParser.Parse(args);

        //    return InternalDeserialize(args.InternalObject);
        //}

        public async Task<object> DeserializeAsync(Stream stream)
        {
            var xObject = await XmlParser.ParseAsync(new Internal.BufferedReaderStream(stream, 8 * 1024));

            return InternalDeserialize(xObject);
        }

        private object InternalDeserialize(XObject xObject)
        {
            var instance = Activator.CreateInstance(Type);

            if (! xObject.Elements.HasLength())
            {
                return instance;
            }

            foreach (var kv in xObject.Elements.OfType<XAttribute>().GroupBy(x => XmlSettings.CurrentEncoding.GetString(x.Key)))
            {
                if (kv.Count() == 1 && XmlProperties.ContainsKey(kv.Key))
                {
                    var property = XmlProperties[kv.Key];
                    if (property.Attribute)
                    {
                        property.PropertyInfo.SetValue(instance, XmlSettings.CurrentEncoding.GetString(kv.First().Value).ConvertTo(property.PropertyInfo.PropertyType));
                    }
                }
            }

            foreach (var kv in xObject.Elements.OfType<XObject>().GroupBy(x => XmlSettings.CurrentEncoding.GetString(x.Key)))
            {
                if (XmlProperties.ContainsKey(kv.Key))
                {
                    var property = XmlProperties[kv.Key];
                    property.PropertyInfo.SetValue(instance, InternalDeserialize(property, kv.ToArray()));
                }
            }

            return instance;
        }

        private object InternalDeserialize(XmlProperty xmlProperty, XObject[] xObjects)
        {
            if (xmlProperty.XmlType == XType.Object && xObjects.Length == 1)
            {
                return GetSerializer(xmlProperty.PropertyInfo.PropertyType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObjects[0]);
            }
            else if (xmlProperty.XmlType == XType.Array || xmlProperty.ArrayElementType != null)
            {
                if (xmlProperty.ArrayItem)
                {
                    var targetType = xmlProperty.PropertyInfo.PropertyType;

                    if (targetType.IsArray)
                    {
                        var elementType = targetType.GetElementType();
                        var array = Array.CreateInstance(elementType, xObjects.Length);
                        for (var i = 0; i < array.Length; i++)
                        {
                            var xObject = xObjects[i];
                            var value = GetSerializer(elementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObject);
                            array.SetValue(value, i);
                        }

                        return array;
                    }
                    else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                    {
                        var elementType = targetType.GetTypeInfo().GetGenericArguments().FirstOrDefault() ?? typeof(object);
                        var collection = Activator.CreateInstance(targetType) as IList;
                        for (int i = 0; i < xObjects.Length; i++)
                        {
                            var xObject = xObjects[i];
                            var value = GetSerializer(elementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObject);
                            collection.Add(value);
                        }

                        return collection;
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
                else
                {
                    if (xObjects.Length != 1)
                    {
                        throw new Exception("");
                    }

                    if (!xObjects[0].Elements.HasLength())
                    {
                        var targetType = xmlProperty.PropertyInfo.PropertyType;

                        if (targetType.IsArray)
                        {
                            var elementType = targetType.GetElementType();
                            var array = Array.CreateInstance(elementType, xObjects.Length);
                            return array;
                        }
                        else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                        {
                            var elementType = targetType.GetTypeInfo().GetGenericArguments().FirstOrDefault() ?? typeof(object);
                            var collection = Activator.CreateInstance(targetType) as IList;
                            return collection;
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }

                    var xmlType = XmlUtility.GetXmlType(xmlProperty.ArrayElementType);
                    if (xmlType == XType.Object)
                    {
                        var elements = xObjects[0].Elements.OfType<XObject>().Where(x => XmlSettings.CurrentEncoding.GetString(x.Key) == xmlProperty.ArrayItemAlias).ToArray();

                        var targetType = xmlProperty.PropertyInfo.PropertyType;

                        if (targetType.IsArray)
                        {
                            var array = Array.CreateInstance(xmlProperty.ArrayElementType, elements.Length);
                            for (var i = 0; i < array.Length; i++)
                            {
                                var xObject = elements[i];
                                var value = GetSerializer(xmlProperty.ArrayElementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObject);
                                array.SetValue(value, i);
                            }

                            return array;
                        }
                        else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                        {
                            var collection = Activator.CreateInstance(targetType) as IList;
                            for (int i = 0; i < elements.Length; i++)
                            {
                                var xObject = elements[i];
                                var value = GetSerializer(xmlProperty.ArrayElementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObject);
                                collection.Add(value);
                            }

                            return collection;
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                    else if (xmlType == XType.Value)
                    {
                        var elements = xObjects[0].Elements.OfType<XObject>().Where(x => XmlSettings.CurrentEncoding.GetString(x.Key) == xmlProperty.ArrayItemAlias).ToArray();

                        var targetType = xmlProperty.PropertyInfo.PropertyType;

                        if (targetType.IsArray)
                        {
                            var array = Array.CreateInstance(xmlProperty.ArrayElementType, elements.Length);
                            for (var i = 0; i < array.Length; i++)
                            {
                                array.SetValue(DeserializeValue(elements[0], xmlProperty.ArrayElementType), i);
                            }

                            return array;
                        }
                        else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                        {
                            var collection = Activator.CreateInstance(targetType) as IList;
                            for (int i = 0; i < elements.Length; i++)
                            {
                                collection.Add(DeserializeValue(elements[0], xmlProperty.ArrayElementType));
                            }

                            return collection;
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                    else
                    {
                        throw new Exception("");
                    }
                    
                }
            }
            else if (xmlProperty.XmlType == XType.Value && xObjects.Length == 1)
            {
                return DeserializeValue(xObjects[0], xmlProperty.PropertyInfo.PropertyType);
            }

            return null;
        }

        private object DeserializeValue(XObject xObject, Type targetType)
        {
            if (!xObject.Elements.HasLength())
            {
                return string.Empty.ConvertTo(targetType);
            }

            var values = xObject.Elements.Subset(x => !(x is XComment));

            if (values.Length == 0 || values.Length > 1 || !(values[0] is XValue))
            {
                throw new Exception("");
            }

            var value = values[0] as XValue;

            return XmlSettings.CurrentEncoding.GetString(value.Value).ConvertTo(targetType);
        }

        //public object Deserialize(JBase jsonObject)
        //{
        //    return InternalDeserialize(jsonObject);
        //}

        //private object InternalDeserialize(JBase jsonObject)
        //{
        //    if (jsonObject is JObject)
        //    {
        //        return DeserializeJsonClassObject(jsonObject as JObject);
        //    }
        //    else if (jsonObject is JArray)
        //    {
        //        return DeserializeJsonCollectionObject(jsonObject as JArray, Type);
        //    }
        //    else if (jsonObject is JValue)
        //    {
        //        return JsonSettings.DeserializeValue(jsonObject as JValue, Type);
        //    }

        //    return null;
        //}

        //private object DeserializeJsonClassObject(JObject dictionaryObject)
        //{
        //    if (JsonType == JType.Object)
        //    {
        //        var instance = Activator.CreateInstance(Type);

        //        foreach (var element in dictionaryObject.Elements)
        //        {
        //            var key = JsonSettings.CurrentEncoding.GetString(element.Key);

        //            JsonProperty jsonProperty;
        //            JsonProperties.TryGetValue(key, out jsonProperty);
        //            if (jsonProperty == null)
        //            {
        //                continue;
        //            }

        //            if (jsonProperty.IsJsonObject)
        //            {
        //                jsonProperty.PropertyInfo.SetValue(instance, element, null);
        //                continue;
        //            }

        //            if (jsonProperty.JsonType == JType.Dynamic)
        //            {
        //                continue;
        //            }

        //            if (jsonProperty.JsonType == JType.Object)
        //            {
        //                if (element is JObject)
        //                {
        //                    var value = GetSerializer(jsonProperty.PropertyInfo.PropertyType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(element);
        //                    jsonProperty.PropertyInfo.SetValue(instance, value, null);
        //                }
        //                else if (element is JValue)
        //                {
        //                    var value = element as JValue;
        //                    jsonProperty.PropertyInfo.SetValue(instance,
        //                        JsonSettings.DeserializeValue(value, jsonProperty.PropertyInfo.PropertyType),
        //                        null);
        //                }
        //                else
        //                {
        //                    throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
        //                }
        //            }
        //            else if (jsonProperty.JsonType == JType.Array)
        //            {
        //                if (element is JArray)
        //                {
        //                    var collectionObject = element as JArray;
        //                    jsonProperty.PropertyInfo.SetValue(instance,
        //                        DeserializeJsonCollectionObject(collectionObject, jsonProperty.PropertyInfo.PropertyType), null);
        //                }
        //                else if (element is JValue)
        //                {
        //                    var value = element as JValue;
        //                    jsonProperty.PropertyInfo.SetValue(instance,
        //                        JsonSettings.DeserializeValue(value, jsonProperty.PropertyInfo.PropertyType),
        //                        null);
        //                }
        //                else
        //                {
        //                    throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
        //                }
        //            }
        //            else if (jsonProperty.JsonType == JType.Value && element is JValue)
        //            {
        //                var value = JsonSettings.DeserializeValue(element as JValue, jsonProperty.PropertyInfo.PropertyType);
        //                jsonProperty.PropertyInfo.SetValue(instance, value, null);
        //            }
        //            else
        //            {
        //                throw new Errors.JsonSerializeFailedException(key, ".net runtime type does not match json type.");
        //            }
        //        }

        //        return instance;
        //    }
        //    else if (JsonType == JType.Map)
        //    {
        //        var type = typeof(Dictionary<,>);

        //        var args = Type.GetGenericArguments();

        //        var instance = Activator.CreateInstance(type.MakeGenericType(args)) as IDictionary;

        //        foreach (var element in dictionaryObject.Elements)
        //        {
        //            var dictKey = JsonSettings.CurrentEncoding.GetString(element.Key);

        //            object dictValue = null;

        //            var valueType = JsonUtility.GetJsonType(args[1]);

        //            if (valueType == JType.Dynamic)
        //            {
        //                continue;
        //            }

        //            if (valueType == JType.Object)
        //            {
        //                if (element is JObject)
        //                {
        //                    dictValue = GetSerializer(args[1], JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(element);
        //                }
        //                else if (element is JValue)
        //                {
        //                    dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
        //                }
        //                else
        //                {
        //                    throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
        //                }
        //            }
        //            else if (valueType == JType.Array)
        //            {
        //                if (element is JArray)
        //                {
        //                    dictValue = DeserializeJsonCollectionObject(element as JArray, args[1]);
        //                }
        //                else if (element is JValue)
        //                {
        //                    dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
        //                }
        //                else
        //                {
        //                    throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
        //                }
        //            }
        //            else if (valueType == JType.Value && element is JValue)
        //            {
        //                dictValue = JsonSettings.DeserializeValue(element as JValue, args[1]);
        //            }
        //            else
        //            {
        //                throw new Errors.JsonSerializeFailedException(dictKey, ".net runtime type does not match json type.");
        //            }

        //            instance.Add(dictKey, dictValue);
        //        }

        //        return instance;
        //    }

        //    return null;
        //}

        //private object DeserializeJsonCollectionObject(JArray collectionObject, Type targetType)
        //{
        //    if (targetType.IsArray)
        //    {
        //        var elementType = targetType.GetElementType();
        //        var array = Array.CreateInstance(elementType, collectionObject.Elements.Length);
        //        for (var i = 0; i < array.Length; i++)
        //        {
        //            var jsonObject = collectionObject.Elements[i];
        //            if (jsonObject is JObject)
        //            {
        //                var value = GetSerializer(elementType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(jsonObject);
        //                array.SetValue(value, i);
        //            }
        //            else if (jsonObject is JArray)
        //            {
        //                // TODO: multi-dimension array
        //            }
        //            else if (jsonObject is JValue)
        //            {
        //                var value = JsonSettings.DeserializeValue(jsonObject as JValue, elementType);
        //                array.SetValue(value, i);
        //            }
        //        }

        //        return array;
        //    }
        //    else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
        //    {
        //        var elementType = targetType.GetTypeInfo().GetGenericArguments().FirstOrDefault() ?? typeof(object);
        //        var collection = Activator.CreateInstance(targetType) as IList;
        //        for (int i = 0; i < collectionObject.Elements.Length; i++)
        //        {
        //            var jsonObject = collectionObject.Elements[i];
        //            if (jsonObject is JObject)
        //            {
        //                var value = GetSerializer(elementType, JsonSettings.CurrentEncoding, JsonSettings.OmitDefaultValue).InternalDeserialize(jsonObject);
        //                collection.Add(value);
        //            }
        //            else if (jsonObject is JArray)
        //            {
        //                // TODO: multi-dimension array
        //            }
        //            else if (jsonObject is JValue)
        //            {
        //                var value = JsonSettings.DeserializeValue(jsonObject as JValue, elementType);
        //                collection.Add(value);
        //            }
        //        }

        //        return collection;
        //    }

        //    return null;
        //}

        #endregion
    }
}