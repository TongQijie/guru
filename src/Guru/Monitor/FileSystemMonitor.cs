using System;
using System.IO;
using System.Collections.Concurrent;

using Guru.ExtensionMethod;
using Guru.Monitor.Internal;
using Guru.DependencyInjection;
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
            string folder = GetFolder(path);

            FolderMonitor folderMonitor = null;

            if (_FolderMonitors.ContainsKey(folder))
            {
                if (!_FolderMonitors[folder].ReferencedObjects.Exists(x => x.Equals(referenceObject)))
                {
                    folderMonitor = _FolderMonitors[folder];
                }
                else
                {
                    return;
                }
            }
            else
            {
                folderMonitor = new FolderMonitor(folder);

                if (!_FolderMonitors.TryAdd(folder, folderMonitor))
                {
                    throw new Exception(string.Format("failed to add folder '{0}' monitor.", folder));
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
            string folder = GetFolder(path);

            FolderMonitor folderMonitor = null;
            if (_FolderMonitors.ContainsKey(folder) && _FolderMonitors[folder].ReferencedObjects.Exists(x => x.Equals(referenceObject)))
            {
                folderMonitor = _FolderMonitors[folder];
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
                if (!_FolderMonitors.TryRemove(folder, out folderMonitor))
                {
                    // TODO: throw
                }
            }
        }

        private string GetFolder(string path)
        {
            if (path.IsFile())
            {
                return Path.GetDirectoryName(path).Replace('\\', '/').ToLower();
            }
            else if (path.IsFolder())
            {
                return path.Replace('\\', '/').ToLower();
            }
            else
            {
                throw new Exception(string.Format("monitor path '{0}' is not valid.", path));
            }
        }
    }
}
