namespace Watchman
{
    public interface IFileSystemHelper
    {
         void WriteFile(string path);

         void CreateFolder(string path);

         void Delete(string path);
    }
}