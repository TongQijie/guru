using System.Threading.Tasks;

namespace Guru.Restful.Abstractions
{
    public interface IIdentityValidator
    {
        string Create(string userId);

        Task<string> CreateAsync(string userId);

        bool Validate(RequestHead head);

        Task<bool> ValidateAsync(RequestHead head);
    }
}