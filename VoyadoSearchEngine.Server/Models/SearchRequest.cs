namespace VoyadoSearchEngine.Server.Models
{
    public record SearchRequest
    {
        public required string Query { get; set; }

        public required string Engine { get; set; }
    }
}
