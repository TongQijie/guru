using System;
using System.IO;
using System.Reflection;
using Guru.DependencyInjection.Abstractions;
using Guru.DependencyInjection.Configuration;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;

namespace Guru.DependencyInjection.Implementation.Configurable
{
    internal class ConfigurableDependencyRegister : IDependencyRegister
    {
        private readonly ILightningFormatter _Formatter;

        private readonly IFileLogger _Logger;

        public ConfigurableDependencyRegister()
        {
            _Formatter = DependencyContainer.Resolve<IJsonLightningFormatter>();
            _Logger = DependencyContainer.Resolve<IFileLogger>();
        }

        public IContainerInstance Register(IContainerInstance instance)
        {
            if (!"./Configuration".IsFolder())
            {
                return instance;
            }

            var configFiles = new DirectoryInfo("./Configuration".FullPath()).GetFiles("*.deps");
            foreach (var configFile in configFiles)
            {
                try
                {
                    DependencyConfiguration[] dependencies;
                    using (var inputStream = new FileStream(configFile.FullName, FileMode.Open, FileAccess.Read))
                    {
                        dependencies = _Formatter.ReadObject<DependencyConfiguration[]>(inputStream);
                    }

                    if (!dependencies.HasLength())
                    {
                        continue;
                    }

                    foreach (var dependency in dependencies)
                    {
                        InternalRegister(instance, dependency);
                    }
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(ConfigurableDependencyRegister), Severity.Error, $"failed to resolve file '{configFile.FullName}'", e);
                }
            }

            return instance;
        }

        public IContainerInstance Register(IContainerInstance instance, Assembly assembly)
        {
            throw new NotImplementedException();
        }

        private void InternalRegister(IContainerInstance instance, DependencyConfiguration dependency)
        {
            if (dependency == null ||
                string.IsNullOrEmpty(dependency.Name) ||
                string.IsNullOrEmpty(dependency.Type))
            {
                return;
            }

            var targetType = Type.GetType(dependency.Type, false);
            if (targetType == null)
            {
                _Logger.LogEvent(nameof(ConfigurableDependencyRegister), Severity.Error, $"failed to find type '{dependency.Type}'");
                return;
            }

            var descriptor = new ConfigurableDependencyDescriptor(targetType, dependency.Lifetime, dependency.Priority);
            if (dependency.Properties != null && dependency.Properties.Length > 0)
            {
                foreach (var property in dependency.Properties)
                {
                    descriptor.SetProperty(property.Name, property.Value);
                }
            }

            instance.Add(dependency.Name, new ConfigurableDependencyResolver(descriptor));
        }
    }
}