using System;
using System.Reflection;
using System.Collections;

namespace Guru.Formatter.Json
{
    internal static class JsonUtility
    {
        public static JType GetJsonObjectType(Type type)
        {
            if (type == typeof(object))
            {
                return JType.Dynamic;
            }
            else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type))
            {
                return JType.Map;
            }
            else if (typeof(ICollection).GetTypeInfo().IsAssignableFrom(type))
            {
                return JType.Array;
            }
            else if (type.GetTypeInfo().IsClass && type != typeof(string))
            {
                return JType.Object;
            }
            else
            {
                return JType.Value;
            }
        }
    }
}