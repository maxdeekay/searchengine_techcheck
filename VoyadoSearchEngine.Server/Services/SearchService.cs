using VoyadoSearchEngine.Server.Models;
using VoyadoSearchEngine.Server.Engines;

namespace VoyadoSearchEngine.Server.Services
{
    public class SearchService : ISearchService
    {
        private readonly IEnumerable<ISearchEngine> _engines;

        public SearchService(IEnumerable<ISearchEngine> engines)
        {
            _engines = engines;
        }

        public IEnumerable<string> GetEngines()
        {
            return _engines.Select(e => e.Name);
        }

        public ISearchEngine? FindEngine(string name)
        {
            return _engines.FirstOrDefault(e => e.Name == name);
        }

        public async Task<EngineResult> CountSearchHits(string[] words, ISearchEngine engine)
        {
            // Query the engine word for word
            var tasks = words.Select(async word =>
            {
                try
                {
                    var hits = await engine.SearchAsync(word);
                    return new { Word = word, Result = new WordResult { Hits = hits } };
                }
                catch (Exception ex)
                {
                    return new { Word = word, Result = new WordResult { ErrorMessage = ex.Message } };
                }
            });

            var results = await Task.WhenAll(tasks);

            var wordResults = results.ToDictionary(r => r.Word, r => r.Result);

            return new EngineResult
            {
                Name = engine.Name,
                TotalHits = wordResults.Sum(w => w.Value.Hits),
                WordResults = wordResults
            };
        }
    }
}