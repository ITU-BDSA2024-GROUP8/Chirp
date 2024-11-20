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
}

public class CheepService : ICheepService
{
    private readonly ICheepQueryRepository _cheepQueryRepository;
    private readonly ICheepCommandRepository _cheepCommandRepository;

    public CheepService(ICheepQueryRepository cheepQueryRepository, ICheepCommandRepository cheepCommandRepository)
    {
        _cheepQueryRepository = cheepQueryRepository;
        _cheepCommandRepository = cheepCommandRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps(int page)
    {
        return await _cheepQueryRepository.GetCheepsAsync(page);
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author)
    {
        // filter by the provided author name
        return await _cheepQueryRepository.GetCheepsFromAuthorAsync(page, author);
    }

    public async Task<List<CheepDTO>> GetCheepsFromUserTimeline(int page, string author){
        return await _cheepQueryRepository.GetCheepsFromUserTimelineAsync(page, author);
    }

    public async Task PostCheep(Cheep cheep){
        await _cheepCommandRepository.PostCheepAsync(cheep);
    }

    public async Task FollowAuthor(string currentAuthorId, string targetAuthorId) {
        await _cheepCommandRepository.FollowAuthorAsync(currentAuthorId, targetAuthorId);
    }
    
    public async Task UnfollowAuthor(string currentAuthorId, string targetAuthorId) {
        await _cheepCommandRepository.UnfollowAuthorAsync(currentAuthorId, targetAuthorId);
    }

    public async Task<bool> IsFollowing(string currentAuthorId, string targetAuthorId) {
        return await _cheepCommandRepository.IsFollowingAsync(currentAuthorId, targetAuthorId);
    }
}