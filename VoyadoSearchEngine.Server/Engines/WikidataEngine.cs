using System.Net;
using System.Text.Json;
using VoyadoSearchEngine.Server.Engines;

namespace VoyadoSearchEngine.Server.Engines
{
    public class WikidataEngine : ISearchEngine
    {
        private const string SearchUrlTemplate = "https://www.wikidata.org/w/api.php?action=query&list=search&srsearch={0}&format=json";

        private static readonly HttpClient Client = new()
        {
            DefaultRequestHeaders =
            {
                // Wikidata requires a User-Agent
                { "User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:145.0) Gecko/20100101 Firefox/145.0" }
            }
        };

        public string Name => "Wikidata";

        public async Task<int> SearchAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new Exception("Invalid word format.");

            var url = string.Format(SearchUrlTemplate, WebUtility.UrlEncode(word));
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await Client.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = TryParseError(json);
                throw new Exception(errorMessage ?? $"Wikidata error: {response.StatusCode}");
            }

            return ParseTotalHits(json);
        }

        private static string? TryParseError(string jsonContent)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                if (doc.RootElement.TryGetProperty("error", out var errorElement))
                {
                    return errorElement.GetString();
                }
            }
            catch (Exception)
            {
                // Ignore parse errors for error responses
            }

            return null;
        }

        private static int ParseTotalHits(string jsonContent)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                return doc.RootElement
                    .GetProperty("query")
                    .GetProperty("searchinfo")
                    .GetProperty("totalhits")
                    .GetInt32();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse JSON from Wikidata", ex);
            }
        }
    }
}