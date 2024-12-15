namespace IntegrationTest;
using System;
using System.Threading.Tasks;
using Xunit;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Data;
using Util;

public class IntegrationTest : IAsyncLifetime
{

    public ChirpDBContext context { get; private set; }
    public static DateTime CurrentTime = DateTime.Now;

    public async Task InitializeAsync()
    {
        context = await Util.CreateInMemoryDatabase();
           Author author1 = new Author
        {
            Name = "TestUser1",
            Email = "test1@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
         Author author2 = new Author
        {
            Name = "TestUser2",
            Email = "test2@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
        context.Authors.AddRange(author1, author2);
        
          context.Cheeps.AddRange(
                new Cheep { 
                    Text = "Hello World!", 
                    AuthorId = author1.Id, 
                    TimeStamp = CurrentTime,
                    Author = author1
                },
                new Cheep { 
                    Text = "Another Cheep, hell yeah", 
                    AuthorId = author2.Id, 
                    TimeStamp = CurrentTime,
                    Author = author2
                }
            );

        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (context != null)
        {
            await context.Database.EnsureDeletedAsync();
            await context.DisposeAsync();
        }
    }



    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepService(string author, string message)
    {

        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        // Create the service
        ICheepService cheepService = new CheepService(cheepRepository, authorRepository);

        var (cheeps, _) = await cheepService.GetCheeps(1);
        var (cheepsFromAuthor, _) = await cheepService.GetCheepsFromAuthor(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "Hello World!")]
    public async Task Test_GetCheepsUsingCheepRepository(string author, string message)
    {

        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        var (cheeps, _) = await cheepRepository.GetCheepsAsync(1);
        var (cheepsFromAuthor, _) = await cheepRepository.GetCheepsFromAuthorAsync(1, author);

        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);
        // Assert that the data is correct
        Assert.Equal(message, cheepsFromAuthor.First().Message);
    }

    [Theory]
    [InlineData("TestUser1", "test1@example.dk")]
    public async Task Test_GetAuthor(string authorName, string email)
    {

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

        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        var (cheeps, _) = await cheepRepository.GetCheepsAsync(1);
        // Assert we have two and only two cheeps
        Assert.Equal(2, cheeps.Count);

        // Create a new cheep
        await cheepRepository.NewCheepAsync(authorName, email, "New Cheep!");
        (cheeps, _) = await cheepRepository.GetCheepsAsync(1);

        // Assert we have three and only three cheeps
        Assert.Equal(3, cheeps.Count);
    }

    [Theory]
    [InlineData("TestUser1", "TestUser2")]
    public async Task Test_FollowAndUnfollowAuthor(string followerName, string targetName)
    {
        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };
   
        context.Achievements.AddRange(ach1, ach2, ach3, ach4);
        await context.SaveChangesAsync();

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
        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };
   
        context.Achievements.AddRange(ach1, ach2, ach3, ach4);
        await context.SaveChangesAsync();

        // Create the repositories
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);
        var cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        // Get initial timeline
        var (timeline, _) = await cheepRepository.GetCheepsFromUserTimelineAsync(1, authorName);
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
        var (updatedTimeline, _) = await cheepRepository.GetCheepsFromUserTimelineAsync(1, authorName);

        // Timeline should now include the new cheep
        Assert.Equal(initialCount + 1, updatedTimeline.Count);
        Assert.Contains(updatedTimeline, c => c.Message == "New cheep from followed user");
    }
}