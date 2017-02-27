using System;
using System.IO;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Monitor.Abstractions;

namespace Watchman
{
    public class WatchFolder : IDisposable
    {
        private readonly IFileSystemMonitor _FileSystemMonitor;

        private readonly IFileSystemHelper _FileSystemHelper;

        private readonly IContext _Context;

        public WatchFolder(string path, WatchFolder parent)
        {
            Path = path;
            Parent = parent;

            _FileSystemMonitor = ContainerEntry.Resolve<IFileSystemMonitor>();
            _FileSystemHelper = ContainerEntry.Resolve<IFileSystemHelper>();
            _Context = ContainerEntry.Resolve<IContext>();

            _FileSystemMonitor.Add(this, (_Context.Source + "/" + Path).FullPath(), Changed, Created, Deleted, Renamed);

            var directoryInfo = new DirectoryInfo((_Context.Source + "/" + Path).FullPath());
            foreach (var folder in directoryInfo.GetDirectories())
            {
                Children = Children.Append(new WatchFolder(Path + "/" + folder.Name, this));
            }
        }

        public string Path { get; set; }

        public WatchFolder Parent { get; set; }

        public WatchFolder[] Children { get; set; }

        private void Changed(string path)
        {
            if (path.IsFile())
            {
                Console.WriteLine($"file changed: {path}");

                _FileSystemHelper.WriteFile(Path + "/" + path.Name());
            }
        }

        private void Created(string path)
        {
            if (path.IsFile())
            {
                Console.WriteLine($"file created: {path}");

                _FileSystemHelper.WriteFile(Path + "/" + path.Name());
            }
            else if (path.IsFolder())
            {
                Console.WriteLine($"folder created: {path}");

                _FileSystemHelper.CreateFolder(Path + "/" + path.Name());

                Children = Children.Append(new WatchFolder(Path + "/" + path.Name(), this));
            }
        }

        private void Deleted(string path)
        {
            Console.WriteLine($"deleted: {path}");

            var folder = Children.FirstOrDefault(x => x.Path.EqualsIgnoreCase(Path + "/" + path.Name()));
            if (folder != null)
            {
                folder.Dispose();
                Children = Children.Remove(x => x == folder);
            }

            _FileSystemHelper.Delete(Path + "/" + path.Name());
        }

        private void Renamed(string oldPath, string newPath)
        {
            Console.WriteLine($"deleted: {oldPath}");

            var folder = Children.FirstOrDefault(x => x.Path.EqualsIgnoreCase(Path + "/" + oldPath.Name()));
            if (folder != null)
            {
                folder.Dispose();
                Children = Children.Remove(x => x == folder);
            }

            _FileSystemHelper.Delete(Path + "/" + oldPath.Name());

            if (newPath.IsFile())
            {
                Console.WriteLine($"file created: {newPath}");

                _FileSystemHelper.WriteFile(Path + "/" + newPath.Name());
            }
            else if (newPath.IsFolder())
            {
                Console.WriteLine($"folder created: {newPath}");

                _FileSystemHelper.CreateFolder(Path + "/" + newPath.Name());

                Children = Children.Append(new WatchFolder(Path + "/" + newPath.Name(), this));
            }
        }

        public void Dispose()
        {
            Children.Each(x => x.Dispose());

            _FileSystemMonitor.Remove(this, (_Context.Source + "/" + Path).FullPath(), Changed, Created, Deleted, Renamed);
        }
    }
}