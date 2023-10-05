
namespace HackerNews.Infrastructure.Abstractions
{
    public interface IHttpClientFactoryWrapper
    {
        HttpClient CreateClient();
    }
}
