using System.IO;
using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Implementation.Resource
{
    [Injectable(typeof(IResourceHandler), Lifetime.Singleton)]
    public class DefaultResourceHandler : IResourceHandler
    {
        private readonly ILogger _Logger;

        public DefaultResourceHandler(IFileLogger logger)
        {
            _Logger = logger;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            if (context.RouteData.Length == 0)
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "400",
                });
                return;
            }

            if (context.ApplicationConfiguration.Resource == null ||
                context.ApplicationConfiguration.Resource.Directory == null ||
                !context.ApplicationConfiguration.Resource.Directory.IsFolder())
            {
                _Logger.LogEvent(nameof(DefaultResourceHandler), Severity.Error, "Resource directory is invalid.");
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "500",
                });
                return;
            }

            var mineType = "";
            var dotIndex = context.RouteData[context.RouteData.Length - 1].LastIndexOf('.');
            if (dotIndex >= 0)
            {
                var ext = context.RouteData[context.RouteData.Length - 1].Substring(dotIndex + 1);
                if (context.ApplicationConfiguration.Resource.MineTypes != null &&
                    context.ApplicationConfiguration.Resource.MineTypes.ContainsKey(ext.ToLower()))
                {
                    mineType = context.ApplicationConfiguration.Resource.MineTypes[ext.ToLower()];
                }
            }
            if (!mineType.HasValue())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "415",
                });
                return;
            }

            var resourcePath = context.ApplicationConfiguration.Resource.Directory.FullPath() + "/" + string.Join("/", context.RouteData);
            if (!resourcePath.IsFile())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "404",
                });
                return;
            }

            context.SetOutputParameter(new ContextParameter()
            {
                Name = "Content-Type",
                Source = ContextParameterSource.Header,
                Value = mineType,
            });

            using (var inputStream = new FileStream(resourcePath, FileMode.Open, FileAccess.Read))
            {
                var count = 0;
                var buffer = new byte[16 * 1024];
                while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await context.OutputStream.WriteAsync(buffer, 0, count);
                }
            }
        }
    }
}