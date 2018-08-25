using System;

namespace Guru.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttribute : Attribute
    {
        public TestClassAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}