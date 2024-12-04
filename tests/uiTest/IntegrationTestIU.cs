using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Xunit;

namespace uiIntegrationTest
{
    public class IntegrationTest : IClassFixture<TestFixture<Program>>, IAsyncLifetime
    {
        private readonly TestFixture<Program> _fixture;

        public IntegrationTest(TestFixture<Program> fixture)
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
        [InlineData("adho@itu.dk", "M32Want_Access")]
        public async Task Test_LoginUserStandart(string email, string password)
        {

            // Use the client from the fixture
            var client = _fixture.Client;

            // Initialize Playwright
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Set to false if you want to see the browser UI
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();


            // Navigate to the login page
            if (client.BaseAddress != null)
            {
                var loginUrl = new Uri(client.BaseAddress, "/Identity/Account/Login");
                await page.GotoAsync(loginUrl.ToString());

                // Fill in the login form
                await page.FillAsync("input[name='Input.Email']", email);
                await page.FillAsync("input[name='Input.Password']", password);

                // Submit the login form
                await page.ClickAsync("button[type='submit']");

                // Wait for navigation to complete
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Navigate to the login page again to check for redirection
                await page.GotoAsync(loginUrl.ToString());
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Verify that the user is redirected to the home page
                // which means the user is logged in since you can't access the login page when logged in
                var currentUrl = page.Url;
                Assert.Equal(client.BaseAddress.ToString(), currentUrl);
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