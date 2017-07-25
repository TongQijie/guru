namespace Guru.AspNetCore.Abstractions
{
    public interface IAspNetCoreRouter
    {
        void GetRouteData(CallingContext context);
    }
}