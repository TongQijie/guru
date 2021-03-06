﻿using System;
using System.Collections.Generic;

using Guru.ExtensionMethod;
using Guru.Monitor.Internal;
using Guru.DependencyInjection;
using Guru.Monitor.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.Logging.Abstractions;

namespace Guru.Monitor
{
    [Injectable(typeof(IFileSystemMonitor), Lifetime.Singleton)]
    internal class FileSystemMonitor : IFileSystemMonitor
    {
        private readonly ILogger _Logger;

        public FileSystemMonitor(IFileLogger logger)
        {
            _Logger = logger;
        }

        private object _SyncLocker = new object();

        private Dictionary<string, FolderMonitor> _FolderMonitors = new Dictionary<string, FolderMonitor>();

        public void Add(object referenceObject, string path, 
            Delegates.FileChangedHandlerDelegate fileChanged,
            Delegates.FileCreatedHandlerDelegate fileCreated,
            Delegates.FileDeletedHandlerDelegate fileDeleted,
            Delegates.FileRenamedHandlerDelegate fileRenamed)
        {
            var p = path.FullPath();
            if (!p.IsFolder())
            {
                throw new Exception($"monitor folder '{path}' does not exist.");
            }

            var key = p.ToLower();

            FolderMonitor folderMonitor = null;

            lock (_SyncLocker)
            {
                if (_FolderMonitors.ContainsKey(key))
                {
                    if (!_FolderMonitors[key].ReferencedObjects.Exists(x => x.Equals(referenceObject)))
                    {
                        folderMonitor = _FolderMonitors[key];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    folderMonitor = new FolderMonitor(p, _Logger);
                    _FolderMonitors.Add(key, folderMonitor);
                }
            }
            
            folderMonitor.ReferencedObjects = folderMonitor.ReferencedObjects.Append(referenceObject);

            if (fileChanged != null)
            {
                folderMonitor.FileChanged += fileChanged;
            }
            if (fileCreated != null)
            {
                folderMonitor.FileCreated += fileCreated;
            }
            if (fileDeleted != null)
            {
                folderMonitor.FileDeleted += fileDeleted;
            }
            if (fileRenamed != null)
            {
                folderMonitor.FileRenamed += fileRenamed;
            }

            folderMonitor.Start();
        }

        public void Remove(object referenceObject, string path,
            Delegates.FileChangedHandlerDelegate fileChanged,
            Delegates.FileCreatedHandlerDelegate fileCreated,
            Delegates.FileDeletedHandlerDelegate fileDeleted,
            Delegates.FileRenamedHandlerDelegate fileRenamed)
        {
            var key = path.FullPath().ToLower();

            FolderMonitor folderMonitor = null;
            if (_FolderMonitors.ContainsKey(key) && _FolderMonitors[key].ReferencedObjects.Exists(x => x.Equals(referenceObject)))
            {
                folderMonitor = _FolderMonitors[key];
            }
            else
            {
                return;
            }

            folderMonitor.ReferencedObjects = folderMonitor.ReferencedObjects.Remove(x => x.Equals(referenceObject));

            if (fileChanged != null)
            {
                folderMonitor.FileChanged -= fileChanged;
            }
            if (fileCreated != null)
            {
                folderMonitor.FileCreated -= fileCreated;
            }
            if (fileDeleted != null)
            {
                folderMonitor.FileDeleted -= fileDeleted;
            }
            if (fileRenamed != null)
            {
                folderMonitor.FileRenamed -= fileRenamed;
            }

            if (folderMonitor.ReferencedObjects.Length == 0)
            {
                folderMonitor.Stop();

                lock (_SyncLocker)
                {
                    _FolderMonitors.Remove(key);
                }
            }
        }
    }
}
