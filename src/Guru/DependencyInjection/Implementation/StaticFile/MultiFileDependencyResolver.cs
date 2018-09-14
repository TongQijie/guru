using Guru.DependencyInjection.Abstractions;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Monitor.Abstractions;
using Guru.Utils;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    internal class MultiFileDependencyResolver : IDependencyResolver
    {
        private readonly IFileSystemMonitor _FileSystemMonitor;

        private readonly ILightningFormatter _Formatter;

        private readonly ILogger _Logger;

        public MultiFileDependencyResolver(IDependencyDescriptor descriptor)
        {
            Descriptor = descriptor;

            _FileSystemMonitor = DependencyContainer.Resolve<IFileSystemMonitor>();

            _Logger = DependencyContainer.Resolve<IFileLogger>();

            var dec = descriptor as MultiFileDependencyDescriptor;

            if (dec.Format == FileFormatEnum.Json || dec.Format == FileFormatEnum.Hjson)
            {
                _Formatter = DependencyContainer.Resolve<IJsonLightningFormatter>();
            }
            else if (dec.Format == FileFormatEnum.Xml)
            {
                _Formatter = DependencyContainer.Resolve<IXmlLightningFormatter>();
            }
            else
            {
                _Formatter = DependencyContainer.Resolve<IJsonLightningFormatter>();
            }

            _PathRegex = $"^{dec.Path.Name().Replace("*", @"\S+")}$";
        }

        public IDependencyDescriptor Descriptor { get; private set; }

        private object _SingletonObject = null;

        private object _SyncLocker = new object();

        private bool _IsInitialized = false;

        private bool _IsDirty = true;

        private string _PathRegex = string.Empty;

        public object Resolve()
        {
            var decorator = Descriptor as MultiFileDependencyDescriptor;

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
                                var collection = Activator.CreateInstance(decorator.ImplementationType) as IList;

                                var folder = decorator.Path.Folder();
                                foreach (var fileInfo in new DirectoryInfo(folder).GetFiles())
                                {
                                    if (Regex.IsMatch(fileInfo.Name, _PathRegex))
                                    {
                                        IList elements = null;
                                        using (var inputStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                                        {
                                            if (decorator.Format == FileFormatEnum.Hjson)
                                            {
                                                elements = _Formatter.ReadObject(decorator.ImplementationType, HjsonUtils.ToJson(inputStream)) as IList;
                                            }
                                            else
                                            {
                                                elements = _Formatter.ReadObject(decorator.ImplementationType, inputStream) as IList;
                                            }
                                        }

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

                                _IsDirty = false;
                            }
                            catch (Exception e)
                            {
                                _Logger.LogEvent(nameof(MultiFileDependencyResolver), Severity.Error, $"failed to resolve multifile. {decorator.Path}", e);

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
            _IsDirty = Regex.IsMatch(path.Name(), _PathRegex);
        }

        private void OnFileRenamed(string oldPath, string newPath)
        {
            _IsDirty = Regex.IsMatch(newPath.Name(), _PathRegex);
        }
    }
}
