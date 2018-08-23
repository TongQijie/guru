namespace Guru.Http.Abstractions
{
    public interface ICookieManager
    {
         void SetCookie(string cookieString);

         string GetCookies();
    }
}