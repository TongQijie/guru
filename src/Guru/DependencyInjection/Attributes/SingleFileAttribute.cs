using System;

namespace Guru.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleFileAttribute : Attribute
    {
        public SingleFileAttribute(Type abstraction, string path, FileFormatEnum format)
        {
            Abstraction = abstraction;
            Path = path;
            Format = format;
        }

        public Type Abstraction { get; set; }

        public string Path { get; set; }

        public FileFormatEnum Format { get; set; }
    }
}