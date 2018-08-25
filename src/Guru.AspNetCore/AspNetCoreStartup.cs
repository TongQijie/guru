using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.Executable;
using Guru.Executable.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Guru.AspNetCore
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    internal class AspNetCoreStartup : IConsoleExecutable
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
