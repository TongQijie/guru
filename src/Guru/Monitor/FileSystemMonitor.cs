using System;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.Monitor.Internal;
using Guru.DependencyInjection;
using Guru.Monitor.Abstractions;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Monitor
{
    [DI(typeof(IFileSystemMonitor), Lifetime = Lifetime.Singleton)]
    public class FileSystemMonitor : IFileSystemMonitor
    {
        private ConcurrentDictionary<string, FolderMonitor> _FolderMonitors = new ConcurrentDictionary<string, FolderMonitor>();

        public void Add(object referenceObject, string path, 
            Delegates.FileChangedHandlerDelegate fileChanged,
            Delegates.FileCreatedHandlerDelegate fileCreated,
            Delegates.FileDeletedHandlerDelegate fileDeleted,
            Delegates.FileRenamedHandlerDelegate fileRenamed)
        {
            var p = path.FullPath();
            if (!p.IsFolder())
            {
                throw new Exception($"monitor folder '{p}' does not exist.");
            }

            var key = p.ToLower();

            FolderMonitor folderMonitor = null;

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
                folderMonitor = new FolderMonitor(p);

                if (!_FolderMonitors.TryAdd(key, folderMonitor))
                {
                    throw new Exception(string.Format("failed to add folder '{0}' monitor.", p));
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
                if (!_FolderMonitors.TryRemove(key, out folderMonitor))
                {
                    // TODO: throw
                }
            }
        }
    }
}
