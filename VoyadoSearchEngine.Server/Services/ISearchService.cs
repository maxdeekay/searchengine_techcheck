using VoyadoSearchEngine.Server.Models;

namespace VoyadoSearchEngine.Server.Services
{
    public interface ISearchService
    {
        public Task<EngineResult> SearchAsync(string query, string engine);
        public IEnumerable<string> GetEngines();
    }
}
