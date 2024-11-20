namespace unitTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Util;

public class UnitTest1
{
    private ICheepQueryRepository _cheepQueryRepository;
    private ICheepCommandRepository _cheepCommandRepository;
    private ICheepService _cheepService;

    public UnitTest1()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: "ChirpTestDb")
            .Options;

        var context = new ChirpDBContext(options);

        _cheepQueryRepository = new CheepQueryRepository(context);
        _cheepCommandRepository = new CheepCommandRepository(context, _cheepQueryRepository);
        _cheepService = new CheepService(_cheepQueryRepository, _cheepCommandRepository);
    }

    [Theory]
    [InlineData("TestUser1")]
    public async Task Test_FindAuthorByName(string authorName)
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        var authorByName = await _cheepQueryRepository.GetAuthorByNameAsync(authorName);

        Assert.Equal(authorName, authorByName?.Name);
    }

    [Theory]
    [InlineData("Test1@exsample.dk")]
    public async Task Test_FindAuthorByEmail(string email)
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        var authorByEmail = await _cheepQueryRepository.GetAuthorByEmailAsync(email);

        Assert.Equal(email, authorByEmail?.Email);
    }

    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        var author = await _cheepCommandRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");
        var authorByName = await _cheepQueryRepository.GetAuthorByNameAsync(author.Name);

        Assert.Equal(author, authorByName);
    }

    [Fact]
    public async Task Test_CreateNewCheep()
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        var author = await _cheepCommandRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");

        var cheepsFromAuthor = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author.Name);

        Assert.Empty(cheepsFromAuthor);

        await _cheepCommandRepository.NewCheepAsync(author.Name, author.Email!, "This is a new test cheep");

        var newCheepsFromAuthor = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author.Name);

        Assert.Single(newCheepsFromAuthor);
    }

    [Fact]
    public async Task Test_CheepsForACertainPage()
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(2);

        // Create the service
        var cheepsOnPage1 = await _cheepQueryRepository.GetCheepsAsync(1);
        var cheepsOnPage2 = await _cheepQueryRepository.GetCheepsAsync(2);

        Assert.Equal(32, cheepsOnPage1.Count);
        Assert.Equal(8, cheepsOnPage2.Count);
    }

    [Fact]
    public async Task Test_CheepsForACertainPageByAuthor()
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(2);

        // Create the service
        var author = await _cheepQueryRepository.GetAuthorByNameAsync("Roger Histand");

        Assert.NotNull(author);

        var cheepsOnPage = await _cheepQueryRepository.GetCheepsFromAuthorAsync(1, author.Name);

        foreach (var cheep in cheepsOnPage)
        {
            Assert.Equal(cheep.AuthorName, author.Name);
        }
    }

    [Fact]
    public async Task Test_FollowAndUnfollowAuthor()
    {
        // Initialize the database
        await using var context = await Util.CreateInMemoryDatabase(1);

        // Create the service
        var author1 = await _cheepCommandRepository.NewAuthorAsync("author1", "author1@example.com");
        var author2 = await _cheepCommandRepository.NewAuthorAsync("author2", "author2@example.com");

        // Follow author2
        await _cheepCommandRepository.FollowAuthorAsync(author1.Id, author2.Id);
        var isFollowing = await _cheepCommandRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.True(isFollowing);

        // Unfollow author2
        await _cheepCommandRepository.UnfollowAuthorAsync(author1.Id, author2.Id);
        isFollowing = await _cheepCommandRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.False(isFollowing);
    }
}