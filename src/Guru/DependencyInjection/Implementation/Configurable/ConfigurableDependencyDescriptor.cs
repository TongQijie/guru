using Guru.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Guru.DependencyInjection.Implementation.Configurable
{
    internal class ConfigurableDependencyDescriptor : DefaultDependencyDescriptor
    {
        private List<PropertySetter> _PropertySetters = new List<PropertySetter>();

        public List<PropertySetter> PropertySetters => _PropertySetters;

        public ConfigurableDependencyDescriptor(Type implementationType, Lifetime lifetime, int priority)
            : base(implementationType, lifetime, priority)
        {
        }

        public void SetProperty(string name, string value)
        {
            var propertyInfo = ImplementationType.GetProperty(name);
            if (propertyInfo == null)
            {
                return;
            }

            try
            {
                var propertyValue = value.ConvertTo(propertyInfo.PropertyType);
                _PropertySetters.Add(new PropertySetter()
                {
                    Name = name,
                    Value = value,
                    PropertyInfo = propertyInfo,
                    PropertyValue = propertyValue,
                });
            }
            catch (Exception) { }
        }

        public class PropertySetter
        {
            public string Name { get; set; }

            public string Value { get; set; }

            public object PropertyValue { get; set; }

            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}