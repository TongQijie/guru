using System.Threading.Tasks;

namespace Guru.Restful.Abstractions
{
    public interface IIdentityTokenPersistence
    {
        Task SetAsync(IdentityToken identityToken);

        void Set(IdentityToken identityToken);

        Task<IdentityToken> GetAsync(string token);

        IdentityToken Get(string token);
    }
}