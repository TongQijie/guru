using System;

namespace Guru.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class FileDIAttribute : Attribute
    {
        public FileDIAttribute(Type abstraction, string path)
        {
            Abstraction = abstraction;
            Path = path;
        }

        public Type Abstraction { get; set; }
        
        public string Path { get; set; }
        
        public FileFormat Format { get; set; }

        public bool Multiply { get; set; }
    }
}