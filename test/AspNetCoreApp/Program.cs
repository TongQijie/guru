using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Guru.Middleware;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;

namespace AspNetCoreApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureServices(x => x.AddSingleton<IHttpContextAccessor, HttpContextAccessor>())
                .Configure(x =>
                {
                    AspNetCoreHttpContext.Configure(x.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
                    // x.UseMiddleware<AspNetCoreMiddleware>(new Lifetime());
                    x.UseMiddleware<AspNetCoreMiddleware>();
                })
                .Build()
                .Run();
        }

        public class Lifetime : IMiddlewareLifetime
        {
            public void Startup()
            {
                var handler = ContainerManager.Default.Resolve<IRESTfulServiceHandler>();
                handler.JsonFormatter.OmitDefaultValue = true;
            }
        }
    }
}