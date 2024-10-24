﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    //Query methods
    public Task<List<CheepDTO>> GetCheepsAsync(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author);
    public Task<Author?> GetAuthorByNameAsync(string name);
    public Task<Author?> GetAuthorByEmailAsync(string email);

    //Command methods
    public Task NewCheepAsync(string authorName, string authorEmail, string text);
    public Task<Author> NewAuthorAsync(string authorName, string authorEmail);
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
                on c.AuthorId equals a.AuthorId
            select new CheepDTO
            {
                Author = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).Skip((page*32)-32).Take(32);
        
        return await query.ToListAsync();
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthorAsync(int page, string author)
    {
        var query = (
            from c in _dbContext.Cheeps
            join a in _dbContext.Authors 
                on c.AuthorId equals a.AuthorId
            where a.Name == author
            select new CheepDTO
            {
                Author = a.Name,
                Message = c.Text,
                Timestamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            }).Skip((page*32)-32).Take(32);
        
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
            AuthorId = author.AuthorId,
            Author = author,
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
            Cheeps = new List<Cheep>()
        };

        _dbContext.Authors.Add(newAuthor);

        await _dbContext.SaveChangesAsync();

        return newAuthor;
    }
}