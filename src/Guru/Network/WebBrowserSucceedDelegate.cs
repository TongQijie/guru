using System.Threading.Tasks;
using Guru.Network.Abstractions;

namespace Guru.Network
{
    public delegate Task WebBrowserSucceedDelegate(IHttpClientResponse response);
}