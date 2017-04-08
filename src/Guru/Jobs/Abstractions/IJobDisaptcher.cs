namespace Guru.Jobs.Abstractions
{
    public interface IJobDispatcher
    {
         void Add(IJob job, string[] args);

         void Remove(IJob job);

         void Enable(IJob job);

         void Disable(IJob job);

         void Run();
    }
}