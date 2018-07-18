using System;

using Guru.ExtensionMethod;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    internal class StaticFileDependencyDescriptor : IDependencyDescriptor
    {
        public StaticFileDependencyDescriptor(Type implementationType, string path, string format, bool multifiles)
        {
            ImplementationType = implementationType;
            Path = path;

            if (format.EqualsIgnoreCase("json"))
            {
                Format = StaticFileFormat.Json;
            }
            else if (format.EqualsIgnoreCase("xml"))
            {
                Format = StaticFileFormat.Xml;
            }
            else if (format.EqualsIgnoreCase("hjson"))
            {
                Format = StaticFileFormat.Hjson;
            }
            else
            {
                Format = StaticFileFormat.Json;
            }

            MultiFiles = multifiles;
        }

        public Type ImplementationType { get; private set; }

        public string Path { get; set; }

        public StaticFileFormat Format { get; set; }

        public bool MultiFiles { get; set; }

        public Lifetime Lifetime => Lifetime.Singleton;

        public int Priority => 0;

        public enum StaticFileFormat
        {
            Json = 0,

            Xml = 1,

            Hjson = 2,
        }
    }
}