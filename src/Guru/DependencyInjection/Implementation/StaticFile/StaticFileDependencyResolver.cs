using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

using Guru.ExtensionMethod;
using Guru.Logging.Abstractions;
using Guru.Monitor.Abstractions;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    internal class StaticFileDependencyResolver : IDependencyResolver
    {
        private readonly IFileSystemMonitor _FileSystemMonitor;

        private readonly IFormatter _Formatter;

        private readonly ILogger _Logger;

        public StaticFileDependencyResolver(IDependencyDescriptor descriptor)
        {
            Descriptor = descriptor;

            _FileSystemMonitor = DependencyContainer.Resolve<IFileSystemMonitor>();

            _Logger = DependencyContainer.Resolve<IFileLogger>();

            var dec = descriptor as StaticFileDependencyDescriptor;

            if (dec.Format == StaticFileDependencyDescriptor.StaticFileFormat.Json)
            {
                _Formatter = DependencyContainer.Resolve<IJsonFormatter>();
            }
            else if (dec.Format == StaticFileDependencyDescriptor.StaticFileFormat.Xml)
            {
                _Formatter = DependencyContainer.Resolve<IXmlFormatter>();
            }

            if (dec.MultiFiles)
            {
                _PathExpression = $"^{dec.Path.Name().Replace("*", @"\S+")}$";
            }
        }

        public IDependencyDescriptor Descriptor { get; private set; }

        private object _SingletonObject = null;

        private object _SyncLocker = new object();

        private bool _IsInitialized = false;

        private bool _IsDirty = true;

        private string _PathExpression = string.Empty;

        public object Resolve()
        {
            var decorator = Descriptor as StaticFileDependencyDescriptor;

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

                                    var collectionType = decorator.ImplementationType;
                                    Type elementType = null;
                                    while ((elementType = collectionType.GetTypeInfo().GetGenericArguments().FirstOrDefault()) == null)
                                    {
                                        collectionType = collectionType.GetTypeInfo().BaseType;
                                    }

                                    var folder = decorator.Path.Folder();
                                    foreach (var fileInfo in new DirectoryInfo(folder).GetFiles())
                                    {
                                        if (Regex.IsMatch(fileInfo.Name, _PathExpression))
                                        {
                                            var elements = _Formatter.ReadObject(collectionType, fileInfo.FullName) as IList;
                                            if (elements == null || elements.Count == 0)
                                            {
                                                continue;
                                            }
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
                                _Logger.LogEvent(nameof(StaticFileDependencyResolver), Severity.Error, $"failed to resolve static file. {decorator.Path}", e);

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