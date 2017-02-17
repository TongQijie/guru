using System;

using Guru.Middleware.RESTfulService;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.Abstractions
{
    public interface IRESTfulServiceFactory
    {
        void Init(IContainer container, IAssemblyLoader loader);

        void RegisterService(Type serviceType);

        ServiceContext GetService(string servicePrefix, string serviceName, string methodName, HttpVerb httpVerb);
    }
}