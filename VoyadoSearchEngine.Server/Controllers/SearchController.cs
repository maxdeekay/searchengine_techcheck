using Microsoft.AspNetCore.Mvc;
using VoyadoSearchEngine.Server.Engines;
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
            // Would move this into a separate validator class with further development

            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Engine))
                return BadRequest("An engine must be selected.");

            var words = request.Query.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return BadRequest("No valid words found in the query.");

            if (words.Any(w => w.Length > 20))
                return BadRequest("Words cannot be longer than 20 characters.");

            var selectedEngine = _searchService.FindEngine(request.Engine);

            if (selectedEngine == null)
                return BadRequest($"Couldn't find engine: {request.Engine}");

            var result = await _searchService.CountSearchHits(words, selectedEngine);
            return Ok(result);
        }
    }
}