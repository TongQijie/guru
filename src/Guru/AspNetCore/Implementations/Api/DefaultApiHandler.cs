using System;
using System.Threading.Tasks;

using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using Guru.Logging.Abstractions;

namespace Guru.AspNetCore.Implementations.Api
{
    [Injectable(typeof(IApiHandler), Lifetime.Singleton)]
    public class DefaultApiHandler : IApiHandler
    {
        private readonly IApiProvider _ApiProvider;

        private readonly IApiFormatter _ApiFormatter;

        private readonly ILogger _Logger;

        public DefaultApiHandler(IApiProvider apiHandler, IApiFormatter apiFormater, IFileLogger fileLogger)
        {
            _ApiProvider = apiHandler;
            _ApiFormatter = apiFormater;
            _Logger = fileLogger;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            System.Console.WriteLine($"Handler enter");

            var apiContext = await _ApiProvider.GetApi(context);
            if (apiContext == null)
            {
                System.Console.WriteLine($"apiContext is empty.");

                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "404",
                });
                return;
            }

            object executionResult = null;
            try
            {
                executionResult = apiContext.ApiExecute(apiContext.Parameters);
            }
            catch(Exception e)
            {
                _Logger.LogEvent(nameof(DefaultApiHandler), Severity.Error, "an error occurred when processing api request.", e);
                context.SetOutputParameter(new ContextParameter()
                {
                    Name = "StatusCode",
                    Source = ContextParameterSource.Http,
                    Value = "500",
                });
                return;
            }

            var contentType = "application/json";
            if (context.InputParameters.ContainsKey("formatter"))
            {
                var formatter = context.InputParameters["formatter"].Value;
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
    }
}