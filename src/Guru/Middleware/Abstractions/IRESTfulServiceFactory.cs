using System;

using Guru.Middleware.RESTfulService;

namespace Guru.Middleware.Abstractions
{
    public interface IRESTfulServiceFactory
    {
        void Init();

        void RegisterService(Type serviceType);

        ServiceContext GetService(string servicePrefix, string serviceName, string methodName, HttpVerb httpVerb);
    }
}