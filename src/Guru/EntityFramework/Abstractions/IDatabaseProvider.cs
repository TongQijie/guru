namespace Guru.EntityFramework.Abstractions
{
    public interface IDatabaseProvider
    {
        IDatabase GetDatabase(string name);
    }
}