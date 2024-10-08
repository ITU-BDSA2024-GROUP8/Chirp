using Chirp.Razor.Data;
using Chirp.Razor.DTOs;
using Chirp.Razor.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<int> CreateMessage(MessageDTO message);
    public Task<List<CheepDTO>> GetCheepsByAuthorAsync(int authorId);
    public Task<List<CheepDTO>> GetLatestCheepsAsync(int count = 10);
    public Task DeleteCheepAsync(int cheepId);
    public Task<List<AuthorDTO>> GetTopAuthorsAsync(int count = 5);
    public Task<string> GetAuthorEmailAsync(int authorId);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    //Gets whole cheeps, author name, and timestamp from database
    public async Task<List<CheepDTO>> GetCheepsAsync(int page)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.AuthorId
            select new CheepDTO
            {
                Author = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).Skip((page*32)-32).Take(32);
        
        return await query.ToListAsync();
    }
    
    // gets all messages from a specific author
    public async Task<List<CheepDTO>> GetCheepsByAuthorAsync(int authorId)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors
                on c.AuthorId equals a.AuthorId
            where c.AuthorId == authorId
            select new CheepDTO
            {
                Author = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            });

        return await query.ToListAsync();
    }

// gets the latest cheeps
    public async Task<List<CheepDTO>> GetLatestCheepsAsync(int count = 10)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors
                on c.AuthorId equals a.AuthorId
            orderby c.TimeStamp descending
            select new CheepDTO
            {
                Author = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).Take(count);

        return await query.ToListAsync();
    }

// deletes a cheep by cheep id
    public async Task DeleteCheepAsync(int cheepId)
    {
        var cheep = await _dbContext.Cheeps.FindAsync(cheepId);
        if (cheep != null)
        {
            _dbContext.Cheeps.Remove(cheep);
            await _dbContext.SaveChangesAsync();
        }
    }

// gets the top authors with most cheeps
    public async Task<List<AuthorDTO>> GetTopAuthorsAsync(int count = 5)
    {
        var query = (
            from a in _dbContext.Authors
            orderby a.Cheeps.Count descending
            select new AuthorDTO
            {
                Name = a.Name,
                Email = a.Email,
                CheepCount = a.Cheeps.Count
            }).Take(count);

        return await query.ToListAsync();
    }

// gets the email of an author by their ID
    public async Task<string> GetAuthorEmailAsync(int authorId)
    {
        var query = (
            from a in _dbContext.Authors
            where a.AuthorId == authorId
            select a.Email).FirstOrDefaultAsync();

        return await query;
    }
    
}