using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Guru.ExtensionMethod
{
    public static class StringExtensionMethod
    {
        /// <summary>
        /// Gets the full path relative to current base directory
        /// </summary>
        /// <param name="stringValue">relative path value</param>
        /// <returns>return full path with slash '/' as path seperator</returns>
        public static string FullPath(this string stringValue)
        {
            return stringValue.FullPath(AppContext.BaseDirectory);
        }

        /// <summary>
        /// Gets the full path relative to the specified path
        /// </summary>
        /// <param name="stringValue">relative path value</param>
        /// <param name="relativePath">specified path value</param>
        /// <returns>return full path with slash '/' as path seperator</returns>
        public static string FullPath(this string stringValue, string specifiedPath)
        {
            var fields = stringValue.Replace('\\', '/').SplitByChar('/');
            if (fields.Length == 0)
            {
                return string.Empty;
            }

            var pathParts = new string[0];

            if (fields[0].Equals(".", StringComparison.OrdinalIgnoreCase) || fields[0].Equals("..", StringComparison.OrdinalIgnoreCase))
            {
                if (specifiedPath.HasValue())
                {
                    pathParts = pathParts.Append(specifiedPath.Replace('\\', '/').SplitByChar('/'));
                }
            }

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Equals(".", StringComparison.OrdinalIgnoreCase))
                {
                }
                else if (fields[i].Equals("..", StringComparison.OrdinalIgnoreCase))
                {
                    if (pathParts.Length == 0)
                    {
                        throw new Exception(string.Format("path '{0}' and '{1}' is not valid.", stringValue, specifiedPath ?? string.Empty));
                    }

                    pathParts = pathParts.Subset(0, pathParts.Length - 1);
                }
                else
                {
                    pathParts = pathParts.Append(fields[i]);
                }
            }

            var fullPath = string.Join("/", pathParts);
            if (Regex.IsMatch(fullPath, "^[A-Z]\x3A\x2F", RegexOptions.IgnoreCase))
            {
                // for Windows
                return fullPath;
            }
            else
            {
                // for linux
                return "/" + fullPath;
            }
        }

        public static string Folder(this string stringValue)
        {
            var fullPath = stringValue.FullPath();

            var index = fullPath.LastIndexOf('/');
            if (index == -1)
            {
                return string.Empty;
            }

            return fullPath.Substring(0, index);
        }

        public static string Name(this string stringValue)
        {
            var fullPath = stringValue.FullPath();

            var index = fullPath.LastIndexOf('/');
            if (index == -1 || index == (fullPath.Length - 1))
            {
                return string.Empty;
            }

            return fullPath.Substring(index + 1);
        }

        public static string[] SplitByChar(this string stringValue, char seperator)
        {
            if (stringValue == null)
            {
                return null;
            }

            return stringValue.Split(seperator).Subset(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());
        }

        public static bool HasValue(this string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue) && !string.IsNullOrWhiteSpace(stringValue);
        }

        public static bool IsFile(this string stringValue)
        {
            return File.Exists(stringValue);
        }

        public static bool IsFolder(this string stringValue)
        {
            return Directory.Exists(stringValue);
        }

        public static bool EqualsWith(this string stringValue, string another)
        {
            return string.Equals(stringValue, another, StringComparison.OrdinalIgnoreCase);
        }

        public static T ToEnum<T>(this string stringValue)
        {
            return (T)ToEnum(stringValue, typeof(T));
        }

        public static object ToEnum(this string stringValue, Type targetType)
        {
            return Enum.Parse(targetType, stringValue, true);
        }

        public static bool EqualsWithPath(this string stringValue, string anotherPath)
        {
            if (!stringValue.HasValue() || !anotherPath.HasValue())
            {
                return false;
            }

            return stringValue.FullPath().EqualsWith(anotherPath.FullPath());
        }
    }
}