using HackerNews.Application.Abstractions;
using HackerNews.Infrastructure.Dtos;

namespace HackerNews.Application.QueryService.Queries
{ 
    public sealed record GetAllNewStoriesQuery : IQuery<List<StoryDto>>;
}
