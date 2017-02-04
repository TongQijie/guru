namespace Guru.Middleware.RESTfulService
{
    public class ServiceContext
    {
        public object ServiceIntance { get; set; }

        public RESTfulServiceInfo ServiceInfo { get; set; }

        public RESTfulMethodInfo MethodInfo { get; set; }
    }
}