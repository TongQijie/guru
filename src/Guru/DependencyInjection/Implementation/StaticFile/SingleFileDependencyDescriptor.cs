using Guru.DependencyInjection.Abstractions;
using System;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    internal class SingleFileDependencyDescriptor : IDependencyDescriptor
    {
        public SingleFileDependencyDescriptor(Type implementationType, string path, FileFormatEnum format)
        {
            ImplementationType = implementationType;
            Path = path;
            Format = format;
        }

        public Type ImplementationType { get; private set; }

        public string Path { get; set; }

        public FileFormatEnum Format { get; set; }

        public Lifetime Lifetime => Lifetime.Singleton;

        public int Priority => 0;
    }
}