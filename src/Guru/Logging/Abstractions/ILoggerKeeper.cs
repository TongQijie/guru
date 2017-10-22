namespace Guru.Logging.Abstractions
{
    public interface ILoggerKeeper
    {
        void Connect(ILogger logger);

        void Disconnect(ILogger logger);

        void DisposeAll();
    }
}