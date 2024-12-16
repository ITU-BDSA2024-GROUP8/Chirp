namespace unitTest;

using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Data;
using Xunit;
using Util;
using System.Threading.Tasks;
using System;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public class UnitTest1
{
    public static DateTime CurrentTime = DateTime.Now;


    [Fact]
    public async Task Test_FindAuthorByName()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);

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

        //act
        var authorByName = await authorRepository.GetAuthorByNameAsync(author1.Name);

        //assert
        Assert.Equal(author1, authorByName);

        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_FindAuthorByEmail()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);


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

        //act
        var authorByEmail = await authorRepository.GetAuthorByEmailAsync(author1.Email);

        //assert
        Assert.Equal(author1, authorByEmail);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_CreateNewAuthor()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);

        //act
        var author = await authorRepository.NewAuthorAsync("testAuthor", "testAuthor@email.com");

        //assert
        Assert.Equal("testAuthor@email.com", author.Email);
        Assert.Equal("testAuthor", author.Name);
        
        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_CreateNewCheep()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);
        ICheepRepository cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

        Author author = new Author
        {
            Name = "TestUser1",
            Email = "test1@example.dk",
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };
        context.Authors.AddRange(author);
        await context.SaveChangesAsync();

        //act
        await cheepRepository.NewCheepAsync(author.Name, author.Email!, "This is a new test cheep");

        //assert
        Assert.NotEmpty(author.Cheeps);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_CheepsForACertainPage()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        var achievementRepo = new AchievementRepository(context);
        var authorRepo = new AuthorRepository(context, achievementRepo);
        var cheepRepo = new CheepRepository(context, authorRepo, achievementRepo);

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


        //act
        var (cheepsOnPage1, _) = await cheepRepo.GetCheepsAsync(1);
        var (cheepsOnPage2, _) = await cheepRepo.GetCheepsAsync(2);

        //assert
        Assert.Equal(32, cheepsOnPage1.Count);
        Assert.Equal(8, cheepsOnPage2.Count);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_CheepsForACertainPageByAuthor()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        var achievementRepo = new AchievementRepository(context);
        var authorRepo = new AuthorRepository(context, achievementRepo);
        var cheepRepo = new CheepRepository(context, authorRepo, achievementRepo);

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

        //act
        var (cheepsOnPage, _) = await cheepRepo.GetCheepsFromAuthorAsync(1, a1.Name);

        //assert
        foreach (var cheep in cheepsOnPage)
        {
            Assert.Equal(cheep.AuthorName, a1.Name);
        }

        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_FollowAuthor()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);


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
        await authorRepository.FollowAuthorAsync(followed.Id, following.Id);
        var isFollowing = await context.AuthorFollowers
         .AnyAsync(af => af.FollowerId == followed.Id && af.FollowingId == following.Id);

        //assert
        Assert.True(isFollowing);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_UnfollowAuthor()
    {
        //arrange
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);


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
     
        context.AuthorFollowers.Add(new AuthorFollower { FollowerId = followed.Id, FollowingId = following.Id });

        await context.SaveChangesAsync();

        //act
        await authorRepository.UnfollowAuthorAsync(followed.Id, following.Id);
        var isFollowing = await context.AuthorFollowers
               .AnyAsync(af => af.FollowerId == followed.Id && af.FollowingId == following.Id);


        //assert
        Assert.False(isFollowing);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Test_UserTimeline()
    {
        ChirpDBContext context = await Util.CreateInMemoryDatabase();
        IAchievementRepository achievementRepository = new AchievementRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context, achievementRepository);
        ICheepRepository cheepRepository = new CheepRepository(context, authorRepository, achievementRepository);

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
        context.Cheeps.AddRange(
            new Cheep
            {
                Text = "Hello World!",
                AuthorId = followed.Id,
                TimeStamp = CurrentTime,
                Author = followed
            },
            new Cheep
            {
                Text = "Another Cheep, hell yeah",
                AuthorId = following.Id,
                TimeStamp = CurrentTime,
                Author = following
            }
        );

        context.AuthorFollowers.Add(new AuthorFollower { FollowerId = followed.Id, FollowingId = following.Id });

        await context.SaveChangesAsync();



        //act
        var (timeline, _) = await cheepRepository.GetCheepsFromUserTimelineAsync(1, followed.Name);


        //assert
        Assert.Equal(2, timeline.Count);
        Assert.Contains(timeline, c => c.AuthorName == following.Name);
        Assert.Contains(timeline, c => c.AuthorName == followed.Name);


        //cleanup
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }
}