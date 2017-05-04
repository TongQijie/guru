namespace Guru.Middleware.Configuration
{
    public interface IApplicationConfiguration
    {
        string WWWRoot { get; }

        string[] ServicePrefixes { get; }

        KeyValueItemConfiguration[] Routes { get; }

        KeyValueItemConfiguration[] Headers { get; }

        StaticResouceConfiguration[] Resources { get; }

        RewriteConfiguration[] Rewrites { get; }
    }
}