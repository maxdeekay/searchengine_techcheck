namespace VoyadoSearchEngine.Server.Models
{
    public record WordResult
    {
        public int Hits { get; set; }

        public string? ErrorMessage { get; set; }
    }
}