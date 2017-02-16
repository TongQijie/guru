using System;
using System.Reflection;

namespace Guru.ExtensionMethod
{
    public static class ObjectExtensionMethod
    {
        public static T GetCustomAttribute<T>(this object obj, Predicate<T> predicate)
        {
            if (typeof(ICustomAttributeProvider).GetTypeInfo().IsAssignableFrom(obj.GetType()))
            {
                var attributes = (obj as ICustomAttributeProvider).GetCustomAttributes(typeof(T), false);
                if (attributes.HasLength())
                {
                    return attributes.Subset(x => x is T).Select(x => (T)x).FirstOrDefault<T>(predicate);
                }
            }

            return default(T);
        }

        public static T ConvertTo<T>(this object obj)
        {
            return (T)ConvertTo(obj, typeof(T));
        }

        public static object ConvertTo(this object obj, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("obj or targetType");
            }

            if (targetType.GetTypeInfo().IsValueType)
            {
                var t = Nullable.GetUnderlyingType(targetType);
                if (t != null)
                {
                    if (obj == null)
                    {
                        return null;
                    }
                    else
                    {
                        targetType = t;
                    }
                }
            }
            else
            {
                if (obj == null)
                {
                    return null;
                }
            }

            if (targetType.GetTypeInfo().IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }
            else if (targetType.GetTypeInfo().IsEnum)
            {
                try
                {
                    return obj.ToString().ToEnum(targetType);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (typeof(IConvertible).GetTypeInfo().IsAssignableFrom(obj.GetType()))
            {
                try
                {
                    return Convert.ChangeType(obj, targetType);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new Exception(string.Format("value {0} fails to convert into type '{1}'.", obj.ToString(), targetType.FullName));
            }
        }

        public static T ConvertTo<T>(this object obj, object defaultValue)
        {
            return (T)ConvertTo(obj, typeof(T), defaultValue);
        }

        public static object ConvertTo(this object obj, Type targetType, object defaultValue)
        {
            try
            {
                return ConvertTo(obj, targetType);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static bool Convertible<T>(this object obj, out T result)
        {
            object r;
            if (Convertible(obj, typeof(T), out r))
            {
                result = (T)r;
                return true;
            }
            else
            {
                result = typeof(T).GetDefaultValue<T>();
                return false;
            }
        }

        public static bool Convertible(this object obj, Type targetType, out object result)
        {
            try
            {
                result = ConvertTo(obj, targetType);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        public static bool EqualsWith(this object obj, object another)
        {
            return (obj == null && another == null)
                || (obj != null && obj.Equals(another))
                || (another != null && another.Equals(obj));
        }

        public static object ShallowCopy(this object obj)
        {
            var type = obj.GetType();

            if (type.GetTypeInfo().IsValueType)
            {
                return obj;
            }
            else if (type == typeof(string))
            {
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    return obj as string;
                }
            }
            else if (type.IsArray)
            {
                var array = obj as Array;

                var copy = Array.CreateInstance(type.GetElementType(), array.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    copy.SetValue(array.GetValue(i), i);
                }

                return copy;
            }
            else
            {
                var copy = type.CreateInstance();

                var fields = type.GetTypeInfo().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    field.SetValue(copy, field.GetValue(obj));
                }

                return copy;
            }

        }

        public static T ShallowCopy<T>(this object obj)
        {
            return (T)obj.ShallowCopy();
        }

        public static object DeepCopy(this object obj)
        {
            var type = obj.GetType();

            if (type.GetTypeInfo().IsValueType)
            {
                return obj;
            }
            else if (type == typeof(string))
            {
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    return obj as string;
                }
            }
            else if (type.IsArray)
            {
                var array = obj as Array;

                var copy = Array.CreateInstance(type.GetElementType(), array.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    copy.SetValue(DeepCopy(array.GetValue(i)), i);
                }

                return copy;
            }
            else
            {
                var copy = type.CreateInstance();

                var fields = type.GetTypeInfo().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType.GetTypeInfo().IsValueType)
                    {
                        field.SetValue(copy, field.GetValue(obj));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        var stringValue = field.GetValue(obj) as string;
                        if (stringValue == null)
                        {
                            field.SetValue(copy, null);
                        }
                        else
                        {
                            field.SetValue(copy, stringValue);
                        }
                    }
                    else
                    {
                        var objectValue = field.GetValue(obj);
                        if (objectValue == null)
                        {
                            field.SetValue(copy, null);
                        }
                        else
                        {
                            field.SetValue(copy, DeepCopy(field.GetValue(obj)));
                        }
                    }
                }

                return copy;
            }
        }

        public static T DeepCopy<T>(this object obj)
        {
            return (T)obj.DeepCopy();
        }
    }
}