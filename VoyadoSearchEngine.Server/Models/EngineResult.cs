namespace VoyadoSearchEngine.Server.Models
{
    public record EngineResult
    {
        public required string Name { get; set; }

        public int TotalHits { get; set; }

        public Dictionary<string, WordResult> WordResults { get; set; } = new();
    }
}