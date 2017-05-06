using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;

using Guru.ExtensionMethod;
using Guru.Logging.Abstractions;
using Guru.Monitor.Abstractions;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementations
{
    internal class StaticFileImplementationResolver : IImplementationResolver
    {
        private readonly IFileSystemMonitor _FileSystemMonitor;

        private readonly IFormatter _Formatter;

        private readonly ILogger _Logger;

        public StaticFileImplementationResolver(IImplementationDecorator decorator)
        {
            Decorator = decorator;

            _FileSystemMonitor = ContainerManager.Default.GetImplementation(typeof(IFileSystemMonitor)) as IFileSystemMonitor;

            _Logger = ContainerManager.Default.GetImplementation(typeof(IFileLogger)) as IFileLogger;

            var dec = decorator as StaticFileImplementationDecorator;

            if (dec.Format == StaticFileImplementationDecorator.StaticFileFormat.Json)
            {
                _Formatter = ContainerManager.Default.GetImplementation(typeof(IJsonFormatter)) as IJsonFormatter;
            }
            else if (dec.Format == StaticFileImplementationDecorator.StaticFileFormat.Xml)
            {
                _Formatter = ContainerManager.Default.GetImplementation(typeof(IXmlFormatter)) as IXmlFormatter;
            }

            if (dec.MultiFiles)
            {
                _PathExpression = $"^{dec.Path.Name().Replace("*", @"\S+")}$";
            }
        }

        public IImplementationDecorator Decorator { get; private set; }

        private object _SingletonObject = null;

        private object _SyncLocker = new object();

        private bool _IsInitialized = false;

        private bool _IsDirty = true;

        private string _PathExpression = string.Empty;

        public object Resolve()
        {
            var decorator = Decorator as StaticFileImplementationDecorator;

            if (!_IsInitialized)
            {
                _FileSystemMonitor.Add(this, decorator.Path.Folder(), OnFileChanged, OnFileChanged, OnFileChanged, OnFileRenamed);
                _IsInitialized = true;
            }
            
            if (_IsDirty)
            {
                lock (_SyncLocker)
                {
                    if (_IsDirty)
                    {
                        var retries = 3;
                        while (_IsDirty && retries > 0)
                        {
                            try
                            {
                                if (!decorator.MultiFiles)
                                {
                                    if (!decorator.Path.FullPath().IsFile())
                                    {
                                        _SingletonObject = decorator.ImplementationType.CreateInstance();
                                    }

                                    _SingletonObject = _Formatter.ReadObject(decorator.ImplementationType, decorator.Path.FullPath());
                                }
                                else
                                {
                                    var collection = Activator.CreateInstance(decorator.ImplementationType) as IList;

                                    var folder = decorator.Path.Folder();
                                    foreach (var fileInfo in new DirectoryInfo(folder).GetFiles())
                                    {
                                        if (Regex.IsMatch(fileInfo.Name, _PathExpression))
                                        {
                                            var elements = _Formatter.ReadObject(decorator.ImplementationType, fileInfo.FullName) as IList;
                                            foreach (var element in elements)
                                            {
                                                collection.Add(element);
                                            }
                                        }
                                    }

                                    _SingletonObject = collection;
                                }

                                _IsDirty = false;
                            }
                            catch (Exception e)
                            {
                                _Logger.LogEvent("error", Severity.Error, e);

                                if (retries > 1)
                                {
                                    Thread.Sleep(100);
                                }
                            }

                            retries--;
                        }
                    }
                }
            }

            return _SingletonObject;
        }

        private void OnFileChanged(string path)
        {
            var filename = path.Name();
            if (Regex.IsMatch(filename, _PathExpression))
            {
                _IsDirty = true;
            }
        }

        private void OnFileRenamed(string oldPath, string newPath)
        {
            var filename = newPath.Name();
            if (Regex.IsMatch(filename, _PathExpression))
            {
                _IsDirty = true;
            }
        }
    }
}