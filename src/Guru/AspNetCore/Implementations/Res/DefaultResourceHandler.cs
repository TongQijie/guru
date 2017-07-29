using System.IO;
using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore.Implementations.Res
{
    [Injectable(typeof(IResourceHandler), Lifetime.Singleton)]
    public class DefaultResourceHandler : IResourceHandler
    {
        public async Task ProcessRequest(CallingContext context)
        {
            if (context.RouteData.Length == 0)
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "404",
                });
                return;
            }

            if (context.ApplicationConfiguration.Resource == null ||
                !context.ApplicationConfiguration.Resource.Directory.IsFolder())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "500",
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

            var mineType = "";
            var dotIndex = context.RouteData[context.RouteData.Length - 1].LastIndexOf('.');
            if (dotIndex >= 0)
            {
                var ext = context.RouteData[context.RouteData.Length - 1].Substring(dotIndex + 1);
                if (context.ApplicationConfiguration.Resource.MineTypes.ContainsKey(ext.ToLower()))
                {
                    mineType = context.ApplicationConfiguration.Resource.MineTypes[ext.ToLower()];
                }
            }

            if (mineType.HasValue())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "Content-Type",
                    Source = ContextParameterSource.Header,
                    Value = mineType,
                });
            }

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