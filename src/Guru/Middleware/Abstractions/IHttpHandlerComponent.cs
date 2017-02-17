using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Guru.Middleware.Abstractions
{
    public interface IHttpHandlerComponent
    {
         Task Process(string uri, HttpContext context);
    }
}