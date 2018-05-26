using System;

namespace Guru.Restful.Abstractions
{
    public interface IIdentityValidator
    {
        string Create(string userId);

        bool Validate(RequestHead head);
    }
}