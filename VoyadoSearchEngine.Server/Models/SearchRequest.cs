namespace VoyadoSearchEngine.Server.Models
{
    public class SearchRequest
    {
        public required string Query { get; set; }

        public required string Engine { get; set; }
    }
}
