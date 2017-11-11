using Guru.Executable.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Guru.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Guru.AspNetCore.Delegates;

namespace AspNetCoreAppII
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Startup : IConsoleExecutable
    {
        public int Run(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices(x => x.AddSingleton<IHttpContextAccessor, HttpContextAccessor>())
                .Configure(x =>
                {
                    HttpContextUtils.Configure(x.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
                    x.UseMiddleware<AspNetCoreAppInstance>();
                })
                .Build()
                .Run();

            return 0;
        }
    }
}
