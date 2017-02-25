using Guru.Monitor.Delegates;

namespace Guru.Monitor.Abstractions
{
    public interface IFileSystemMonitor
    {
        void Add(object referenceObject, string path,
            FileChangedHandlerDelegate fileChanged,
            FileCreatedHandlerDelegate fileCreated,
            FileDeletedHandlerDelegate fileDeleted,
            FileRenamedHandlerDelegate fileRenamed);

        void Remove(object referenceObject, string path,
            FileChangedHandlerDelegate fileChanged,
            FileCreatedHandlerDelegate fileCreated,
            FileDeletedHandlerDelegate fileDeleted,
            FileRenamedHandlerDelegate fileRenamed);
    }
}
