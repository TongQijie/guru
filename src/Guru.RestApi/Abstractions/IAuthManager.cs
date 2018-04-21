using System;

namespace Guru.RestApi.Abstractions
{
    public interface IAuthManager
    {
        string New(string uid, TimeSpan expiryTimeSpan);

        bool Validate(IAuthRestApiRequest request);
    }
}