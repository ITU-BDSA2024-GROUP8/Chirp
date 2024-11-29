using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Chirp.Repositories;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string authorId);
    public Task<List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string authorId);
    public Task NewCheepAsync(string authorName, string authorEmail, string text); //Denne metode skal slettes og tests skal tilrettes
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

    public async Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string authorId)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.Id
            where a.Id == authorId
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
    
    public async Task<List<CheepDTO>> GetCheepsFromUserTimelineAsync(int page, string authorId)
    {
        var query = (
            from cheep in _dbContext.Cheeps
            join a in _dbContext.Authors
                   on cheep.AuthorId equals a.Id
            where a.Id == authorId || _dbContext.AuthorFollowers.Any(f =>  f.FollowingId == cheep.AuthorId && f.FollowerId == authorId)
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                AuthorId = cheep.AuthorId,
                AuthorName = a.Name,
                Message = cheep.Text,
                Timestamp = cheep.TimeStamp
            }).Skip((page*32)-32).Take(32);

        return await query.ToListAsync();
    }
    
    //Denne metode skal slettes og tests skal tilrettes
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