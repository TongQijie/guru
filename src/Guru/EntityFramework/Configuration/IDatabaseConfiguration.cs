namespace Guru.EntityFramework.Configuration
{
    public interface IDatabaseConfiguration
    {
        DatabaseItemConfiguration[] Items { get; }
    }
}