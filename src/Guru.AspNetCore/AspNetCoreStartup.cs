using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Guru.AspNetCore
{
    [Injectable(typeof(Executable.Abstractions.IStartup), Lifetime.Singleton)]
    internal class AspNetCoreStartup : Executable.Abstractions.IStartup
    {
        public async Task<int> RunAsync(CommandLineArgs args)
        {
            await Host.RunAsync();
            return 0;
        }

        private IWebHost Host
        {
            get
            {
                return new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(DependencyContainer.Resolve<IApplicationConfiguration>()?.Urls ?? new string[] { "http://localhost:5000" })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureServices(x => x.AddSingleton<IHttpContextAccessor, HttpContextAccessor>())
                    .Configure(x =>
                    {
                        HttpContextUtils.Configure(x.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
                        x.UseMiddleware<AspNetCoreAppInstance>();
                    })
                    .Build();
            }
        }
    }
}
