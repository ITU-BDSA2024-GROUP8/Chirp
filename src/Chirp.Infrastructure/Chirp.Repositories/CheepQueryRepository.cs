using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepQueryRepository {
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author);
    public Task <List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string author);
    public Task<Author?> GetAuthorByNameAsync(string name);
    public Task<Author?> GetAuthorByEmailAsync(string email);
}

public class CheepQueryRepository : ICheepQueryRepository{
    private readonly ChirpDBContext _dbContext;

    public CheepQueryRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
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


}