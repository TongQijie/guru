using Microsoft.AspNetCore.Http;

namespace Guru.Middleware.Abstractions
{
    public interface ICallingContext
    {
        HttpContext Context { get; }
    }
}