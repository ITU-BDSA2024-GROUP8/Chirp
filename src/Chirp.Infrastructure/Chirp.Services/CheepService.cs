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
}

public class CheepService : ICheepService
{

    private readonly ICheepRepository _cheepRepository;

    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        return await _cheepRepository.GetCheepsAsync(page);
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author)
    {
        // filter by the provided author name
        return await _cheepRepository.GetCheepsFromAuthorAsync(page, author);
    }

    public async Task<List<CheepDTO>> GetCheepsFromUserTimeline(int page, string author){
        return await _cheepRepository.GetCheepsFromUserTimelineAsync(page, author);
    }

    public async Task PostCheep(Cheep cheep){
        await _cheepRepository.PostCheepAsync(cheep);
    }

    public async Task FollowAuthor(string currentAuthorId, string targetAuthorId) {
        await _cheepRepository.FollowAuthorAsync(currentAuthorId, targetAuthorId);
    }
    
    public async Task UnfollowAuthor(string currentAuthorId, string targetAuthorId) {
        await _cheepRepository.UnfollowAuthorAsync(currentAuthorId, targetAuthorId);
    }

    public async Task<bool> IsFollowing(string currentAuthorId, string targetAuthorId) {
        return await _cheepRepository.IsFollowingAsync(currentAuthorId, targetAuthorId);
    }

    public async Task<List<string>> GetFollowing(string authorId){
        return await _cheepRepository.GetFollowingAsync(authorId);
    }
}
