namespace GitDiff
{
    public interface IDiff
    {
        void Execute(Commit[] commits);
    }
}