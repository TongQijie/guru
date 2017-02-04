using System.Reflection;

using Guru.ExtensionMethod;

namespace Guru.EntityFramework.Internal
{
    internal class SimpleValuePropertyInfo
    {
        public SimpleValuePropertyInfo(PropertyInfo propertyInfo, SimpleValueAttribute attribute)
        {
            PropertyInfo = propertyInfo;
            ColumnName = attribute.Name.HasValue() ? attribute.Name : PropertyInfo.Name;
        }

        public PropertyInfo PropertyInfo { get; private set; }

        public string ColumnName { get; set; }
    }
}