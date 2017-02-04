namespace Guru.DynamicProxy
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}
