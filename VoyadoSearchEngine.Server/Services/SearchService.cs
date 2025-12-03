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

        public async Task<EngineResult> SearchAsync(string query, string engine)
        {
            string[] words = query.Split(" ");
            var selectedEngine = _engines.FirstOrDefault(e => e.Name == engine);

            if (selectedEngine == null)
                throw new Exception($"Couldn't find engine: {engine}");

            // Query the engine with every word
            var searchTasks = words.Select(async word =>
            {
                try
                {
                    var hits = await selectedEngine.SearchAsync(word);
                    return new { Word = word, Result = new WordResult { Hits = hits } };
                }
                catch (Exception ex)
                {
                    return new { Word = word, Result = new WordResult { ErrorMessage = ex.Message } };
                }
            });

            var results = await Task.WhenAll(searchTasks);

            var wordResults = results.ToDictionary(r => r.Word, r => r.Result);

            return new EngineResult
            {
                Name = engine,
                TotalHits = wordResults.Sum(w => w.Value.Hits ?? 0),
                WordResults = wordResults
            };
        }
    }
}