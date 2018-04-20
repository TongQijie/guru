using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guru.Network.Abstractions
{
    public interface IWebBrowser
    {
         Task<IWebBrowser> Browse(string url, WebBrowserSucceedDelegate succeed);

         Task<IWebBrowser> Browse(string url, IDictionary<string, string> queryString, WebBrowserSucceedDelegate succeed);
    }
}