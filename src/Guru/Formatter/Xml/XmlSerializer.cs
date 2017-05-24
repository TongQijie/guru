using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

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

        public XmlClass XmlClass { get; private set; }

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
                        if (XmlType == XType.Object)
                        {
                            foreach (var propertyInfo in Type.GetTypeInfo().GetProperties().Where(x => x.CanRead && x.CanWrite && !x.IsDefined(typeof(XmlIgnoreAttribute))))
                            {
                                XmlProperty xmlProperty = null;

                                var attribute = propertyInfo.GetCustomAttribute<XmlPropertyAttribute>();
                                if (attribute == null)
                                {
                                    xmlProperty = new XmlProperty(propertyInfo, null, false, false, null);
                                }
                                else
                                {
                                    xmlProperty = new XmlProperty(propertyInfo, attribute.Alias, attribute.IsAttr, attribute.IsArrayElement, attribute.ArrayElementName);
                                }

                                XmlProperties.TryAdd(xmlProperty.Key, xmlProperty);
                            }

                            var xmlClassAttr = Type.GetTypeInfo().GetCustomAttribute<XmlClassAttribute>();
                            if (xmlClassAttr != null)
                            {
                                XmlClass = new XmlClass(Type, xmlClassAttr.RootName);
                            }
                        }
                        else if (XmlType == XType.Array)
                        {
                            var attribute = Type.GetTypeInfo().GetCustomAttribute<XmlClassAttribute>();
                            if (attribute != null)
                            {
                                XmlClass = new XmlClass(Type, attribute.ArrayElementName, attribute.ArrayElementType, attribute.RootName);
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

        public void Serialize(object instance, Stream stream)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var bufferedStream = new BufferedWriterStream(stream, 8 * 1024);

            InternalSerializeXmlObjectAsync(bufferedStream, instance, XmlClass == null ? instance.GetType().Name : XmlClass.Key).GetAwaiter().GetResult();

            bufferedStream.EndWrite().GetAwaiter().GetResult();
        }

        public async Task SerializeAsync(object instance, Stream stream)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var bufferedStream = new BufferedWriterStream(stream, 8 * 1024);

            await InternalSerializeXmlObjectAsync(bufferedStream, instance, XmlClass == null ? instance.GetType().Name : XmlClass.Key);

            await bufferedStream.EndWrite();
        }

        private async Task InternalSerializeXmlObjectAsync(IWriterStream stream, object instance, string tagName)
        {
            await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"<{tagName}"));

            var attributes = XmlProperties.Values.ToArray().Subset(x => x.IsAttr);
            foreach (var attribute in attributes)
            {
                var value = attribute.PropertyInfo.GetValue(instance);
                await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($" {attribute.Key}=\"{value}\""));
            }

            await stream.WriteAsync(XmlConstants.Gt);

            var propertis = XmlProperties.Values.ToArray().Subset(x => !x.IsAttr);
            foreach (var property in propertis)
            {
                var value = property.PropertyInfo.GetValue(instance);
                if (value == null)
                {
                    continue;
                }

                var xmlType = property.XmlType;
                if (xmlType == XType.Dynamic)
                {
                    xmlType = XmlUtility.GetXmlType(value.GetType());
                }

                if (xmlType == XType.Object)
                {
                    await GetSerializer(value.GetType(), XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalSerializeXmlObjectAsync(stream, value, property.Key);
                }
                else if (xmlType == XType.Array)
                {
                    await InternalSerializeXmlArrayAsync(stream, value, property.IsArrayElement ? null : property.Key, property.ArrayElementName);
                }
                else if (xmlType == XType.Value)
                {
                    await InternalSerializeXmlValueAsync(stream, value, property.Key);
                }
            }

            await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"</{tagName}>"));
        }

        private async Task InternalSerializeXmlArrayAsync(IWriterStream stream, object array, string outerTagName, string innerTagName)
        {
            if (outerTagName.HasValue())
            {
                await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"<{outerTagName}>"));
            }

            var collection = array as ICollection;
            foreach (var element in collection)
            {
                var elementType = element.GetType();
                var xmlType = XmlUtility.GetXmlType(elementType);

                if (xmlType == XType.Object)
                {
                    await GetSerializer(elementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalSerializeXmlObjectAsync(stream, element, innerTagName);
                }
                else if (xmlType == XType.Value)
                {
                    await InternalSerializeXmlValueAsync(stream, element, innerTagName);
                }
            }

            if (outerTagName.HasValue())
            {
                await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"</{outerTagName}>"));
            }
        }

        private async Task InternalSerializeXmlValueAsync(IWriterStream stream, object value, string tagName)
        {
            await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"<{tagName}>"));

            await stream.WriteAsync(XmlSettings.SerializeValue(value));

            await stream.WriteAsync(XmlSettings.CurrentEncoding.GetBytes($"</{tagName}>"));
        }

        #endregion

        #region Deserialization

        public object Deserialize(Stream stream)
        {
            return InternalDeserialize(XmlParser.ParseAsync(new Internal.BufferedReaderStream(stream, 8 * 1024)).GetAwaiter().GetResult());
        }

        public async Task<object> DeserializeAsync(Stream stream)
        {
            return InternalDeserialize(await XmlParser.ParseAsync(new Internal.BufferedReaderStream(stream, 8 * 1024)));
        }

        private object InternalDeserialize(XObject xObject)
        {
            var instance = Activator.CreateInstance(Type);

            if (!xObject.Elements.HasLength())
            {
                return instance;
            }

            foreach (var kv in xObject.Elements.OfType<XAttribute>().GroupBy(x => XmlSettings.CurrentEncoding.GetString(x.Key)))
            {
                if (kv.Count() == 1 && XmlProperties.ContainsKey(kv.Key))
                {
                    var property = XmlProperties[kv.Key];
                    if (property.IsAttr)
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

                    if (property.XmlType == XType.Object && kv.Count() == 1)
                    {
                        property.PropertyInfo.SetValue(instance, DeserializeObject(kv.First(), property.PropertyInfo.PropertyType));
                    }
                    else if (property.XmlType == XType.Array || property.ArrayElementType != null)
                    {
                        if (property.IsArrayElement)
                        {
                            property.PropertyInfo.SetValue(instance, DeserializeArray(kv.ToArray(), property.PropertyInfo.PropertyType, property.ArrayElementType));
                        }
                        else
                        {
                            if (kv.Count() != 1)
                            {
                                throw new Exception("");
                            }

                            property.PropertyInfo.SetValue(instance, DeserializeArray(kv.First(), property.PropertyInfo.PropertyType, property.ArrayElementName, property.ArrayElementType));
                        }
                    }
                    else if (property.XmlType == XType.Value && kv.Count() == 1)
                    {
                        property.PropertyInfo.SetValue(instance, DeserializeValue(kv.First(), property.PropertyInfo.PropertyType));
                    }
                }

                if (XmlClass != null && XmlClass.Key == kv.Key)
                {
                    if (XmlClass.XmlType == XType.Array)
                    {
                        return DeserializeArray(kv.ToArray(), XmlClass.ClassType, XmlClass.ArrayElementType);
                    }
                }
            }

            return instance;
        }

        private object DeserializeObject(XObject xObject, Type targetType)
        {
            return GetSerializer(targetType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObject);
        }

        private object DeserializeArray(XObject xObject, Type targetType, string elementName, Type elementType)
        {
            if (!xObject.Elements.HasLength())
            {
                if (targetType.IsArray)
                {
                    return Array.CreateInstance(elementType, 0);
                }
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    return Activator.CreateInstance(targetType) as IList;
                }
                else
                {
                    throw new Exception("");
                }
            }

            return DeserializeArray(xObject.Elements.OfType<XObject>().Where(x => XmlSettings.CurrentEncoding.GetString(x.Key) == elementName).ToArray(), targetType, elementType);
        }

        private object DeserializeArray(XObject[] xObjects, Type targetType, Type elementType)
        {
            var xmlType = XmlUtility.GetXmlType(elementType);
            if (xmlType == XType.Object)
            {
                if (targetType.IsArray)
                {
                    if (!xObjects.HasLength())
                    {
                        return Array.CreateInstance(elementType, 0);
                    }

                    var array = Array.CreateInstance(elementType, xObjects.Length);
                    for (var i = 0; i < array.Length; i++)
                    {
                        array.SetValue(GetSerializer(elementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObjects[i]), i);
                    }
                    return array;
                }
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    if (!xObjects.HasLength())
                    {
                        return Activator.CreateInstance(targetType);
                    }

                    var collection = Activator.CreateInstance(targetType) as IList;
                    for (int i = 0; i < xObjects.Length; i++)
                    {
                        collection.Add(GetSerializer(elementType, XmlSettings.CurrentEncoding, XmlSettings.OmitDefaultValue).InternalDeserialize(xObjects[i]));
                    }
                    return collection;
                }
            }
            else if (xmlType == XType.Value)
            {
                if (targetType.IsArray)
                {
                    if (!xObjects.HasLength())
                    {
                        return Array.CreateInstance(elementType, 0);
                    }

                    var array = Array.CreateInstance(elementType, xObjects.Length);
                    for (var i = 0; i < array.Length; i++)
                    {
                        array.SetValue(DeserializeValue(xObjects[i], elementType), i);
                    }
                    return array;
                }
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    if (!xObjects.HasLength())
                    {
                        return Activator.CreateInstance(targetType);
                    }

                    var collection = Activator.CreateInstance(targetType) as IList;
                    for (int i = 0; i < xObjects.Length; i++)
                    {
                        collection.Add(DeserializeValue(xObjects[i], elementType));
                    }
                    return collection;
                }
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

            if (values[0] is XData)
            {
                return XmlSettings.CurrentEncoding.GetString((values[0] as XData).Value).ConvertTo(targetType);
            }
            else
            {
                return WebUtility.HtmlDecode(XmlSettings.CurrentEncoding.GetString((values[0] as XValue).Value)).ConvertTo(targetType);
            }
        }

        #endregion
    }
}