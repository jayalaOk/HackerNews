using HackerNews.Infrastructure.Abstractions;
using HackerNews.Infrastructure.ExternalDtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace HackerNews.Infrastructure.ExternalServices
{
    public class HackerNewsAPIService : IHackerNewsAPIService
    {
        private readonly ILogger<HackerNewsAPIService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;

        public HackerNewsAPIService(HttpClient httpClient, ICacheService cache, ILogger<HackerNewsAPIService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ExternalStoryDto>> GetNewStoriesAsync() 
        {           

            if (!_cache.TryGetValue("stories", out List<ExternalStoryDto> storiesList))
            {
                // If the data is not cached, do some logic to get it
                storiesList = await GetExternalStoryList();

                // We store cache data with an expiration of 10 minutes.
                _cache.Set("stories", storiesList, TimeSpan.FromMinutes(30));
            }
            return storiesList;
        }
        private async Task<List<ExternalStoryDto>> GetExternalStoryList()
        {
        
            // Initialize a StoryDto list
            List<ExternalStoryDto> stories = new List<ExternalStoryDto>();

            try
            {
                // We make the call to the third-party API using HttpClient
                HttpResponseMessage response = await _httpClient.GetAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");

                if (response.IsSuccessStatusCode)
                {
                    // We read the response as a list of JSON strings
                    string json = await response.Content.ReadAsStringAsync();

                    List<long> storyIds = JsonConvert.DeserializeObject<List<long>>(json);
                    if (storyIds != null && storyIds.Count > 0) {
                        // We read the IDs and get the details of each story
                        foreach (long storyId in storyIds)
                        {
                            string storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty";
                            HttpResponseMessage storyResponse = await _httpClient.GetAsync(storyUrl);

                            if (storyResponse.IsSuccessStatusCode)
                            {
                                string storyJson = await storyResponse.Content.ReadAsStringAsync();
                                ExternalStoryDto story = JsonConvert.DeserializeObject<ExternalStoryDto>(storyJson);
                                stories.Add(story);
                            }
                            else
                            {
                                _logger.LogWarning("Error fetching individual story with ID {StoryId}. Status code: {StatusCode}", storyId, storyResponse.StatusCode);
                            }
                        }
                    }                  

                }
                else
                {
                    _logger.LogWarning("Error fetching list of story IDs. Status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
            }
            return stories;
        }

    }
}
