using System;
using System.Reflection;
using System.Collections;

namespace Guru.Formatter.Xml
{
    internal static class XmlUtility
    {
        public static XType GetXmlType(Type type)
        {
            if (type == typeof(object))
            {
                return XType.Dynamic;
            }
            else if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(type))
            {
                return XType.Array;
            }
            else if (type.GetTypeInfo().IsClass && type != typeof(string))
            {
                return XType.Object;
            }
            else
            {
                return XType.Value;
            }
        }
    }
}
