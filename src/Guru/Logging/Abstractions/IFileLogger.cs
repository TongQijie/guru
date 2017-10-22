namespace Guru.Logging.Abstractions
{
    public interface IFileLogger : ILogger
    {
        string Folder { get; set; }

        int Interval { get; set; }
    }
}