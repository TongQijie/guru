using System;
using System.Reflection;
using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Logging.Abstractions;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection.Attributes;

namespace Guru.AspNetCore.Implementation.Api
{
    [Injectable(typeof(IApiHandler), Lifetime.Singleton)]
    public class DefaultApiHandler : IApiHandler
    {
        private readonly IApiProvider _ApiProvider;

        private readonly IApiFormatter _ApiFormatter;

        private readonly ILogger _DefaultLogger;

        private readonly IRequestLogger _RequestLogger;

        public DefaultApiHandler(IApiProvider apiHandler, IApiFormatter apiFormater, IFileLogger defaultLogger, IRequestLogger requestLogger)
        {
            _ApiProvider = apiHandler;
            _ApiFormatter = apiFormater;
            _DefaultLogger = defaultLogger;
            _RequestLogger = requestLogger;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            var startTime = DateTime.Now;

            var apiContext = await _ApiProvider.GetApi(context);
            if (apiContext == null)
            {
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "404",
                });
                return;
            }

            object executionResult = null;
            var contentType = "application/json";
            try
            {
                executionResult = apiContext.ApiExecute(apiContext.Parameters);

                if (context.InputParameters.ContainsKey("formatter"))
                {
                    var formatter = context.InputParameters.Get("formatter").Value;
                    if (formatter.ContainsIgnoreCase("json"))
                    {
                        contentType = "application/json";
                    }
                    else if (formatter.ContainsIgnoreCase("xml"))
                    {
                        contentType = "application/xml";
                    }
                    else if (formatter.ContainsIgnoreCase("text"))
                    {
                        contentType = "plain/text";
                    }
                }
                else if (executionResult.GetType() == typeof(string) || 
                    executionResult.GetType().GetTypeInfo().IsValueType)
                {
                    contentType = "plain/text";
                }

                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "Content-Type",
                    Source = ContextParameterSource.Header,
                    Value = contentType,
                });

                if (contentType == "application/json")
                {
                    await _ApiFormatter.GetFormatter("json").WriteObjectAsync(executionResult, context.OutputStream);
                }
                else if (contentType == "application/xml")
                {
                    await _ApiFormatter.GetFormatter("xml").WriteObjectAsync(executionResult, context.OutputStream);
                }
                else
                {
                    await _ApiFormatter.GetFormatter("text").WriteObjectAsync(executionResult, context.OutputStream);
                }
            }
            catch(Exception e)
            {
                _DefaultLogger.LogEvent(nameof(DefaultApiHandler), Severity.Error, "an error occurred when processing api request.", e);
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "500",
                });
                return;
            }
            finally
            {
                if (context.ApplicationConfiguration?.Api?.EnableLog == true)
                {
                    _RequestLogger.LogEvent(nameof(DefaultApiHandler), context, startTime, DateTime.Now, apiContext.Parameters, executionResult);
                }
            }
        }
    }
}