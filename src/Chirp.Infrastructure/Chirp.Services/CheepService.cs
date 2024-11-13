using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author);
    public Task PostCheep(Cheep cheep);
    public Task FollowAuthor(Author currentAuthor, Author targetAuthor);
    public Task UnfollowAuthor(Author currentAuthor, Author targetAuthor);
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

    public async Task PostCheep(Cheep cheep){
        await _cheepRepository.PostCheepAsync(cheep);
    }

    public async Task FollowAuthor(Author currentAuthor, Author targetAuthor) {
        await _cheepRepository.FollowAuthorAsync(currentAuthor, targetAuthor);
    }
    
    public async Task UnfollowAuthor(Author currentAuthor, Author targetAuthor) {
        await _cheepRepository.UnfollowAuthorAsync(currentAuthor, targetAuthor);
    }
}
