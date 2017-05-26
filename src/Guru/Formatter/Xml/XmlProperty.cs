using System;
using System.Reflection;
using System.Collections;

using Guru.ExtensionMethod;

namespace Guru.Formatter.Xml
{
    internal class XmlProperty
    {
        public XmlProperty(PropertyInfo propertyInfo, string alias, bool isAttr, bool isArrayItem, string arrayItemName)
        {
            PropertyInfo = propertyInfo;
            Alias = alias;
            IsAttr = isAttr;
            IsArrayElement = isArrayItem;
            ArrayElementName = arrayItemName;

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

                }

                if (!ArrayElementName.HasValue())
                {
                    ArrayElementName = ArrayElementType.Name;
                }
            }
        }

        public string Key
        {
            get
            {
                if (IsArrayElement)
                {
                    return ArrayElementName.HasValue() ? ArrayElementName : ArrayElementType?.Name;
                }

                return Alias.HasValue() ? Alias : PropertyInfo.Name;
            }
        }

        public PropertyInfo PropertyInfo { get; private set; }

        public XType XmlType { get; private set; }

        public object DefaultValue { get; private set; }

        public string Alias { get; private set; }

        public bool IsAttr { get; private set; }

        public bool IsArrayElement { get; private set; }

        public string ArrayElementName { get; private set; }

        public Type ArrayElementType { get; private set; }
    }
}