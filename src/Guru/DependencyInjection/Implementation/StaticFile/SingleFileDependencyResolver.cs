using Guru.DependencyInjection.Abstractions;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.Logging;
using Guru.Logging.Abstractions;
using Guru.Monitor.Abstractions;
using Guru.Utils;
using System;
using System.IO;
using System.Threading;

namespace Guru.DependencyInjection.Implementation.StaticFile
{
    class SingleFileDependencyResolver : IDependencyResolver
    {
        private readonly IFileSystemMonitor _FileSystemMonitor;

        private readonly ILightningFormatter _Formatter;

        private readonly ILogger _Logger;

        public SingleFileDependencyResolver(IDependencyDescriptor descriptor)
        {
            Descriptor = descriptor;

            _FileSystemMonitor = DependencyContainer.Resolve<IFileSystemMonitor>();

            _Logger = DependencyContainer.Resolve<IFileLogger>();

            var dec = descriptor as SingleFileDependencyDescriptor;

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
        }

        public IDependencyDescriptor Descriptor { get; private set; }

        private object _SingletonObject = null;

        private object _SyncLocker = new object();

        private bool _IsInitialized = false;

        private bool _IsDirty = true;

        public object Resolve()
        {
            var decorator = Descriptor as SingleFileDependencyDescriptor;

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
                                if (!decorator.Path.FullPath().IsFile())
                                {
                                    _SingletonObject = decorator.ImplementationType.CreateInstance();
                                }
                                else
                                {
                                    using (var inputStream = new FileStream(decorator.Path.FullPath(), FileMode.Open, FileAccess.Read))
                                    {
                                        if (decorator.Format == FileFormatEnum.Hjson)
                                        {
                                            _SingletonObject = _Formatter.ReadObject(decorator.ImplementationType, HjsonUtils.ToJson(inputStream));
                                        }
                                        else
                                        {
                                            _SingletonObject = _Formatter.ReadObject(decorator.ImplementationType, inputStream);
                                        }
                                    }
                                }

                                _IsDirty = false;
                            }
                            catch (Exception e)
                            {
                                _Logger.LogEvent(nameof(SingleFileDependencyResolver), Severity.Error, $"failed to resolve single file. {decorator.Path}", e);

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
            _IsDirty = (Descriptor as SingleFileDependencyDescriptor).Path.Name().EqualsIgnoreCase(path.Name());
        }

        private void OnFileRenamed(string oldPath, string newPath)
        {
            _IsDirty = (Descriptor as SingleFileDependencyDescriptor).Path.Name().EqualsIgnoreCase(newPath.Name());
        }
    }
}