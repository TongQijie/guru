namespace Guru.Formatter.Json
{
    public enum JType
    {
        /// <summary>
        /// object
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// int, string ...
        /// </summary>
        Value = 1,

        /// <summary>
        /// IDictionary
        /// </summary>
        Map = 2,

        /// <summary>
        /// ICollection
        /// </summary>
        Array = 3,

        /// <summary>
        /// any class
        /// </summary>
        Object = 4,
    }
}