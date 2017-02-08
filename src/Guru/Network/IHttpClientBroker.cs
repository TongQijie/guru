namespace Guru.Network
{
    public interface IHttpClientBroker
    {
         IHttpClientRequest Get();

         IHttpClientRequest Get(string id);

         IHttpClientRequest Get(IHttpClientSettings settings);
    }
}