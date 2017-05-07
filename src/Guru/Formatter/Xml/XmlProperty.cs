using System.Reflection;
using Guru.ExtensionMethod;

namespace Guru.Formatter.Xml
{
    internal class XmlProperty
    {
        public XmlProperty(PropertyInfo propertyInfo, string alias)
        {
            PropertyInfo = propertyInfo;
            Alias = alias;

            DefaultValue = propertyInfo.PropertyType.GetDefaultValue();
            XmlType = XmlUtility.GetXmlType(propertyInfo.PropertyType);
        }

        public string Key { get { return Alias.HasValue() ? Alias : PropertyInfo.Name; } }

        public PropertyInfo PropertyInfo { get; private set; }

        public XType XmlType { get; private set; }

        public object DefaultValue { get; private set; }

        public string Alias { get; private set; }
    }
}