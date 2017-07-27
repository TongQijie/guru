using System.Threading.Tasks;
using Guru.AspNetCore.Abstractions;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;
using System;

namespace Guru.AspNetCore.Implementations.Api
{
    [Injectable(typeof(IApiHandler), Lifetime.Singleton)]
    public class DefaultApiHandler : IApiHandler
    {
        private readonly IApiProvider _ApiProvider;

        private readonly IApiFormatter _ApiFormatter;

        public DefaultApiHandler(IApiProvider apiHandler, IApiFormatter apiFormater)
        {
            _ApiProvider = apiHandler;
            _ApiFormatter = apiFormater;
        }

        public async Task ProcessRequest(CallingContext context)
        {
            var apiContext = await _ApiProvider.GetApi(context);
            if (apiContext == null)
            {
                // TODO: log error
                return;
            }

            object executionResult = null;
            try
            {
                executionResult = apiContext.ApiExecute(apiContext.Parameters);
            }
            catch(Exception e)
            {
                // TODO: log error
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