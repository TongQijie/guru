using Guru.AspNetCore.Delegates;

namespace Guru.AspNetCore.Abstractions
{
    public interface IApiContext
    {
         object[] Parameters { get; }

         ApiExecuteDelegate ApiExecute { get; }
    }
}