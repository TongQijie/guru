namespace Guru.Network.Abstractions
{
    public interface ICookieManager
    {
         void SetCookie(string cookieString);

         string GetCookies();
    }
}