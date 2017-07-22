using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Guru.AspNetCore.Abstractions;

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
                if (startup != null)
                {
                    startup(this);
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
            await _Component?.Process(CallingContextBuilder.Build(context));
        }
    }
}