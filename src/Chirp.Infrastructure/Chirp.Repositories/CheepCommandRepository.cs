using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepCommandRepository {
    public Task NewCheepAsync(string authorName, string authorEmail, string text);
    public Task<Author> NewAuthorAsync(string authorName, string authorEmail);
    public Task PostCheepAsync(Cheep cheep);
    public Task FollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task UnfollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId);
}

public class CheepCommandRepository : ICheepCommandRepository{
     private readonly ChirpDBContext _dbContext;
     private readonly ICheepQueryRepository _cheepQueryRepository;

    public CheepCommandRepository(ChirpDBContext dbContext, ICheepQueryRepository cheepQueryRepository)
    {
        _dbContext = dbContext;
        _cheepQueryRepository = cheepQueryRepository ?? throw new ArgumentNullException(nameof(cheepQueryRepository));
    }
     public async Task NewCheepAsync(string authorName, string authorEmail, string text)
    {
        var author = await _cheepQueryRepository.GetAuthorByNameAsync(authorName);

        if (author == null)
        {
            author = await NewAuthorAsync(authorName, authorEmail);
        }

        var newCheep = new Cheep
        {
            AuthorId = author.Id,
            Text = text,
            TimeStamp = DateTime.Now
        };

        _dbContext.Cheeps.Add(newCheep);

        author.Cheeps.Add(newCheep);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Author> NewAuthorAsync(string authorName, string authorEmail)
    {
        var newAuthor = new Author
        {
            Name = authorName,
            Email = authorEmail,
            Cheeps = new List<Cheep>(),
            Followers = new List<AuthorFollower>(),
            Following = new List<AuthorFollower>()
        };

        _dbContext.Authors.Add(newAuthor);

        await _dbContext.SaveChangesAsync();

        return newAuthor;
    }

    public async Task PostCheepAsync(Cheep cheep){
        _dbContext.Cheeps.Add(cheep);
        cheep.Author.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        var followRelation = new AuthorFollower
        {
            FollowerId = currentAuthorId,
            FollowingId = targetAuthorId
        };

        _dbContext.AuthorFollowers.Add(followRelation);
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        var followRelation =
            await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
                a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);

        _dbContext.AuthorFollowers.Remove(followRelation!);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId) {
        var followRelation =
            await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
                a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);
        
        return followRelation != null;
    }
}