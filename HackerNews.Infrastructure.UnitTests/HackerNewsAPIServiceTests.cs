using HackerNews.Infrastructure.Abstractions;
using HackerNews.Infrastructure.ExternalDtos;
using HackerNews.Infrastructure.ExternalServices;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.Infrastructure.UnitTests
{
    [Trait("Category", "Services")]
    public class HackerNewsAPIServiceTests
    {
        [Fact]
        public async Task GetNewStoriesAsync_CacheHit_ReturnsCachedData()
        {
            // Arrange

            object cachedStories = this.GetAllExternalStoriesList();
            var mockCache = new Mock<ICacheService>();
            mockCache.Setup(c => c.TryGetValue("stories", out cachedStories)).Returns(true);
            var mockLogger = new Mock<ILogger<HackerNewsAPIService>>();
            var httpClientFactoryMock = new Mock<IHttpClientFactoryWrapper>();
            var apiService = new HackerNewsAPIService(httpClientFactoryMock.Object, mockCache.Object, mockLogger.Object);

            // Act
            var result = await apiService.GetNewStoriesAsync();

            // Assert
            Assert.Equal(cachedStories, result);
            mockCache.Verify(repo => repo.TryGetValue("stories", out cachedStories), Times.Once());
        }

      

        [Fact]
        public async Task GetNewStoriesAsync_WithoutCachedData_ReturnsExternalStoryList()
        {
            // Arrange
            var cacheMock = new Mock<ICacheService>();
            object expectedValue = null;
            cacheMock.Setup(cache => cache.TryGetValue("stories", out expectedValue))
                .Returns(false);

            var httpClientFactoryMock = new Mock<IHttpClientFactoryWrapper>();


            var firstResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK, //Simulate a Ok API call
                Content = new StringContent(JsonConvert.SerializeObject(new List<long> { 8863 }), Encoding.UTF8, "application/json"),
            };
            var secondResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK, //Simulate a Ok API call
                Content = new StringContent(JsonConvert.SerializeObject(new ExternalStoryDto
                {
                    By = "dhouston",
                    Descendants = 71,
                    Id = 8863,
                    Kids = null,
                    Score = 111,
                    Time = 1175714200,
                    Title = "My YC app: Dropbox - Throw away your USB drive",
                    Type = "story",
                    Url = "http://www.getdropbox.com/u/2/screencast.html"
                }), Encoding.UTF8, "application/json"),
            };


            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(firstResponse).ReturnsAsync(secondResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactoryMock.Setup(factory => factory.CreateClient()).Returns(httpClient);

            var loggerMock = new Mock<ILogger<HackerNewsAPIService>>();
            
            var service = new HackerNewsAPIService(httpClientFactoryMock.Object, cacheMock.Object, loggerMock.Object);
            
            // Act
            var result = await service.GetNewStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            cacheMock.Verify(repo => repo.TryGetValue("stories", out expectedValue), Times.Once());
            cacheMock.Verify(repo => repo.Set("stories", It.IsAny<List<ExternalStoryDto>>(), TimeSpan.FromMinutes(30)), Times.Once());
        }
              

        [Fact]
        public async Task GetNewStoriesAsync_CacheMiss_ApiCallFailure_ReturnsEmptyList()
        {

            // Arrange
            var firstResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError, // Simulate a failed API call
            };

            object cachedStories = null;
            var mockCache = new Mock<ICacheService>();

            object expectedValue = null;
            mockCache.Setup(c => c.TryGetValue("stories", out expectedValue)).Returns(false);
            mockCache.Setup(c => c.Set("stories", It.IsAny<List<ExternalStoryDto>>(), TimeSpan.FromMinutes(30))).Returns(It.IsAny<List<ExternalStoryDto>>());

            var httpClientFactoryMock = new Mock<IHttpClientFactoryWrapper>();


            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(firstResponse).ReturnsAsync(firstResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactoryMock.Setup(factory => factory.CreateClient()).Returns(httpClient);

            var mockLogger = new Mock<ILogger<HackerNewsAPIService>>();
            var apiService = new HackerNewsAPIService(httpClientFactoryMock.Object, mockCache.Object, mockLogger.Object);

            // Act
            var result = await apiService.GetNewStoriesAsync();

            // Assert
            Assert.Empty(result);
            mockCache.Verify(repo => repo.TryGetValue("stories", out expectedValue), Times.Once());
            mockCache.Verify(repo => repo.Set("stories", It.IsAny<List<ExternalStoryDto>>(), TimeSpan.FromMinutes(30)), Times.Once());


        }
        #region Private Methods      

        public List<ExternalStoryDto> GetAllExternalStoriesList()
        {
            var dtoList = new List<ExternalStoryDto>
            {
                new ExternalStoryDto
                {
                    By = "dhouston",
                    Descendants= 71,
                    Id = 8863,
                    Kids = null,
                    Score=111,
                    Time=1175714200,
                    Title="My YC app: Dropbox - Throw away your USB drive",
                    Type = "story",
                    Url= "http://www.getdropbox.com/u/2/screencast.html"
                },
                new ExternalStoryDto
                {
                    By = "norvig",
                    Descendants= 71,
                    Id = 2921983,
                    Kids = null,
                    Score=111,
                    Time=1314211127,
                    Title="Justin.tv is looking for a Lead Flash Engineer!",
                    Type = "story",
                    Url= ""
                }
            };
            return dtoList;
        }
        #endregion
    }
}