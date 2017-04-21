using System.Reflection;

using Guru.ExtensionMethod;

namespace Guru.Formatter.Json
{
    internal class JsonProperty
    {
        public JsonProperty(PropertyInfo propertyInfo, string alias, bool isJsonObject)
        {
            PropertyInfo = propertyInfo;
            Alias = alias;
            IsJsonObject = isJsonObject;
            
            DefaultValue = propertyInfo.PropertyType.GetDefaultValue();
            ObjectType = JsonUtility.GetJsonObjectType(propertyInfo.PropertyType);
        }

        public string Key { get { return Alias.HasValue() ? Alias : PropertyInfo.Name; } }

        public PropertyInfo PropertyInfo { get; private set; }

        public JType ObjectType { get; private set; }

        public object DefaultValue { get; private set; }

        public string Alias { get; private set; }

        public bool IsJsonObject { get; private set; }
    }
}