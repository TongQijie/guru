using Guru.Formatter.Abstractions;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiFormatter
    {
         IFormatter GetFormatter(string name);
    }
}