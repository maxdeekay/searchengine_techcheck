using Microsoft.Playwright;

namespace VoyadoSearchEngine.Server.Engines
{
    public class GoogleEngine : ISearchEngine
    {
        private readonly IBrowser _browser;

        public GoogleEngine(IBrowser browser)
        {
            _browser = browser;
        }

        public string Name => "Google";

        public async Task<int> SearchAsync(string word)
        {
            return 0;

            var page = await _browser.NewPageAsync();
            await page.GotoAsync($"https://www.bing.com/search?q={Uri.EscapeDataString(word)}");
            var element = await page.QuerySelectorAsync(".sb_count");
            if (element == null) return 0;

            var text = await element.InnerTextAsync();
            var digits = new string(text.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var hits) ? hits : 0;
        }
    }
}
