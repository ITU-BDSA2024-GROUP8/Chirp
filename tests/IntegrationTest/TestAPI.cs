using Chirp.Tests;
using Util;

namespace apiTest;

using Microsoft.AspNetCore.Mvc.Testing;
public class TestAPI
{
    [Fact]
    public async void CanSeePublicTimeline()
    {
        var fixture = new TestFixture<Program>();
        var client = fixture.CreateClient();
        
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        var fixture = new IntegrationTestFixture<Program>();
        var client = fixture.CreateClient();
        
        var response = await client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}