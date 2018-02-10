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
using Guru.Cache.Abstractions;
using System.Threading.Tasks;

namespace AspNetCoreAppII
{
    [Injectable(typeof(IConsoleExecutable), Lifetime.Singleton)]
    public class Startup : IConsoleExecutable
    {
        private readonly IMemoryCacheProvider _MemoryCacheProvider;

        public Startup(IMemoryCacheProvider memoryCacheProvider)
        {
            _MemoryCacheProvider = memoryCacheProvider;
            _MemoryCacheProvider.Persistent = true;
        }

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

            _MemoryCacheProvider.Dispose();

            return 0;
        }

        public Task<int> RunAsync(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}
