using System;

namespace Guru.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestInputAttribute : Attribute
    {
        public object[] InputValues { get; set; }

        public TestInputAttribute(params object[] inputValues)
        {
            InputValues = inputValues;
        }
    }
}
