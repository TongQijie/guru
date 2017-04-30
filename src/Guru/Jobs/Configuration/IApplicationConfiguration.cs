namespace Guru.Jobs.Configuration
{
    public interface IApplicationConfiguration
    {
        bool Enabled { get; }

        JobItemConfiguration[] Jobs { get; }
    }
}
