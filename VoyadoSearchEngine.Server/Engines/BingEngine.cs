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

        // NOTE: This Playwright setup times out and fails about 50% of the time but left it in for display purposes
        public async Task<int> SearchAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new Exception("Invalid word format.");

            var page = await _browser.NewPageAsync();
            
            try
            {
                // Navigate to Bing search results page and wait until page is fully loaded before continuing
                await page.GotoAsync(
                    $"https://www.bing.com/search?q={Uri.EscapeDataString(word)}",
                    new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle }
                );

                var element = page.Locator("span.sb_count");

                // Wait for element to be visible on the page
                // Can sometimes timeout because Playwright considers it "not visible" eg. off screen/hidden etc.
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
                // Finally close the page to free up resources
                await page.CloseAsync();
            }
        }
    }
}