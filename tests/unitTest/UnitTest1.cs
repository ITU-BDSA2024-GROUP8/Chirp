namespace unitTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Xunit;
using Util;
using System.Threading.Tasks;
using System;
using Chirp.Infrastructure.Models;
public class UnitTest1 : IAsyncLifetime
{
    public static DateTime CurrentTime = DateTime.Now;
    public ChirpDBContext context { get; private set; }
    private IAchievementRepository _achievementRepository;
    private IAuthorRepository _authorRepository;
    private ICheepRepository _cheepRepository;


    public async Task InitializeAsync()
    {
        // Use a new context with more data for this test
        
        _achievementRepository = new AchievementRepository(context);
        _authorRepository = new AuthorRepository(context, _achievementRepository);
        _cheepRepository = new CheepRepository(context, _authorRepository, _achievementRepository);
    }

    public async Task DisposeAsync()
    {
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }


    [Fact]
    public async Task Test_FindAuthorByName()
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
        context.Authors.AddRange(author1);
        await context.SaveChangesAsync();

        var authorByName = await _authorRepository.GetAuthorByNameAsync(author1.Name);
        Assert.Equal(author1, authorByName);
    }

    [Fact]
    public async Task Test_FindAuthorByEmail()
    {
        Author author1 = new Author
        {
            Name = "TestUser1",
            Email = "test1@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
        context.Authors.AddRange(author1);
        await context.SaveChangesAsync();

        var authorByEmail = await _authorRepository.GetAuthorByEmailAsync(author1.Email);
        Assert.Equal(author1, authorByEmail);
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
        var (cheepsFromAuthor, _) = await _cheepRepository.GetCheepsFromAuthorAsync(1, author.Name);
        Assert.Empty(cheepsFromAuthor);

        // Create a new cheep
        await _cheepRepository.NewCheepAsync(author.Name, author.Email!, "This is a new test cheep");

        // Verify the cheep was created
        var (newCheepsFromAuthor, _) = await _cheepRepository.GetCheepsFromAuthorAsync(1, author.Name);
        Assert.Single(newCheepsFromAuthor);
    }

    [Fact]
    public async Task Test_CheepsForACertainPage()
    {
        
        var a1 = new Author()
        {
            Name = "Roger Histand",
            Email = "Roger+Histand@hotmail.com",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };

        context.Authors.AddRange(a1);

        for (int i = 0; i < 40; i++)
        {
            var cheep = new Cheep
            {
                Text = "" + i,
                AuthorId = a1.Id,
                TimeStamp = CurrentTime,
                Author = a1
            };

            context.Cheeps.AddRange(cheep);
        }
        await context.SaveChangesAsync();


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
          var a1 = new Author()
        {
            Name = "Roger Histand",
            Email = "Roger+Histand@hotmail.com",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };

        context.Authors.AddRange(a1);

        for (int i = 0; i < 40; i++)
        {
            var cheep = new Cheep
            {
                Text = "" + i,
                AuthorId = a1.Id,
                TimeStamp = CurrentTime,
                Author = a1
            };

            context.Cheeps.AddRange(cheep);
        }
        await context.SaveChangesAsync();

        var achievementRepo = new AchievementRepository(context);
        var authorRepo = new AuthorRepository(context, achievementRepo);
        var cheepRepo = new CheepRepository(context, authorRepo, achievementRepo);

        var author = await authorRepo.GetAuthorByNameAsync("Roger Histand");
        Assert.NotNull(author);

        var (cheepsOnPage, _) = await cheepRepo.GetCheepsFromAuthorAsync(1, author.Name);

        foreach (var cheep in cheepsOnPage)
        {
            Assert.Equal(cheep.AuthorName, author.Name);
        }

    }

    [Fact]
    public async Task Test_FollowAndUnfollowAuthor()
    {
      
        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };
    
        context.Achievements.AddRange(ach1, ach2, ach3, ach4);
        await context.SaveChangesAsync();
        // Create two authors

        var follower = await _authorRepository.NewAuthorAsync("follower", "follower@test.com");
        var following = await _authorRepository.NewAuthorAsync("following", "following@test.com");

        // Test follow
        await _authorRepository.FollowAuthorAsync(follower.Id, following.Id);
        var isFollowing = await _authorRepository.IsFollowingAsync(follower.Id, following.Id);
        Assert.True(isFollowing);

        // Test unfollow
        await _authorRepository.UnfollowAuthorAsync(follower.Id, following.Id);
        isFollowing = await _authorRepository.IsFollowingAsync(follower.Id, following.Id);
        Assert.False(isFollowing);
    }

    [Fact]
    public async Task Test_UserTimeline()
    {

        Achievement ach1 = new Achievement() { Title = "Rookie Chirper", Description = "Welcome aboard! You signed up successfully to Chirp", ImagePath = "/images/Badges/Signup-badge.png" };
        Achievement ach2 = new Achievement() { Title = "Novice Cheepster", Description = "Congratulations! You created your first Cheep.", ImagePath = "/images/Badges/First-cheep-badge.png" };
        Achievement ach3 = new Achievement() { Title = "Branching Out", Description = "You followed your first Chirper. Every great tree starts with a single branch.", ImagePath = "/images/Badges/First-following-badge.png" };
        Achievement ach4 = new Achievement() { Title = "Social Magnet", Description = "Someone followed you. You must be cheeping some good stuff.", ImagePath = "/images/Badges/First-follower-badge.png" };
   
        context.Achievements.AddRange(ach1, ach2, ach3, ach4);
        await context.SaveChangesAsync();

        // Create authors
        var mainAuthor = await _authorRepository.NewAuthorAsync("main", "main@test.com");
        var followedAuthor = await _authorRepository.NewAuthorAsync("followed", "followed@test.com");

        // Create some cheeps
        await _cheepRepository.NewCheepAsync(mainAuthor.Name, mainAuthor.Email!, "Main author cheep");
        await _cheepRepository.NewCheepAsync(followedAuthor.Name, followedAuthor.Email!, "Followed author cheep");

        // Follow the author
        await _authorRepository.FollowAuthorAsync(mainAuthor.Id, followedAuthor.Id);

        // Get timeline
        var (timeline, _) = await _cheepRepository.GetCheepsFromUserTimelineAsync(1, mainAuthor.Name);

        // Should see both cheeps
        Assert.Equal(2, timeline.Count);
        Assert.Contains(timeline, c => c.AuthorName == mainAuthor.Name);
        Assert.Contains(timeline, c => c.AuthorName == followedAuthor.Name);
    }
}