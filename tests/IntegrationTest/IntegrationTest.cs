namespace IntegrationTest;
using System;
using System.Threading.Tasks;
using Xunit;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Data;
using Util;

public class IntegrationTest
{

    public static DateTime CurrentTime = DateTime.Now;
    
    [Fact]
    public async Task Test_FollowAndUnfollowAuthor()
    {
        //araange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();

        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };

        context.Achievements.AddRange(ach1, ach2, ach3, ach4);

          Author followed = new Author
        {
            Name = "TestUser1",
            Email = "test1@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
        Author following = new Author
        {
            Name = "TestUser2",
            Email = "test2@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
        context.Authors.AddRange(followed, following);
       
        await context.SaveChangesAsync();

        //act
        var achievementRepository = new AchievementRepository(context);
        var authorRepository = new AuthorRepository(context, achievementRepository);

        // Get authors
        var follower = await authorRepository.GetAuthorByNameAsync(followed.Name);
        var target = await authorRepository.GetAuthorByNameAsync(following.Name);

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

    [Fact]
    public async Task Test_GetCheepsFromUserTimeline()
    {
        // arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();

        Author author = new Author
        {
            Name = "TestUser1",
            Email = "test1@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
      
        context.Authors.Add(author);
       
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
        var (timeline, _) = await cheepRepository.GetCheepsFromUserTimelineAsync(1, author.Id);
        var initialCount = timeline.Count;

        // Create a new author and have the test user follow them
        var newAuthor = await authorRepository.NewAuthorAsync("NewUser", "new@example.com");
        var follower = await authorRepository.GetAuthorByNameAsync(author.Name);
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
        var (updatedTimeline, _) = await cheepRepository.GetCheepsFromUserTimelineAsync(1, author.Id);

        // Timeline should now include the new cheep
        Assert.Equal(initialCount + 1, updatedTimeline.Count);
        Assert.Contains(updatedTimeline, c => c.Message == "New cheep from followed user");
    }
}