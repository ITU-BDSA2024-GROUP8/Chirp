using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Xunit;


namespace Chirp.Tests
{
    public class End2End : IClassFixture<TestFixture<Program>>, IAsyncLifetime
    {
        private readonly TestFixture<Program> _fixture;

        public End2End(TestFixture<Program> fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.EnsureServerIsReady();
        }
        
        public Task DisposeAsync()
        {
            _fixture.StopServer();
            return Task.CompletedTask;
            
        }

        [Theory]
        [InlineData("end2end@example.com", "End2end1234!", "End2EndUser")]
        public async Task Test_FullUserJourney(string email, string password, string username)
        {
            // Use the client from the fixture
            var client = _fixture.Client;

            // Initialize Playwright
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false // Set to false if you want to see the browser UI
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // Navigate to the login page
            if (client.BaseAddress != null)
            {
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
                Assert.Equal(client.BaseAddress.ToString(), currentUrl);

                // Submit a new cheep
                var cheepMessage = "Hello, world!";
                await page.FillAsync("input[name='Message']", cheepMessage);
                await page.ClickAsync("input[type='submit']");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Verify the cheep is displayed
                var cheepText = await page.InnerTextAsync($"text={cheepMessage}");
                Assert.Contains(cheepMessage, cheepText);
                
                // Check for achievements
                var myTimeLine = new Uri(client.BaseAddress, "/"+username);
                await page.GotoAsync(myTimeLine.ToString());
                await page.Locator(".achievement-heading:has-text('Novice Cheepster')").WaitForAsync();
                var achievements = await page.InnerTextAsync(".achievement-heading");
                Assert.Contains("Novice Cheepster", achievements);

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
                Assert.Contains("Invalid login attempt.", errorMessage);
            }
            else
            {
                throw new InvalidOperationException("BaseAddress is null.");
            }

            // Close the browser
            await browser.CloseAsync();
        }
    }
}