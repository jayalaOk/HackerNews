using HackerNews.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNews.Infrastructure.Services
{
    public class CacheService :ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            return _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
