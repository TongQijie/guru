using System;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

using Guru.Monitor;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.DependencyInjection
{
    /*********************************
     * IJsonFormatter, IXmlFormatter 
     * IFileSystemMonitor
     *********************************/
    public class FileResolver : IResolver
    {
        private readonly IContainer _Container;
        
        private readonly Type _Abstraction;

        private readonly Type _Implementation;
        
        private readonly string _Path;
        
        private readonly FileFormat _Format;

        private readonly bool _Multiply;
        
        private bool _IsDirty = true;
        
        private object _Sync = new object();

        private bool _IsInit;
        
        private object _SingletonObject;
        
        public FileResolver(IContainer container, Type abstraction, Type implementation, string path, FileFormat format, bool multiply)
        {
            _Container = container;
            _Abstraction = abstraction;
            _Implementation = implementation;
            _Path = path;
            _Format = format;
            _Multiply = multiply;
        }
        
        public Type Abstraction { get { return _Abstraction; } }

        public Type Implementation { get { return _Implementation; } }

        public int Priority { get { return 0; } }

        public Lifetime Lifetime { get { return Lifetime.Singleton; } }
        
        public string RegexPathName { get { return $"^{_Path.Name().Replace("*", @"\S+")}$"; } }
        
        public object Resolve()
        {
            if (!_IsInit)
            {
                _Container.GetImplementation<IFileSystemMonitor>().Add(this, _Path.Folder(), OnFileChanged, null, null, OnFileRenamed);
                _IsInit = true;
            }
            
            if (_IsDirty)
            {
                lock (_Sync)
                {
                    if (_IsDirty)
                    {
                        var retries = 3;
                        while(_IsDirty && retries > 0)
                        {
                            try
                            {
                                if (!_Multiply)
                                {
                                    if (!_Path.FullPath().IsFile())
                                    {
                                        _SingletonObject = _Implementation.CreateInstance();
                                    }
                                    else if (_Format == FileFormat.Json)
                                    {
                                        _SingletonObject = _Container.GetImplementation<IJsonFormatter>().ReadObject(_Implementation, _Path.FullPath());
                                    }
                                    else if (_Format == FileFormat.Xml)
                                    {
                                        _SingletonObject = _Container.GetImplementation<IXmlFormatter>().ReadObject(_Implementation, _Path.FullPath());
                                    }
                                }
                                else
                                {
                                    var objects = new object[0];

                                    var folder = _Path.Folder();
                                    foreach (var fileInfo in new DirectoryInfo(folder).GetFiles())
                                    {
                                        if (Regex.IsMatch(fileInfo.Name, RegexPathName, RegexOptions.IgnoreCase))
                                        {
                                            if (_Format == FileFormat.Json)
                                            {
                                                objects = objects.Append(_Container.GetImplementation<IJsonFormatter>().ReadObject(_Implementation, fileInfo.FullName));
                                            }
                                            else if (_Format == FileFormat.Xml)
                                            {
                                                objects = objects.Append(_Container.GetImplementation<IXmlFormatter>().ReadObject(_Implementation, fileInfo.FullName));
                                            }
                                        }
                                    }

                                    _SingletonObject = objects;
                                }
                                
                                _IsDirty = false;
                            }
                            catch (Exception)
                            {
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
            if (Regex.IsMatch(filename, RegexPathName, RegexOptions.IgnoreCase))
            {
                _IsDirty = true;
            }
        }
        
        private void OnFileRenamed(string oldPath, string newPath)
        {
            var filename = newPath.Name();
            if (Regex.IsMatch(filename, RegexPathName, RegexOptions.IgnoreCase))
            {
                _IsDirty = true;
            }
        }
    }
}