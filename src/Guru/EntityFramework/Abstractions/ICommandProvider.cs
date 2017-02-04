namespace Guru.EntityFramework.Abstractions
{
    public interface ICommandProvider
    {
         ICommand GetCommand(string name);
    }
}