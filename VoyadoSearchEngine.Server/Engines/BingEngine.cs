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

        // NOTE:
        // There's about a 50% risk that this method runs into a CAPTCHA and fails, still left it in for theoretical reasons
        // Added some example code on how the CAPTCHA could be handled
        // Bing also doesn't show exact search hits, only rounded numbers
        public async Task<int> SearchAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new Exception("Invalid word format.");

            var page = await _browser.NewPageAsync();
            
            try
            {
                await page.GotoAsync(
                    $"https://www.bing.com/search?q={Uri.EscapeDataString(word)}",
                    new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded }
                );

                await Task.Delay(500);

                // Check for CAPTCHA
                var captchaElement = page.Locator("#turnstile-widget");
                var captchaCount = await captchaElement.CountAsync();

                if (captchaCount > 0)
                {
                    throw new Exception("Request encountered a CAPTCHA and can't continue.");
                }

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
                // Finally close the page to free up resources
                await page.CloseAsync();
            }
        }
    }
}