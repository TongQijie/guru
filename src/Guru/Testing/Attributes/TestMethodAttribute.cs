using System;

namespace Guru.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodAttribute : Attribute
    {
        public TestMethodAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}