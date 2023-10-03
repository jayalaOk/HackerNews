using HackerNews.Application.QueryService.Queries;
using HackerNews.Infrastructure.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IMediator mediator;
        public StoriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Get all New Stories
        /// </summary>
        /// <returns>Returns new all Stories </returns>
        [HttpGet()]
        [ProducesResponseType(typeof(List<StoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllNewStories()
        {
            var query = new GetAllNewStoriesQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

    }
}
