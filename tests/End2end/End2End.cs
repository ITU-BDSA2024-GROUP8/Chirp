using Microsoft.Playwright;
using NUnit.Framework;

namespace Chirp.Tests
{
    [TestFixture]
    public class End2End
    {
        private TestFixture<Program> _fixture = null!;
        private string email = "end2end@example.com";
        private string password = "End2end1234!";
        private string username = "End2EndUser";
        private HttpClient client = null!;

        private IPlaywright playwright = null!;
        private IBrowser browser = null!;
        private IBrowserContext context = null!;
        private IPage page = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _fixture = new TestFixture<Program>();
            await _fixture.EnsureServerIsReady();

            // Use the client from the fixture
            client = _fixture.Client;

            // Initialize Playwright
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false // Set to false if you want to see the browser UI
            });
            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            _fixture.StopServer();
            if (browser != null)
            {
                await browser.CloseAsync();
            }

        }
      
        [Test, Order(1)]
        public async Task Test_register()
        {
            // Navigate to the registration page
            var registerUrl = new Uri(client.BaseAddress, "/Identity/Account/Register");
            await page.GotoAsync(registerUrl.ToString());

            // Fill in the registration form
            await page.FillAsync("input[name='Input.Name']", username);
            await page.FillAsync("input[name='Input.Email']", email);
            await page.FillAsync("input[name='Input.Password']", password);
            await page.FillAsync("input[name='Input.ConfirmPassword']", password);

            // Submit the registration form
            await page.ClickAsync("button[type='submit'][id='registerSubmit']");

            // Wait for navigation to complete
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify that the user is redirected to the home page
            var currentUrl = page.Url;
            NUnit.Framework.Assert.That(currentUrl, Is.EqualTo(client.BaseAddress.ToString()));
        }

        [Test, Order(2)]
        public async Task Test_cheep()
        {
            // Submit a new cheep
            var cheepMessage = "Hello, world!";
            await page.FillAsync("input[name='Message']", cheepMessage);
            await page.ClickAsync("input[type='submit']");

            // Wait for the cheep to appear
            await page.WaitForSelectorAsync($"text={cheepMessage}");

            // Verify the cheep is displayed
            var cheepText = await page.InnerTextAsync($"text={cheepMessage}");
            NUnit.Framework.Assert.That(cheepText, Does.Contain(cheepMessage));
        }

        [Test, Order(3)]
        public async Task Test_achievements()
        {
            // Check for achievements
            var myTimeLine = new Uri(client.BaseAddress, "/" + username);
            await page.GotoAsync(myTimeLine.ToString());
            var achievements = await page.InnerTextAsync(".achievement-heading");
            await page.WaitForSelectorAsync($"text={achievements}");
            NUnit.Framework.Assert.That(achievements, Does.Contain("Novice Cheepster"));
        }

        [Test, Order(4)]
        public async Task Test_delete_account()
        {
            // Delete the account
            await page.GotoAsync(new Uri(client.BaseAddress, "/AboutMe").ToString());
            await page.ClickAsync("button[type='submit'][name='forgetMeBTN']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify the account is deleted
            var loginUrl = new Uri(client.BaseAddress, "/Identity/Account/Login");
            await page.GotoAsync(loginUrl.ToString());
            await page.FillAsync("input[name='Input.Email']", email);
            await page.FillAsync("input[name='Input.Password']", password);
            await page.ClickAsync("button[type='submit']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var errorMessage = await page.InnerTextAsync(".validation-summary-errors");
            NUnit.Framework.Assert.That(errorMessage, Does.Contain("Invalid login attempt."));
        }

    }
}
