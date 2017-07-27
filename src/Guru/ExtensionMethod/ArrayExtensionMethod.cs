using System;

namespace Guru.ExtensionMethod
{
    public static class ArrayExtensionMethod
    {
        public static bool HasLength<T>(this T[] source)
        {
            return source != null && source.Length > 0;
        }

        public static T[] Append<T>(this T[] source, T element)
        {
            T[] buffer;
            if (!source.HasLength<T>())
            {
                buffer = new T[1];
            }
            else
            {
                buffer = new T[source.Length + 1];
                Array.Copy(source, buffer, source.Length);
            }

            buffer[buffer.Length - 1] = element;
            return buffer;
        }

        public static T[] Append<T>(this T[] source, T[] elements)
        {
            if (elements == null)
            {
                return source;
            }

            return source.Append<T>(elements, 0, elements.Length);
        }

        public static T[] Append<T>(this T[] source, T[] elements, int offset, int count)
        {
            if (elements == null || offset < 0 || (offset + count) > elements.Length)
            {
                throw new Exception("");
            }

            T[] buffer;
            if (!source.HasLength<T>())
            {
                buffer = new T[count];
                Array.Copy(elements, offset, buffer, 0, count);
            }
            else
            {
                buffer = new T[source.Length + count];
                Array.Copy(source, buffer, source.Length);
                Array.Copy(elements, offset, buffer, source.Length, count);
            }
            
            return buffer;
        }

        public static bool Exists<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return false;
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (predicate(source[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static T[] Remove<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return new T[0];
            }

            var buffer = new T[0];
            for (int i = 0; i < source.Length; i++)
            {
                if (!predicate(source[i]))
                {
                    buffer = buffer.Append(source[i]);
                }
            }

            return buffer;
        }

        public static bool EqualsWith<T>(this T[] source, T[] target)
        {
            if (source == null || target == null || source.Length != target.Length)
            {
                return false;
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (!source[i].Equals(target[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static int IndexOf<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return -1;
            }

            for (var i = 0; i < source.Length; i++)
            {
                if (predicate(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LastIndexOf<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return -1;
            }

            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (predicate(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static T[] Subset<T>(this T[] source, int offset)
        {
            if (source == null)
            {
                throw new Exception("");
            }

            return Subset(source, offset, source.Length - offset);
        }

        public static T[] Subset<T>(this T[] source, int offset, int count)
        {
            if (source == null || offset < 0 || count < 0)
            {
                throw new Exception("");
            }

            var buffer = new T[Math.Min(count, source.Length - offset)];
            Array.Copy(source, offset, buffer, 0, buffer.Length);
            return buffer;
        }

        public static T[] Subset<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return new T[0];
            }

            var buffer = new T[0];
            for (int i = 0; i < source.Length; i++)
            {
                if (predicate(source[i]))
                {
                    buffer = buffer.Append(source[i]);
                }
            }

            return buffer;
        }

        public static T FirstOrDefault<T>(this T[] source, Predicate<T> predicate)
        {
            if (!source.HasLength<T>())
            {
                return typeof(T).GetDefaultValue<T>();
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (predicate(source[i]))
                {
                    return source[i];
                }
            }

            return typeof(T).GetDefaultValue<T>();
        }

        public static T FirstOrDefault<T>(this T[] source)
        {
            if (!source.HasLength<T>())
            {
                return typeof(T).GetDefaultValue<T>();
            }

            return source[0];
        }

        public static TTarget[] Select<TSource, TTarget>(this TSource[] source, Func<TSource, TTarget> selector)
        {
            if (!source.HasLength<TSource>())
            {
                return new TTarget[0];
            }

            var buffer = new TTarget[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                buffer[i] = selector(source[i]);
            }

            return buffer;
        }

        public static T[] Each<T>(this T[] source, Action<T> func)
        {
            if (!source.HasLength<T>())
            {
                return source;
            }

            for (int i = 0; i < source.Length; i++)
            {
                func(source[i]);
            }

            return source;
        }
    }
}