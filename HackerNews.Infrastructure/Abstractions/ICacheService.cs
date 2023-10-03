namespace HackerNews.Infrastructure.Abstractions
{
    public interface ICacheService
    {
        T Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
        bool TryGetValue<T>(string key, out T value);
    }
}
