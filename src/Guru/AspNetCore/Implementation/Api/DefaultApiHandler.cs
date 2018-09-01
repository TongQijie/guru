using System;
using System.Reflection;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;
using Guru.AspNetCore.Implementation.Api.Formatter;
using Guru.Logging;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiHandler), Lifetime.Singleton)]
    public class DefaultApiHandler : IApiHandler
    {
        private readonly IApiProvider _ApiProvider;

        private readonly IApiFormatters _ApiFormatters;

        private readonly ILogger _DefaultLogger;

        private readonly IApiLogger _ApiLogger;

        public DefaultApiHandler(IApiProvider apiHandler, IApiFormatters apiFormaters, IFileLogger defaultLogger, IApiLogger apiLogger)
        {
            _ApiProvider = apiHandler;
            _ApiFormatters = apiFormaters;
            _DefaultLogger = defaultLogger;
            _ApiLogger = apiLogger;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            var startTime = DateTime.Now;

            var apiContext = await _ApiProvider.GetApi(context);
            if (apiContext == null)
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "400",
                });
                return;
            }

            object executionResult = null;
            try
            {
                executionResult = apiContext.ApiExecute(apiContext.Parameters);

                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "200",
                });

                if (context.ApplicationConfiguration?.Api?.Headers.HasLength() == true)
                {
                    foreach (var header in context.ApplicationConfiguration.Api.Headers)
                    {
                        if (header.Name.HasValue() && header.Values.HasLength())
                        {
                            foreach (var value in header.Values)
                            {
                                context.SetOutputParameter(new ContextParameter()
                                {
                                    Name = header.Name,
                                    Source = ContextParameterSource.Header,
                                    Value = value,
                                });
                            }
                        }
                    }
                }

                if (executionResult != null)
                {
                    AbstractApiFormatter apiFormatter = null;
                    if (executionResult.GetType() == typeof(string) || executionResult.GetType().GetTypeInfo().IsValueType)
                    {
                        apiFormatter = _ApiFormatters.Text;
                    }
                    else
                    {
                        apiFormatter = _ApiFormatters.Get(context);
                    }

                    context.SetOutputParameter(new ContextParameter()
                    {
                        Name = CallingContextConstants.HeaderContentType,
                        Source = ContextParameterSource.Header,
                        Value = apiFormatter?.ContentType,
                    });

                    await apiFormatter.Write(executionResult, context.OutputStream);
                }
            }
            catch(Exception e)
            {
                _DefaultLogger.LogEvent(nameof(DefaultApiHandler), Severity.Error, "an error occurred when processing api request.", e);
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = CallingContextConstants.HttpStatusCode,
                    Source = ContextParameterSource.Http,
                    Value = "500",
                });
                return;
            }
            finally
            {
                if (context.ApplicationConfiguration?.Api?.EnableLog == true)
                {
                    _ApiLogger.LogEvent(context, startTime, DateTime.Now, apiContext.Parameters, executionResult);
                }
            }
        }
    }
}