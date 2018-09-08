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

                if (Component == null)
                {
                    Component = DependencyContainer.Resolve<IAspNetCoreComponent>();
                }

                startup?.Invoke(this);

                Console.WriteLine($"AspNetCore App Instance '{_Name ?? string.Empty}' startup succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetInfo());
                Console.WriteLine($"AspNetCore App Instance '{_Name ?? string.Empty}' startup failed.");
            }
        }

        public IAspNetCoreComponent Component { get; private set; }

        public void RegisterComponent(IAspNetCoreComponent component)
        {
            Component = component;
        }

        public async Task Invoke(HttpContext context)
        {
            if (Component == null)
            {
                Console.WriteLine("AspNetCoreComponent is not initialized.");
                return;
            }

            try
            {
                var callingContext = CallingContextBuilder.Build(context);
                if (callingContext != null)
                {
                    context.Items["CallingContext"] = callingContext;
                }

                await Component.Process(callingContext);
            }
            catch (Exception e)
            {
                Console.WriteLine($"fatal error: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}