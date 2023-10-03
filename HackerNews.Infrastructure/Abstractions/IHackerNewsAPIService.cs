using HackerNews.Infrastructure.ExternalDtos;

namespace HackerNews.Infrastructure.Abstractions
{
    public interface IHackerNewsAPIService
    {
        Task<List<ExternalStoryDto>> GetNewStoriesAsync();
    }
}
