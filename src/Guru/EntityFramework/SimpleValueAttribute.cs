using System;

namespace Guru.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SimpleValueAttribute : Attribute
    {
        public SimpleValueAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}