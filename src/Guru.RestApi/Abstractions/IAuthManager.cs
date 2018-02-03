namespace Guru.RestApi.Abstractions
{
    public interface IAuthManager
    {
        bool Validate(AuthRestApiRequestHead head);

        void Create(string auth, string uid);
    }
}