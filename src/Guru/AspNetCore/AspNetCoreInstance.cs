using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;
using Guru.DependencyInjection;

namespace Guru.AspNetCore
{
    public class AspNetCoreInstance
    {
        private readonly RequestDelegate _Next;

        private readonly string _Name;

        public AspNetCoreInstance(RequestDelegate next, string name = null, StartupDelegate startup = null)
        {
            _Next = next;
            _Name = name;

            try
            {
                startup?.Invoke(this);

                if (_Component == null)
                {
                    _Component = ContainerManager.Default.Resolve<IAspNetCoreComponent>();
                }

                Console.WriteLine($"instance({name ?? string.Empty}) startup succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(new Logging.ExceptionWrapper(e).ToString());
                Console.WriteLine($"instance({name ?? string.Empty}) startup failed.");
            }
        }

        private IAspNetCoreComponent _Component = null;

        public void RegisterComponent(IAspNetCoreComponent component)
        {
            _Component = component;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"request enter: {context.Request.Host}/{context.Request.Path}");

            if (_Component == null)
            {
                Console.WriteLine("AspNetCoreComponent is not initialized.");
            }

            try
            {
                await _Component?.Process(CallingContextBuilder.Build(context));
            }
            catch (Exception e)
            {
                Console.WriteLine($"fatal error: {e.Message}");
            }
        }
    }
}