using System.Reflection;
using Guru.ExtensionMethod;
using System;
using System.Collections;

namespace Guru.Formatter.Xml
{
    internal class XmlProperty
    {
        public XmlProperty(PropertyInfo propertyInfo, string alias, bool attribute, bool array, bool arrayItem, string arrayItemAlias)
        {
            PropertyInfo = propertyInfo;
            Alias = alias;
            Attribute = attribute;
            Array = array;
            ArrayItem = arrayItem;
            ArrayItemAlias = arrayItemAlias;

            DefaultValue = propertyInfo.PropertyType.GetDefaultValue();
            XmlType = XmlUtility.GetXmlType(propertyInfo.PropertyType);

            if (XmlType == XType.Array)
            {
                if (propertyInfo.PropertyType.IsArray)
                {
                    ArrayElementType = propertyInfo.PropertyType.GetElementType();
                }
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    ArrayElementType = propertyInfo.PropertyType.GetTypeInfo().GetGenericArguments().FirstOrDefault();
                }
                else
                {
                    throw new Exception("");
                }

                if (!ArrayItemAlias.HasValue())
                {
                    ArrayItemAlias = ArrayElementType.Name;
                }
            }
        }

        public string Key
        {
            get
            {
                if (ArrayItem)
                {
                    return ArrayItemAlias.HasValue() ? ArrayItemAlias : ArrayElementType?.Name;
                }

                return Alias.HasValue() ? Alias : PropertyInfo.Name;
            }
        }

        public PropertyInfo PropertyInfo { get; private set; }

        public XType XmlType { get; private set; }

        public object DefaultValue { get; private set; }

        public string Alias { get; private set; }

        public bool Attribute { get; private set; }

        public bool Array { get; private set; }

        public bool ArrayItem { get; private set; }

        public string ArrayItemAlias { get; private set; }

        public Type ArrayElementType { get; private set; }
    }
}