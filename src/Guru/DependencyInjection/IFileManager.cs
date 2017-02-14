namespace Guru.DependencyInjection
{
    public interface IFileManager
    {
         T Single<T>();

         T[] Many<T>();
    }
}