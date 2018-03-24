using Guru.Executable.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Guru.AspNetCore;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;

namespace AspNetCoreAppII
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Startup : IConsoleExecutable
    {
        public int Run(string[] args)
        {
            Host.Run();
            return 0;
        }

        public async Task<int> RunAsync(string[] args)
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
