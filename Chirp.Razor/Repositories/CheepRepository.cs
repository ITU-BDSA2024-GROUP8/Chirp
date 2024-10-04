using Chirp.Razor.Data;
using Chirp.Razor.DTOs;
using Chirp.Razor.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
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
    
    //creates a new message
    public async Task<int> CreateMessage(MessageDTO message)
    {
        Message newMessage = new() { Text = message.Text, ... };
        var queryResult = await _dbContext.Messages.AddAsync(newMessage); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity.CheepId;
    }
    //gets all messages from a specific author
    public async Task<List<CheepDTO>> GetCheepsByAuthorAsync(int authorId)
    {
        return await _dbContext.Cheeps
            .Where(c => c.AuthorId == authorId)
            .Select(c => new CheepDTO
            {
                Author = c.Author.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).ToListAsync();
    }
    //gets the latest cheeps
    public async Task<List<CheepDTO>> GetLatestCheepsAsync(int count = 10)
    {
        return await _dbContext.Cheeps
            .OrderByDescending(c => c.TimeStamp)
            .Take(count)
            .Select(c => new CheepDTO
            {
                Author = c.Author.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).ToListAsync();
    }
    //deletes a cheep by cheep id
    public async Task DeleteCheepAsync(int cheepId)
    {
        var cheep = await _dbContext.Cheeps.FindAsync(cheepId);
        if (cheep != null)
        {
            _dbContext.Cheeps.Remove(cheep);
            await _dbContext.SaveChangesAsync();
        }
    }
    //gets the top authors with most cheeps
    public async Task<List<AuthorDTO>> GetTopAuthorsAsync(int count = 5)
    {
        return await _dbContext.Authors
            .OrderByDescending(a => a.Cheeps.Count)
            .Take(count)
            .Select(a => new AuthorDTO
            {
                Name = a.Name,
                Email = a.Email,
                CheepCount = a.Cheeps.Count
            }).ToListAsync();
    }

    
}