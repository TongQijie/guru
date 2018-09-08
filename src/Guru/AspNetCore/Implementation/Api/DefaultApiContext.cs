using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;

namespace Guru.AspNetCore.Implementation.Api
{
    internal class DefaultApiContext : IApiContext
    {
        public object[] Parameters { get; set; }

        public ApiExecuteDelegate ApiExecute { get; set; }
    }
}