using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author);
    public Task<List<CheepDTO>> GetCheepsFromUserTimeline(int page, string author);
    public Task PostCheep(Cheep cheep);
    public Task FollowAuthor(string currentAuthorName, string targetAuthorName);
    public Task UnfollowAuthor(string currentAuthorName, string targetAuthorName);
    public Task<bool> IsFollowing(string currentAuthorId, string targetAuthorId);
    public Task<List<string>> GetFollowing(string authorId);
    public Task DeleteCheepsByAuthor(string authorId);
    public Task DeleteFollowersAndFollowing(string authorId);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        return await _cheepRepository.GetCheepsAsync(page);
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author)
    {
        return await _cheepRepository.GetCheepsFromAuthorAsync(page, author);
    }

    public async Task<List<CheepDTO>> GetCheepsFromUserTimeline(int page, string author)
    {
        return await _cheepRepository.GetCheepsFromUserTimelineAsync(page, author);
    }

    public async Task PostCheep(Cheep cheep)
    {
        await _cheepRepository.PostCheepAsync(cheep);
    }

    public async Task FollowAuthor(string currentAuthorId, string targetAuthorId)
    {
        await _authorRepository.FollowAuthorAsync(currentAuthorId, targetAuthorId);
    }

    public async Task UnfollowAuthor(string currentAuthorId, string targetAuthorId)
    {
        await _authorRepository.UnfollowAuthorAsync(currentAuthorId, targetAuthorId);
    }

    public async Task<bool> IsFollowing(string currentAuthorId, string targetAuthorId)
    {
        return await _authorRepository.IsFollowingAsync(currentAuthorId, targetAuthorId);
    }
    
    public async Task<List<string>> GetFollowing(string authorId){
        return await _authorRepository.GetFollowingAsync(authorId);
    }

    public async Task DeleteCheepsByAuthor(string authorId){
        await _authorRepository.DeleteCheepsByAuthorAsync(authorId);
    }
    public async Task DeleteFollowersAndFollowing(string authorId){
        await _authorRepository.DeleteFollowersAndFollowingAsync(authorId);
    }
}