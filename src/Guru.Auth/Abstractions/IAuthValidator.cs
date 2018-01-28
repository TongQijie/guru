namespace Guru.Auth.Abstractions
{
    public interface IAuthValidator
    {
        void Validate(IAuthRequest authRequest);

        void AddUid(string auth, string uid);
    }
}