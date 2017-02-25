namespace Guru.Network.Abstractions
{
    public interface IHttpClientBroker
    {
         IHttpClientRequest Get();

         IHttpClientRequest Get(string id);

         IHttpClientRequest Get(IHttpClientSettings settings);
    }
}