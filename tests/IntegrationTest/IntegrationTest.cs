using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;

namespace IntegrationTest;

public class IntegrationTest
{
    private ChirpDBContext _context;
    private ICheepQueryRepository _cheepQueryRepository;
    private ICheepCommandRepository _cheepCommandRepository;
    private ICheepService _cheepService;

    private async Task InitializeTestEnvironment()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChirpDBContext(options);
        _cheepQueryRepository = new CheepQueryRepository(_context);
        _cheepCommandRepository = new CheepCommandRepository(_context, _cheepQueryRepository);
        _cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);

        // Set up initial test data
        var author = await _cheepCommandRepository.NewAuthorAsync("TestUser1", "Test1@exsample.dk");
        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, "Hello World!");
        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, "Second test cheep");
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {
        // Arrange
        await InitializeTestEnvironment();

        // Act
        var cheeps = await _cheepService.GetCheeps(1);
        var cheepsFromAuthor = await _cheepService.GetCheepsFromAuthor(1, author);

        // Assert
        Assert.Equal(2, cheeps.Count);
        Assert.Contains(cheepsFromAuthor, c => c.Message == message);
        Assert.All(cheepsFromAuthor, c => Assert.Equal(author, c.AuthorName));
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingRepositories(string author, string message)
    {
        // Arrange
        await InitializeTestEnvironment();

        // Act
        var cheeps = await _cheepQueryRepository.GetCheepsAsync(1);
        var cheepsFromAuthor = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author);

        // Assert
        Assert.Equal(2, cheeps.Count);
        Assert.Contains(cheepsFromAuthor, c => c.Message == message);
        Assert.All(cheepsFromAuthor, c => Assert.Equal(author, c.AuthorName));
    }

    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_GetAuthor(string author, string email)
    {
        // Arrange
        await InitializeTestEnvironment();

        // Act
        var authorByName = await _cheepQueryRepository.GetAuthorByNameAsync(author);
        var authorByEmail = await _cheepQueryRepository.GetAuthorByEmailAsync(email);

        // Assert
        Assert.Equal(author, authorByName?.Name);
        Assert.Equal(email, authorByEmail?.Email);
    }

    [Theory]
    [InlineData("TestUser1", "Test1@exsample.dk")]
    public async Task Test_CreateCheep(string author, string email)
    {
        // Arrange
        await InitializeTestEnvironment();
        
        // Act - Get initial cheeps
        var initialCheeps = await _cheepQueryRepository.GetCheepsAsync(1);
        Assert.Equal(2, initialCheeps.Count);

        // Act - Create new cheep
        await _cheepCommandRepository.NewCheepAsync(author, email, "New Cheep!");
        var updatedCheeps = await _cheepQueryRepository.GetCheepsAsync(1);

        // Assert
        Assert.Equal(3, updatedCheeps.Count);
        Assert.Contains(updatedCheeps, c => c.Message == "New Cheep!");
    }

    [Fact]
    public async Task Test_UserTimeline()
    {
        // Arrange
        await InitializeTestEnvironment();
        
        // Create second user and follow first user
        var follower = await _cheepCommandRepository.NewAuthorAsync("Follower", "follower@example.com");
        var targetAuthor = await _cheepQueryRepository.GetAuthorByNameAsync("TestUser1");
        await _cheepCommandRepository.FollowAuthorAsync(follower.Id, targetAuthor!.Id);

        // Act
        var timeline = await _cheepQueryRepository.GetCheepsFromUserTimelineAsync(1, follower.Name);

        // Assert
        Assert.Equal(2, timeline.Count); // Should see the two cheeps from followed user
        Assert.All(timeline, c => Assert.Equal("TestUser1", c.AuthorName));
    }

    [Fact]
    public async Task Test_FollowUnfollowIntegration()
    {
        // Arrange
        await InitializeTestEnvironment();
        
        var follower = await _cheepCommandRepository.NewAuthorAsync("Follower", "follower@example.com");
        var targetAuthor = await _cheepQueryRepository.GetAuthorByNameAsync("TestUser1");

        // Act & Assert - Follow
        await _cheepCommandRepository.FollowAuthorAsync(follower.Id, targetAuthor!.Id);
        var isFollowing = await _cheepCommandRepository.IsFollowingAsync(follower.Id, targetAuthor.Id);
        Assert.True(isFollowing);

        // Act & Assert - Unfollow
        await _cheepCommandRepository.UnfollowAuthorAsync(follower.Id, targetAuthor.Id);
        isFollowing = await _cheepCommandRepository.IsFollowingAsync(follower.Id, targetAuthor.Id);
        Assert.False(isFollowing);
    }
}