﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Chirp.Repositories;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsAsync(int page);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthorAsync(int page, string author);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimelineAsync(int page, string author);
    public Task NewCheepAsync(string authorName, string authorEmail, string text);
    public Task PostCheepAsync(Cheep cheep);
}

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    private readonly IAchievementRepository _achievementRepository;

    public CheepRepository(ChirpDBContext dbContext, IAuthorRepository authorRepository, IAchievementRepository achievementRepository)
    {
        _dbContext = dbContext;
        _authorRepository = authorRepository;
        _achievementRepository = achievementRepository;
    }
    
    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsAsync(int page)
    {
        var query =
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
            };
        
        return (await query.Skip((page*32)-32).Take(32).ToListAsync(), await query.CountAsync());
    }

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthorAsync(int page, string author)
    {
        var query =
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
            };
        
        return (await query.Skip((page*32)-32).Take(32).ToListAsync(), await query.CountAsync());
    }
    
    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimelineAsync(int page, string author)
    {
        var query =
            from cheep in _dbContext.Cheeps
            join a in _dbContext.Authors
                   on cheep.AuthorId equals a.Id
            where a.Name == author || _dbContext.AuthorFollowers.Any(f =>  f.FollowingId == cheep.AuthorId && f.Follower.Name == author)
            orderby cheep.TimeStamp descending
            select new CheepDTO
            {
                AuthorId = cheep.AuthorId,
                AuthorName = a.Name,
                Message = cheep.Text,
                Timestamp = cheep.TimeStamp
            };

        return (await query.Skip((page*32)-32).Take(32).ToListAsync(), await query.CountAsync());
    }
    
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
    

    public async Task PostCheepAsync(Cheep cheep)
    {
        var hasAchievement = await _dbContext.AuthorAchievements.AnyAsync(a => a.AuthorId == cheep.AuthorId && a.AchievementId == 2);
        
        _dbContext.Cheeps.Add(cheep);
        cheep.Author.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();

        if (!hasAchievement) await _achievementRepository.AddNewAuthorAchievementAsync(cheep.AuthorId, 2);
    }
}