namespace Guru.Executable.Abstractions
{
    public interface ICommandLineArgsParser
    {
        CommandLineArgs Parse(string[] args);
    }
}