namespace Guru.Restful.Abstractions
{
    public interface IIdentityConfiguration
    {
        long ExpireMillis { get; }

        long RenewMillis { get; }
    }
}