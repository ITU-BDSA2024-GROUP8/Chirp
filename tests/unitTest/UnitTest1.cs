namespace unitTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Util;

public class UnitTest1
{
    private readonly IAchievementRepository _achievementRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly ChirpDBContext _context;

    public UnitTest1()
    {
        _context = Util.CreateInMemoryDatabase(1).Result;
        _achievementRepository = new AchievementRepository(_context);
        _authorRepository = new AuthorRepository(_context, _achievementRepository);
        _cheepRepository = new CheepRepository(_context, _authorRepository, _achievementRepository);
    }

    [Theory]
    [InlineData("TestUser1")]
    public async Task Test_FindAuthorByName(string authorName)
    {
        var authorByName = await _authorRepository.GetAuthorByNameAsync(authorName);
        Assert.Equal(authorName, authorByName?.Name);
    }

    [Theory]
    [InlineData("test1@example.dk")]
    public async Task Test_FindAuthorByEmail(string email)
    {
        var authorByEmail = await _authorRepository.GetAuthorByEmailAsync(email);
        Assert.Equal(email, authorByEmail?.Email);
    }

    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        var author = await _authorRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");
        var authorByName = await _authorRepository.GetAuthorByNameAsync(author.Name);
        Assert.Equal(author, authorByName);
    }

    [Fact]
    public async Task Test_CreateNewCheep()
    {
        // Create a new author first
        var author = await _authorRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");

        // Check that author has no cheeps initially
        var (cheepsFromAuthor, _) = await _cheepRepository.GetCheepsFromAuthorAsync(1, author.Id);
        Assert.Empty(cheepsFromAuthor);

        // Create a new cheep
        await _cheepRepository.NewCheepAsync(author.Name, author.Email!, "This is a new test cheep");

        // Verify the cheep was created
        var (newCheepsFromAuthor, _) = await _cheepRepository.GetCheepsFromAuthorAsync(1, author.Id);
        Assert.Single(newCheepsFromAuthor);
    }

    [Fact]
    public async Task Test_CheepsForACertainPage()
    {
        // Use a new context with more data for this test
        await using var context = await Util.CreateInMemoryDatabase(2);
        var achievementRepo = new AchievementRepository(context);
        var authorRepo = new AuthorRepository(context, achievementRepo);
        var cheepRepo = new CheepRepository(context, authorRepo, achievementRepo);

        var (cheepsOnPage1, _) = await cheepRepo.GetCheepsAsync(1);
        var (cheepsOnPage2, _) = await cheepRepo.GetCheepsAsync(2);

        Assert.Equal(32, cheepsOnPage1.Count);
        Assert.Equal(8, cheepsOnPage2.Count);
    }

    [Fact]
    public async Task Test_CheepsForACertainPageByAuthor()
    {
        // Use a new context with more data for this test
        await using var context = await Util.CreateInMemoryDatabase(2);
        var achievementRepo = new AchievementRepository(context);
        var authorRepo = new AuthorRepository(context, achievementRepo);
        var cheepRepo = new CheepRepository(context, authorRepo, achievementRepo);

        var author = await authorRepo.GetAuthorByNameAsync("Roger Histand");
        Assert.NotNull(author);

        var (cheepsOnPage, _) = await cheepRepo.GetCheepsFromAuthorAsync(1, author.Id);

        foreach (var cheep in cheepsOnPage)
        {
            Assert.Equal(cheep.AuthorName, author.Name);
        }
    }

    [Fact]
    public async Task Test_FollowAndUnfollowAuthor()
    {
        // Create two authors
        var author1 = await _authorRepository.NewAuthorAsync("follower", "follower@test.com");
        var author2 = await _authorRepository.NewAuthorAsync("following", "following@test.com");

        // Test follow
        await _authorRepository.FollowAuthorAsync(author1.Id, author2.Id);
        var isFollowing = await _authorRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.True(isFollowing);

        // Test unfollow
        await _authorRepository.UnfollowAuthorAsync(author1.Id, author2.Id);
        isFollowing = await _authorRepository.IsFollowingAsync(author1.Id, author2.Id);
        Assert.False(isFollowing);
    }

    [Fact]
    public async Task Test_UserTimeline()
    {
        // Create authors
        var mainAuthor = await _authorRepository.NewAuthorAsync("main", "main@test.com");
        var followedAuthor = await _authorRepository.NewAuthorAsync("followed", "followed@test.com");

        // Create some cheeps
        await _cheepRepository.NewCheepAsync(mainAuthor.Name, mainAuthor.Email!, "Main author cheep");
        await _cheepRepository.NewCheepAsync(followedAuthor.Name, followedAuthor.Email!, "Followed author cheep");

        // Follow the author
        await _authorRepository.FollowAuthorAsync(mainAuthor.Id, followedAuthor.Id);

        // Get timeline
        var (timeline, _) = await _cheepRepository.GetCheepsFromUserTimelineAsync(1, mainAuthor.Id);

        // Should see both cheeps
        Assert.Equal(2, timeline.Count);
        Assert.Contains(timeline, c => c.AuthorName == mainAuthor.Name);
        Assert.Contains(timeline, c => c.AuthorName == followedAuthor.Name);
    }
}