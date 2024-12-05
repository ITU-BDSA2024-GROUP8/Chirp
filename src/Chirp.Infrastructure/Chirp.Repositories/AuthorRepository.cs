using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface IAuthorRepository
{
    public Task<Author?> GetAuthorByNameAsync(string name);
    public Task<Author?> GetAuthorByEmailAsync(string email);
    public Task<Author> NewAuthorAsync(string authorName, string authorEmail);
    public Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId);
    public Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId);
    public Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId);
    public Task<List<string>> GetFollowingAsync(string authorId);
    public Task<string?> UpdateBioAsync(Author author, string? newBio);
    public Task DeleteCheepsByAuthorAsync(string authorId);
    public Task DeleteFollowersAndFollowingAsync(string authorId);
}
public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _dbContext;
    private readonly IAchievementRepository _achievementRepository;

    public AuthorRepository(ChirpDBContext dbContext, IAchievementRepository achievementRepository)
    {
        _dbContext = dbContext;
        _achievementRepository = achievementRepository;
    }

    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        var query = from a in _dbContext.Authors
                    where a.Name == name
                    select a;

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author?> GetAuthorByEmailAsync(string email)
    {
        var query = from a in _dbContext.Authors
                    where a.Email == email
                    select a;

        return await query.FirstOrDefaultAsync();
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

    public async Task FollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        var hasAchievementFollow = await _dbContext.AuthorAchievements.AnyAsync(a => a.AuthorId == currentAuthorId && a.AchievementId == 3);
        var hasAchievementFollowed = await _dbContext.AuthorAchievements.AnyAsync(a => a.AuthorId == targetAuthorId && a.AchievementId == 4);
        
        var followRelation = new AuthorFollower
        {
            FollowerId = currentAuthorId,
            FollowingId = targetAuthorId
        };

        _dbContext.AuthorFollowers.Add(followRelation);
        await _dbContext.SaveChangesAsync();

        if (!hasAchievementFollow) await _achievementRepository.AddNewAuthorAchievementAsync(currentAuthorId, 3);
        if (!hasAchievementFollowed) await _achievementRepository.AddNewAuthorAchievementAsync(targetAuthorId, 4);
    }

    public async Task UnfollowAuthorAsync(string currentAuthorId, string targetAuthorId)
    {
        var followRelation = await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
            a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);

        _dbContext.AuthorFollowers.Remove(followRelation!);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsFollowingAsync(string currentAuthorId, string targetAuthorId)
    {
        var followRelation = await _dbContext.AuthorFollowers.SingleOrDefaultAsync(a =>
            a.Follower.Id == currentAuthorId && a.Following.Id == targetAuthorId);

        return followRelation != null;
    }

    public async Task<List<string>> GetFollowingAsync(string authorId)
    {
        var query = (
            from a in _dbContext.AuthorFollowers
            where a.FollowerId == authorId
            select a.Following.Name);
        
        return await query.ToListAsync();
    }

    public async Task<string?> UpdateBioAsync(Author author, string? newBio)
    {
        author.Bio = newBio;
        
        _dbContext.Authors.Update(author);
        
        await _dbContext.SaveChangesAsync();

        return newBio;
    }
    
    public async Task DeleteCheepsByAuthorAsync(string authorId)
    {
        var cheeps = _dbContext.Cheeps.Where(c => c.AuthorId == authorId);
        _dbContext.Cheeps.RemoveRange(cheeps);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFollowersAndFollowingAsync(string authorId)
    {
        var followers = _dbContext.AuthorFollowers.Where(af => af.FollowerId == authorId || af.FollowingId == authorId);
        _dbContext.AuthorFollowers.RemoveRange(followers);

        await _dbContext.SaveChangesAsync();
    }
}