using AutoMapper;
using HackerNews.Application.Abstractions;
using HackerNews.Application.QueryService.Queries;
using HackerNews.Infrastructure.Abstractions;
using HackerNews.Infrastructure.Dtos;

namespace HackerNews.Application.QueryService.QueryHandlers
{
    public class GetAllNewStoriesQueryHandler : IQueryHandler<GetAllNewStoriesQuery, List<StoryDto>>
    {
        private readonly IHackerNewsAPIService _hackerNewsApiService;
        private readonly IMapper _mapper;

        public GetAllNewStoriesQueryHandler(IHackerNewsAPIService hackerNewsApiService, IMapper mapper)
        {
            _= hackerNewsApiService ?? throw new ArgumentNullException(nameof(hackerNewsApiService));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _hackerNewsApiService = hackerNewsApiService;
            this._mapper = mapper;
        }

        public async Task<List<StoryDto>> Handle(GetAllNewStoriesQuery request, CancellationToken cancellationToken)
        {
            var externalStoryList= await _hackerNewsApiService.GetNewStoriesAsync();

            return _mapper.Map<List<StoryDto>>(externalStoryList);
        }
    }
}
