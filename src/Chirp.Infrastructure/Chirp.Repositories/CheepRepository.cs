using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    //Query methods
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author);
    public Task <List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string author);
    public Task<Author?> GetAuthorByNameAsync(string name);
    public Task<Author?> GetAuthorByEmailAsync(string email);

    //Command methods
    public Task NewCheepAsync(string authorName, string authorEmail, string text);
    public Task<Author> NewAuthorAsync(string authorName, string authorEmail);
    public Task PostCheepAsync(Cheep cheep);
    public Task FollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task UnfollowAuthorAsync(string currentAuthorName, string targetAuthorName);
    public Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId);
    public Task<List<string>> GetFollowingAsync(string authorId);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    //Query methods that return a value
    public async Task<List<CheepDTO>> GetCheepsAsync(int page)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.Id
            orderby c.TimeStamp descending
            select new CheepDTO
            {
                AuthorId = c.AuthorId,
                AuthorName = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp
            }).Skip((page*32)-32).Take(32);
        
        return await query.ToListAsync();
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.Id
            where a.Name == author
            orderby c.TimeStamp descending
            select new CheepDTO
            {
                AuthorId = c.AuthorId,
                AuthorName = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp
            }).Skip((page*32)-32).Take(32);
        
        return await query.ToListAsync();
    }

    public async Task <List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string author)
    {
        var authorCheeps = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.Id
            where a.Name == author
 
            select new CheepDTO
            {
                AuthorId = c.AuthorId,
                AuthorName = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp
            });

        var followingCheeps = (
            from f in _dbContext.AuthorFollowers
            join c in _dbContext.Cheeps
                on f.FollowingId equals c.AuthorId
            where f.Follower.Name == author

            select new CheepDTO
            {
                AuthorId = c.AuthorId,
                AuthorName = c.Author.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp
            });

        var query = authorCheeps.Union(followingCheeps).OrderByDescending(c => c.Timestamp).Skip((page * 32) - 32).Take(32);
        
        return await query.ToListAsync();
    }

    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        var query = (
            from a in _dbContext.Authors
            where a.Name == name
            select a);
        
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author?> GetAuthorByEmailAsync(string email)
    {
        var query = (
            from a in _dbContext.Authors
            where a.Email == email
            select a);
        
        return await query.FirstOrDefaultAsync();
    }


    //Command methods that create, delete and modify data in the DB.
    public async Task NewCheepAsync(string authorName, string authorEmail, string text)
    {
        var author = await GetAuthorByNameAsync(authorName);

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

    public async Task<List<string>> GetFollowingAsync(string authorId){
        var query = (
            from a in _dbContext.AuthorFollowers
            where a.FollowerId == authorId
            select a.Following.Name);
        
        return await query.ToListAsync();
    }
}