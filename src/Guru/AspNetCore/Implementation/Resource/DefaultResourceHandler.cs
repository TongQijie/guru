using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

        private readonly IResourceLogger _ResourceLogger;

        public DefaultResourceHandler(IFileLogger logger, IResourceLogger resourceLogger)
        {
            _Logger = logger;
            _ResourceLogger = resourceLogger;
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
            else
            {
                mineType = "application/octet-stream";
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

            var parameter = context.InputParameters.Get("Range");
            if (parameter != null && parameter.Source == ContextParameterSource.Header && RangeHeaderValue.TryParse(parameter.Value, out var rangeHeaderValue))
            {
                var totalLength = new FileInfo(resourcePath).Length;

                foreach (var range in rangeHeaderValue.Ranges)
                {
                    if (range.From != null && range.From >= 0 && range.To == null && range.From < totalLength)
                    {
                        var startIndex = (long)range.From;

                        var totalRead = 0L;
                        if ((startIndex + context.ApplicationConfiguration.Resource.MaxRangeBytes) <= totalLength)
                        {
                            totalRead = context.ApplicationConfiguration.Resource.MaxRangeBytes;
                        }
                        else
                        {
                            totalRead = totalLength - startIndex;
                        }

                        await OutputPartialContent(context, resourcePath, mineType, startIndex, totalRead, totalLength);

                        break;
                    }
                    else if (range.From != null && range.From >= 0 && range.To != null && range.To >= 0 && range.From <= range.To && range.To < totalLength)
                    {
                        var startIndex = (long)range.From;
                        var endIndex = (long)range.To;

                        var totalRead = endIndex - startIndex + 1;

                        await OutputPartialContent(context, resourcePath, mineType, startIndex, totalRead, totalLength);

                        break;
                    }
                    else
                    {
                        // not support
                    }
                }
            }
            else
            {
                await OutputTotalContent(context, resourcePath, mineType);
            }

            if (context.ApplicationConfiguration?.Resource?.EnableLog == true)
            {
                _ResourceLogger.LogEvent(context);
            }
        }

        private async Task OutputTotalContent(CallingContext context, string resourcePath, string contentType)
        {
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "Content-Type",
                Source = ContextParameterSource.Header,
                Value = contentType,
            });
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "StatusCode",
                Source = ContextParameterSource.Http,
                Value = "200",
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

        private async Task OutputPartialContent(CallingContext context, string resourcePath, string contentType, 
            long startIndex, long totalRead, long totalLength)
        {
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "Content-Type",
                Source = ContextParameterSource.Header,
                Value = contentType,
            });
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "Accept-Ranges",
                Source = ContextParameterSource.Header,
                Value = "bytes",
            });
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "Content-Range",
                Source = ContextParameterSource.Header,
                Value = $"bytes {startIndex}-{startIndex + totalRead - 1}/{totalLength}",
            });
            context.SetOutputParameter(new ContextParameter()
            {
                Name = "StatusCode",
                Source = ContextParameterSource.Http,
                Value = "206",
            });

            using (var inputStream = new FileStream(resourcePath, FileMode.Open, FileAccess.Read))
            {
                inputStream.Seek(startIndex, SeekOrigin.Begin);

                var count = 0L;
                var buffer = new byte[16 * 1024];
                while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    totalRead -= count;
                    if (totalRead <= 0)
                    {
                        count = count + totalRead;
                    }
                    await context.OutputStream.WriteAsync(buffer, 0, (int)count);
                    if (totalRead <= 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}