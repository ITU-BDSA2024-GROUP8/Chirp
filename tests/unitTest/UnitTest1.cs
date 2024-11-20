using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;

namespace unitTest;

public class UnitTest1
{
    private ChirpDBContext _context;
    private ICheepQueryRepository _cheepQueryRepository;
    private ICheepCommandRepository _cheepCommandRepository;
    private ICheepService _cheepService;

    public UnitTest1()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChirpDBContext(options);
        _cheepQueryRepository = new CheepQueryRepository(_context);
        _cheepCommandRepository = new CheepCommandRepository(_context, _cheepQueryRepository);
        _cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);
    }

    [Theory]
    [InlineData("TestUser1")]
    public async Task Test_FindAuthorByName(string authorName)
    {
        // Arrange
        var author = await _cheepCommandRepository.NewAuthorAsync(authorName, "test1@example.com");

        // Act
        var authorByName = await _cheepQueryRepository.GetAuthorByNameAsync(authorName);

        // Assert
        Assert.Equal(authorName, authorByName?.Name);
    }

    [Theory]
    [InlineData("Test1@example.com")]
    public async Task Test_FindAuthorByEmail(string email)
    {
        // Arrange
        var author = await _cheepCommandRepository.NewAuthorAsync("TestUser", email);

        // Act
        var authorByEmail = await _cheepQueryRepository.GetAuthorByEmailAsync(email);

        // Assert
        Assert.Equal(email, authorByEmail?.Email);
    }

    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        // Arrange
        string testName = "testAuthor";
        string testEmail = "testAuthor@email.com";

        // Act
        var author = await _cheepCommandRepository.NewAuthorAsync(testName, testEmail);
        var authorByName = await _cheepQueryRepository.GetAuthorByNameAsync(testName);

        // Assert
        Assert.NotNull(authorByName);
        Assert.Equal(testName, authorByName.Name);
        Assert.Equal(testEmail, authorByName.Email);
    }

    [Fact]
    public async Task Test_CreateNewCheep()
    {
        // Arrange
        var author = await _cheepCommandRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");
        string testMessage = "This is a new test cheep";

        // Act
        var cheepsBeforePost = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author.Name);
        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, testMessage);
        var cheepsAfterPost = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author.Name);

        // Assert
        Assert.Empty(cheepsBeforePost);
        Assert.Single(cheepsAfterPost);
        Assert.Equal(testMessage, cheepsAfterPost.First().Message);
    }

    [Fact]
    public async Task Test_FollowAndUnfollowAuthor()
    {
        // Arrange
        var author1 = await _cheepCommandRepository.NewAuthorAsync("author1", "author1@example.com");
        var author2 = await _cheepCommandRepository.NewAuthorAsync("author2", "author2@example.com");

        // Act & Assert - Follow
        await _cheepCommandRepository.FollowAuthorAsync(author1.Id, author2.Id);
        var isFollowing = await _cheepCommandRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.True(isFollowing);

        // Act & Assert - Unfollow
        await _cheepCommandRepository.UnfollowAuthorAsync(author1.Id, author2.Id);
        isFollowing = await _cheepCommandRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.False(isFollowing);
    }

    [Fact]
    public async Task Test_GetCheepsFromUserTimeline()
    {
        // Arrange
        var author1 = await _cheepCommandRepository.NewAuthorAsync("author1", "author1@example.com");
        var author2 = await _cheepCommandRepository.NewAuthorAsync("author2", "author2@example.com");
        
        // Create cheeps for both authors
        await _cheepCommandRepository.NewCheepAsync(author1.Name, author1.Email!, "Test cheep 1 from author1");
        await _cheepCommandRepository.NewCheepAsync(author2.Name, author2.Email!, "Test cheep 1 from author2");
        
        // Follow author2
        await _cheepCommandRepository.FollowAuthorAsync(author1.Id, author2.Id);

        // Act
        var timeline = await _cheepQueryRepository.GetCheepsFromUserTimelineAsync(1, author1.Name);

        // Assert
        Assert.Equal(2, timeline.Count); // Should see both their own cheep and the followed author's cheep
    }
}