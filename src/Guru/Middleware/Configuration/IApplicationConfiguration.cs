namespace Guru.Middleware.Configuration
{
    public interface IApplicationConfiguration
    {
         KeyValueItemConfiguration[] Routes { get; }

         KeyValueItemConfiguration[] Headers { get; }

         StaticResouceConfiguration[] Resources { get; }

         RewriteConfiguration[] Rewrites { get; }
    }
}