using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VirtusaTest.Models;
using VirtusaTest.Services;

namespace VirtusaTest.Controllers
{
    [Route("api/stories")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly NewsService _newsService;
        private readonly IMemoryCache _cache;
        public TestController(NewsService newsService, IMemoryCache cache)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(NewsService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public async Task<IActionResult> GetBestStories(int n = 10)
        {
            try
            {
                if (n <= 0)
                {
                    return BadRequest("Invalid value for 'n'. 'n' should be a positive integer.");
                }

                if (!_cache.TryGetValue("BestStories", out List<StoryModel> bestStories))
                {

                    var storyIds = await _newsService.GetBestStoryIdsAsync();

                    bestStories = new List<StoryModel>();

                    foreach (var storyId in storyIds.Take(n))
                    {
                        var storyDetails = await _newsService.GetStoryDetailsAsync(storyId);
                        bestStories.Add(new StoryModel
                        {
                            Title = storyDetails.Title,
                            Uri = storyDetails.Uri,
                            PostedBy = storyDetails.PostedBy,
                            Time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(storyDetails.Time)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            Score = storyDetails.Score,
                            CommentCount = storyDetails.CommentCount
                        });
                    }

                    _cache.Set("BestStories", bestStories, TimeSpan.FromMinutes(5));
                }

                return Ok(bestStories.OrderByDescending(s => s.Score));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed.
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}

