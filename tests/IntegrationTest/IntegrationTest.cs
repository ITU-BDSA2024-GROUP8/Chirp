namespace IntegrationTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;
using Util;

public class IntegrationTest
{
    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);
        
        // Create the service
        ICheepService cheepService = new CheepService(cheepRepository, authorRepository);
        
        var cheeps = await cheepService.GetCheeps(1);
        var cheepsFromAuthor = await cheepService.GetCheepsFromAuthor(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepRepository(string author, string message)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        var cheeps = await cheepRepository.GetCheepsAsync(1);
        var cheepsFromAuthor = await cheepRepository.GetCheepsFromAuthorAsync(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "test1@example.dk")]
    public async Task Test_GetAuthor(string authorName, string email)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repository
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);

        var authorByName = await authorRepository.GetAuthorByNameAsync(authorName);
        var authorByEmail = await authorRepository.GetAuthorByEmailAsync(email);
        
        // Assert that the data is correct
        Assert.Equal(authorName, authorByName?.Name);
        Assert.Equal(email, authorByEmail?.Email);
    }

    [Theory]
    [InlineData("TestUser1", "test1@example.dk")]
    public async Task Test_CreateCheep(string authorName, string email)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        var cheeps = await cheepRepository.GetCheepsAsync(1);
        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);

        // Create a new cheep
        await cheepRepository.NewCheepAsync(authorName, email, "New Cheep!");
        cheeps = await cheepRepository.GetCheepsAsync(1);

        // Assert we have three and only three cheeps
        Assert.Equal(3, cheeps.Count);
    }

    [Theory]
    [InlineData("TestUser1", "TestUser2")]
    public async Task Test_FollowAndUnfollowAuthor(string followerName, string targetName)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        
        // Get authors
        var follower = await authorRepository.GetAuthorByNameAsync(followerName);
        var target = await authorRepository.GetAuthorByNameAsync(targetName);
        
        Assert.NotNull(follower);
        Assert.NotNull(target);

        // Test following
        await authorRepository.FollowAuthorAsync(follower.Id, target.Id);
        var isFollowing = await authorRepository.IsFollowingAsync(follower.Id, target.Id);
        Assert.True(isFollowing);

        // Test unfollowing
        await authorRepository.UnfollowAuthorAsync(follower.Id, target.Id);
        isFollowing = await authorRepository.IsFollowingAsync(follower.Id, target.Id);
        Assert.False(isFollowing);
    }

    [Theory]
    [InlineData("TestUser1")]
    public async Task Test_GetCheepsFromUserTimeline(string authorName)
    {
        // Initialize the database
        using var context = await Util.CreateInMemoryDatabase(1);
        
        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);
        
        // Get initial timeline
        var timeline = await cheepRepository.GetCheepsFromUserTimelineAsync(1, authorName);
        var initialCount = timeline.Count;

        // Create a new author and have the test user follow them
        var newAuthor = await authorRepository.NewAuthorAsync("NewUser", "new@example.com");
        var follower = await authorRepository.GetAuthorByNameAsync(authorName);
        Assert.NotNull(follower);
        
        await authorRepository.FollowAuthorAsync(follower.Id, newAuthor.Id);

        // Create a new cheep from the followed author
        var newCheep = new Cheep
        {
            AuthorId = newAuthor.Id,
            Text = "New cheep from followed user",
            TimeStamp = DateTime.Now
        };
        await cheepRepository.PostCheepAsync(newCheep);

        // Get updated timeline
        var updatedTimeline = await cheepRepository.GetCheepsFromUserTimelineAsync(1, authorName);
        
        // Timeline should now include the new cheep
        Assert.Equal(initialCount + 1, updatedTimeline.Count);
        Assert.Contains(updatedTimeline, c => c.Message == "New cheep from followed user");
    }
}