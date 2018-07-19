using System;
using System.IO;
using System.Collections.Generic;

using Guru.ExtensionMethod;
using Guru.Logging.Abstractions;
using Guru.Logging;

namespace Guru.Monitor.Internal
{
    internal class FolderMonitor
    {
        private readonly ILogger _Logger;

        public FolderMonitor(string folderPath, ILogger logger)
        {
            Path = folderPath;
            _Logger = logger;
        }

        private FileSystemWatcher _Watcher = null;

        public string Path { get; private set; }

        public void Start()
        {
            if (_Watcher == null)
            {
                _Watcher = new FileSystemWatcher();
                _Watcher.Path = Path;
                _Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                _Watcher.Changed += new FileSystemEventHandler(OnChanged);
                _Watcher.Created += new FileSystemEventHandler(OnCreated);
                _Watcher.Deleted += new FileSystemEventHandler(OnDeleted);
                _Watcher.Renamed += new RenamedEventHandler(OnRenamed);
                _Watcher.Error += new ErrorEventHandler(OnError);
            }

            _Watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (_Watcher != null)
            {
                _Watcher.Changed -= new FileSystemEventHandler(OnChanged);
                _Watcher.Created -= new FileSystemEventHandler(OnCreated);
                _Watcher.Deleted -= new FileSystemEventHandler(OnDeleted);
                _Watcher.Renamed -= new RenamedEventHandler(OnRenamed);
                _Watcher.Error -= new ErrorEventHandler(OnError); 
                _Watcher.EnableRaisingEvents = false;
                _Watcher = null;
            }
        }

        public object[] ReferencedObjects = new object[0];

        public event Delegates.FileChangedHandlerDelegate FileChanged;

        public event Delegates.FileCreatedHandlerDelegate FileCreated;

        public event Delegates.FileDeletedHandlerDelegate FileDeleted;

        public event Delegates.FileRenamedHandlerDelegate FileRenamed;

        private Dictionary<string, DateTime> ChangeTimes = new Dictionary<string, DateTime>();

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                DateTime lastWriteTime = new DateTime();
                if (e.FullPath.IsFile())
                {
                    lastWriteTime = new FileInfo(e.FullPath).LastWriteTime;
                }
                else if (e.FullPath.IsFolder())
                {
                    lastWriteTime = new DirectoryInfo(e.FullPath).LastWriteTime;
                }

                if (ChangeTimes.ContainsKey(e.FullPath))
                {
                    if (lastWriteTime - ChangeTimes[e.FullPath] > TimeSpan.FromMilliseconds(1))
                    {
                        try
                        {
                            FileChanged.Invoke(e.FullPath.FullPath());
                        }
                        catch (Exception ex)
                        {
                            _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, $"error occurred when '{e.FullPath}' changed.", ex);
                        }
                    }
                }
                else
                {
                    try
                    {
                        FileChanged.Invoke(e.FullPath.FullPath());
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, $"error occurred when '{e.FullPath}' changed.", ex);                        
                    }
                }

                ChangeTimes[e.FullPath] = lastWriteTime;
            }
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                DateTime lastWriteTime = new DateTime();
                if (e.FullPath.IsFile())
                {
                    lastWriteTime = new FileInfo(e.FullPath).LastWriteTime;
                }
                else if (e.FullPath.IsFolder())
                {
                    lastWriteTime = new DirectoryInfo(e.FullPath).LastWriteTime;
                }

                try
                {
                    FileCreated.Invoke(e.FullPath.FullPath());
                }
                catch (Exception ex)
                {
                    _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, $"error occurred when '{e.FullPath}' created.", ex);                                            
                }

                ChangeTimes[e.FullPath] = lastWriteTime;
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
            {
                try
                {
                    FileDeleted.Invoke(e.FullPath.FullPath());
                }
                catch (Exception ex)
                {
                    _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, $"error occurred when '{e.FullPath}' deleted.", ex);                                                                
                }
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (FileRenamed != null)
            {
                try
                {
                    FileRenamed.Invoke(e.OldFullPath.FullPath(), e.FullPath.FullPath());
                }
                catch (Exception ex)
                {
                    _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, $"error occurred when '{e.FullPath}' renamed.", ex);                                                                                    
                }
            }
        }

        private void OnError(object source, ErrorEventArgs e)
        {
            _Logger.LogEvent(nameof(FolderMonitor), Severity.Error, "error occurred in folder monitor.", e.GetException());
        }
    }
}
