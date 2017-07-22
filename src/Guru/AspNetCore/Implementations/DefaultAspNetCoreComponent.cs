using System;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.AspNetCore.Abstractions;

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