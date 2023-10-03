using Autofac.Extras.Moq;
using FluentAssertions;
using HackerNews.Api.Controllers;
using HackerNews.Application.QueryService.Queries;
using HackerNews.Infrastructure.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.Api.UnitTests
{
    [Trait("Category", "API")]
    public class StoriesControllerTests
    {
        [Fact]
        public async Task Get_AllNewStories_WithOutParameters_ReturnOk()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                //Arrange
                var expectedResult = this.GetAllStoriesList();

                autoMock.Mock<IMediator>()
                    .Setup(x => x.Send(It.IsAny<GetAllNewStoriesQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedResult);

                //Act
                var result = await autoMock.Create<StoriesController>().GetAllNewStories();

                //Assert
                result.Should().BeOfType<OkObjectResult>();
                result.As<OkObjectResult>().Value.Should().Be(expectedResult);
                autoMock.Mock<IMediator>()
                    .Verify(x => x.Send(It.IsAny<GetAllNewStoriesQuery>(), It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }

		#region Private Methods
		public List<StoryDto> GetAllStoriesList()
		{
            var dtoList = new List<StoryDto>
            {
                new StoryDto
                {
                    Id = 1,
                    Url = "https://www.infobae.com/america/",
                    Title = "388 REPUBLIC SERVICES-NEW"
                },
                new StoryDto
                {
                    Id = 2,
                    Url = "https://www.batimes.com.ar/topics/street",                    
                    Title = "BANK OF AMERICA",                    
                }
            };
            return dtoList;
		}
		#endregion
	}
}
