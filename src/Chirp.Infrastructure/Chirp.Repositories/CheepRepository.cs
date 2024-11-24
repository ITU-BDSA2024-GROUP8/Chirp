using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Chirp.Repositories;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author);
    public Task <List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string author);
    public Task NewCheepAsync(string authorName, string authorEmail, string text);
    public Task PostCheepAsync(Cheep cheep);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(ChirpDBContext dbContext, IAuthorRepository authorRepository)
    {
        _dbContext = dbContext;
        _authorRepository = authorRepository;
    }
    
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
    // public async Task<List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string author)
    // {
    //     var query = (
    //         from c in _dbContext.Cheeps
    //         join a in _dbContext.Authors on c.AuthorId equals a.Id
    //         join f in _dbContext.AuthorFollowers on c.AuthorId equals f.FollowingId into followers
    //         from f in followers.DefaultIfEmpty()
    //         where a.Name == author || f.Follower.Name == author
    //         orderby c.TimeStamp descending
    //         select new CheepDTO
    //         {
    //             AuthorId = c.AuthorId,
    //             AuthorName = a.Name,
    //             Message = c.Text,
    //             Timestamp = c.TimeStamp
    //         }).Skip((page * 32) - 32).Take(32);
    //
    //     return await query.ToListAsync();
    // }
    
    public async Task NewCheepAsync(string authorName, string authorEmail, string text)
    {
        var author = await _authorRepository.GetAuthorByNameAsync(authorName);

        if (author == null)
        {
            author = await _authorRepository.NewAuthorAsync(authorName, authorEmail);
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
    

    public async Task PostCheepAsync(Cheep cheep){
        _dbContext.Cheeps.Add(cheep);
        cheep.Author.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
}