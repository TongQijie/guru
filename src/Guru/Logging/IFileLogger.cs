namespace Guru.Logging
{
    public interface IFileLogger : ILogger
    {
        string Folder { get; }
    }
}