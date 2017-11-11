using System;

namespace Guru.Cache.Implementation
{
    internal class DefaultMemoryCacheItem
    {
        public object Value { get; set; }

        public DateTime ExpiryTime { get; set; }
    }
}
