namespace Guru.RestApi.Abstractions
{
    public interface IAuthManager
    {
        bool Validate(IAuthRestApiRequest request);

        void Create(string auth, string uid);
    }
}