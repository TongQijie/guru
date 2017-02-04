using System;

namespace Guru.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CompositeValueAttribute : Attribute
    {
        public CompositeValueAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; set; }
    }
}