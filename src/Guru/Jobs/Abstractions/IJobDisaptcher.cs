namespace Guru.Jobs.Abstractions
{
    public interface IJobDispatcher
    {
        bool Async { get; set; }

        void Run();

    }
}