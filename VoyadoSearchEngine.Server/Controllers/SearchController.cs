using Microsoft.AspNetCore.Mvc;
using VoyadoSearchEngine.Server.Models;
using VoyadoSearchEngine.Server.Services;

namespace VoyadoSearchEngine.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("engines")]
        public IActionResult Get()
        {
            var engines = _searchService.GetEngines();
            return Ok(engines);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Engine))
                return BadRequest("An engine must be selected.");

            var result = await _searchService.SearchAsync(request.Query, request.Engine);

            return Ok(result);
        }
    }
}