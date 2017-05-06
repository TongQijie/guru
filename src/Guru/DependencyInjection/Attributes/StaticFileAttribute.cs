using System;

namespace Guru.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class StaticFileAttribute : Attribute
    {
        public StaticFileAttribute() { }

        public StaticFileAttribute(Type abstraction, string path)
        {
            Abstraction = abstraction;
            Path = path;
        }

        public Type Abstraction { get; set; }

        public string Path { get; set; }

        public string Format { get; set; }

        public bool MultiFiles { get; set; }
    }
}