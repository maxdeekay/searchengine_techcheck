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

        public async Task<EngineResult> SearchAsync(string query, string engine)
        {
            string[] words = query.Split(" ");
            var selectedEngine = _engines.FirstOrDefault(e => e.Name == engine);

            if (selectedEngine == null)
                throw new Exception($"Couldn't find engine: {engine}");

            var wordResults = new Dictionary<string, WordResult>();

            // Query the engine with every word
            foreach (var word in words)
            {
                try
                {
                    var hits = await selectedEngine.SearchAsync(word);
                    wordResults.Add(word, new WordResult { Hits = hits });
                }
                catch (Exception ex)
                {
                    wordResults.Add(word, new WordResult { ErrorMessage = ex.Message });
                }
            }

            return new EngineResult()
            {
                Name = engine,
                TotalHits = wordResults.Sum(w => w.Value.Hits ?? 0),
                WordResults = wordResults
            };
        }
    }
}