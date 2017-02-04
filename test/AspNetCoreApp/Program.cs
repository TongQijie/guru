using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Guru.Middleware;

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
                    x.UseMiddleware<AspNetCoreMiddleware>();
                })
                .Build()
                .Run();
        }
    }
}
