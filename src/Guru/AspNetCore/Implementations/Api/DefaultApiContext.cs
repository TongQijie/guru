using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;

namespace Guru.AspNetCore.Implementations.Api
{
    public class DefaultApiContext : IApiContext
    {
        public object[] Parameters { get; set; }

        public ApiExecuteDelegate ApiExecute { get; set; }
    }
}