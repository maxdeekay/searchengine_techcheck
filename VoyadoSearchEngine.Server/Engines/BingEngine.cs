using Microsoft.Playwright;

namespace VoyadoSearchEngine.Server.Engines
{
    public class BingEngine : ISearchEngine
    {
        private readonly IBrowser _browser;

        public BingEngine(IBrowser browser)
        {
            _browser = browser;
        }

        public string Name => "Bing";

        public async Task<int> SearchAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new Exception("Invalid word format.");

            var page = await _browser.NewPageAsync();
            
            try
            {
                await page.GotoAsync(
                    $"https://www.bing.com/search?q={Uri.EscapeDataString(word)}",
                    new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle }
                );

                var element = page.Locator("span.sb_count");

                await element.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                if (element == null)
                    throw new Exception("Could not find element.");

                var text = await element.InnerTextAsync();
                var digits = new string(text.Where(char.IsDigit).ToArray());

                if (!int.TryParse(digits, out var hits))
                    throw new Exception("Could not parse digits.");

                return hits;
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}