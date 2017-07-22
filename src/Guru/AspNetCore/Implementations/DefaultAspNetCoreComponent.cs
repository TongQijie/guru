using System;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Microsoft.AspNetCore.Http;

namespace Guru.AspNetCore.Implementations
{
    [Injectable(typeof(IAspNetCoreComponent), Lifetime.Singleton)]
    public class DefaultAspNetCoreComponent : IAspNetCoreComponent
    {
        public Task Process(CallingContext context)
        {
            throw new NotImplementedException();
        }
    }
}