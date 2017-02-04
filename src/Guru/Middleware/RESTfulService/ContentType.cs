namespace Guru.Middleware.RESTfulService
{
    public enum ContentType
    {
        /// <summary>
        /// support any data format.
        /// </summary>
        Any = 0,

        /// <summary>
        /// application/json
        /// </summary>
        Json = 1,

        /// <summary>
        /// application/xml
        /// text/xml
        /// </summary>
        Xml = 2,

        /// <summary>
        /// text/plain
        /// </summary>
        Text = 3,
    }
}