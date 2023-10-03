using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using HackerNews.Application.QueryService.Queries;
using HackerNews.Application.QueryService.QueryHandlers;
using HackerNews.Infrastructure.Abstractions;
using HackerNews.Infrastructure.Dtos;
using HackerNews.Infrastructure.ExternalDtos;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.Application.UnitTests
{
    [Trait("Category", "Query Handler")]
    public class GetAllStoriesQueryHandlerTests 
    {
        public GetAllStoriesQueryHandlerTests() {  
        
        }

        [Fact]
        public async Task GetAllNewStories()
        {
            using (var automock = AutoMock.GetLoose())
            {
                //Arrange
                var mockMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ExternalStoryDto,StoryDto>();
                });
                var mapper = mockMapper.CreateMapper();
                var parameters = new List<NamedParameter>
                {
                    new NamedParameter("mapper",mapper)
                }.ToArray();

                automock.Mock<IHackerNewsAPIService>().Setup(x=>x.GetNewStoriesAsync())
               .ReturnsAsync(this.GetAllExternalStoriesList());

                var query = new GetAllNewStoriesQuery();

                //Act
                var result = await automock.Create<GetAllNewStoriesQueryHandler>(parameters)
                    .Handle(query, It.IsAny<CancellationToken>());

                //Assert
                Assert.NotEmpty(result);
                automock.Mock<IHackerNewsAPIService>().Verify(x => x.GetNewStoriesAsync(),Times.Once);
            }

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
