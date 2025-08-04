using HackerNews.Model;
using HackerNews.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public StoriesController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewStories(int page = 1, int pageSize = 10, string search = "")
        {
            return Ok(await _hackerNewsService.GetNewStoriesAsync(page, pageSize, search));
        }
    }
}