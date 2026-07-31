using Baseera.Service.Connectors.Reddit.Abstractions;
using Baseera.Service.Connectors.Reddit.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Baseera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedditTestController : ControllerBase
    {
        private readonly IRedditSearchClient _searchClient;

        public RedditTestController(IRedditSearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            var request = new SearchPostsRequest
            {
                Query = "iphone",
                Limit = 3,
                Sort = "relevance",
                TimeFilter = "all"
            };

            var result = await _searchClient.SearchAsync(request);

            return Ok(result);
        }
    }
}
