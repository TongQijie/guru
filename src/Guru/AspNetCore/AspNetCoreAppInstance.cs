using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Delegates;
using Guru.DependencyInjection;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore
{
    public class AspNetCoreAppInstance
    {
        private readonly RequestDelegate _Next;

        private readonly string _Name;

        public AspNetCoreAppInstance(RequestDelegate next, StartupDelegate startup = null)
        {
            _Next = next;

            try
            {
                var config = DependencyContainer.Resolve<IApplicationConfiguration>();
                if (config.AppId.HasValue())
                {
                    _Name = config.AppId;
                }

                if (_Component == null)
                {
                    _Component = DependencyContainer.Resolve<IAspNetCoreComponent>();
                }

                startup?.Invoke(this);

                Console.WriteLine($"AspNetCore App Instance '{_Name ?? string.Empty}' startup succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(new Logging.ExceptionWrapper(e).ToString());
                Console.WriteLine($"AspNetCore App Instance '{_Name ?? string.Empty}' startup failed.");
            }
        }

        private IAspNetCoreComponent _Component = null;

        public IAspNetCoreComponent Component => _Component;

        public void RegisterComponent(IAspNetCoreComponent component)
        {
            _Component = component;
        }

        public async Task Invoke(HttpContext context)
        {
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
                Console.WriteLine($"fatal error: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}