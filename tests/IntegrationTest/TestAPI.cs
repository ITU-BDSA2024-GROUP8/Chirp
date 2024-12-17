using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Chirp.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Util;

namespace apiTest;

public class TestAPI : IClassFixture<IntegrationTestFixture<Program>>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture<Program> _factory;

    public TestAPI(IntegrationTestFixture<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
    
    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
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
        using (var scope = _factory.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetRequiredService<ChirpDBContext>();

            var author1 = new Author() { Name = "Helge", UserName = "ropf@itu.dk", Email = "ropf@itu.dk", EmailConfirmed = true, Cheeps = new List<Cheep>(), Followers = new List<AuthorFollower>(), Following = new List<AuthorFollower>(), Bio = "Pro golfer"};
            var author2 = new Author() { Name = "Adrian", UserName = "adho@itu.dk", Email = "adho@itu.dk", EmailConfirmed = true, Cheeps = new List<Cheep>(), Followers = new List<AuthorFollower>(), Following = new List<AuthorFollower>(), Bio = "Je suis..."};
            
            dbContext.Authors.AddRange(author1, author2);
            await dbContext.SaveChangesAsync();
        }
        
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}