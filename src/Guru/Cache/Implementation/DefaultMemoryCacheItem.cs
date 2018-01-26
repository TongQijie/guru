using System;

namespace Guru.Cache.Implementation
{
    internal class DefaultMemoryCacheItem
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public DateTime ExpiryTime { get; set; }
    }
}
