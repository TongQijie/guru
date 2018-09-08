using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging;
using Guru.Logging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiMetadataHandler), Lifetime.Singleton)]
    internal class DefaultApiMetadataHandler : IApiMetadataHandler
    {
        private readonly IApiFormatterProvider _ApiFormatterProvider;

        private readonly IApiMetadataProvider _ApiMetadataProvider;

        private readonly ILogger _Logger;

        public DefaultApiMetadataHandler(IApiFormatterProvider apiFormatterProvider, IApiMetadataProvider apiMetadataProvider, IFileLogger fileLogger)
        {
            _ApiFormatterProvider = apiFormatterProvider;
            _ApiMetadataProvider = apiMetadataProvider;
            _Logger = fileLogger;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            if (context?.ApplicationConfiguration?.Api?.EnableMetadata != true)
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "500",
                });
                return;
            }

            if (!context.RouteData.HasLength())
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HeaderContentType,
                    Source = ContextParameterSource.Header,
                    Value = "text/html",
                });

                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "200",
                });

                try
                {
                    await _ApiFormatterProvider.Text.Write(_ApiMetadataProvider.GetListHtml(), context.OutputStream);
                    return;
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(DefaultApiMetadataHandler), Severity.Error, "an error occurred when processing api request.", e);
                }
            }
            else
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HeaderContentType,
                    Source = ContextParameterSource.Header,
                    Value = "text/html",
                });

                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "200",
                });

                if (context.RouteData.Length < 2)
                {
                    context.SetOutputParameter(new ContextParameter()
                    {
                        Name = CallingContextConstants.HttpStatusCode,
                        Source = ContextParameterSource.Http,
                        Value = "500",
                    });
                    return;
                }

                try
                {
                    await _ApiFormatterProvider.Text.Write(_ApiMetadataProvider.GetMethodHtml(context.RouteData[0], context.RouteData[1]), context.OutputStream);
                    return;
                }
                catch (Exception e)
                {
                    _Logger.LogEvent(nameof(DefaultApiMetadataHandler), Severity.Error, "an error occurred when processing api request.", e);
                }
            }

            context.SetOutputParameter(new ContextParameter()
            {
                Name = CallingContextConstants.HttpStatusCode,
                Source = ContextParameterSource.Http,
                Value = "500",
            });
        }
    }
}
