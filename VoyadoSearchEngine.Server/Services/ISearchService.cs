using VoyadoSearchEngine.Server.Engines;
using VoyadoSearchEngine.Server.Models;

namespace VoyadoSearchEngine.Server.Services
{
    public interface ISearchService
    {
        public IEnumerable<string> GetEngines();
        public ISearchEngine? FindEngine(string name);
        public Task<EngineResult> CountSearchHits(string[] words, ISearchEngine engine);
    }
}
