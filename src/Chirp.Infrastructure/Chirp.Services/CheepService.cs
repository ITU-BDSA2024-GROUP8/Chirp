using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author);
    public Task PostCheep(CheepDTO cheepDTO);
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

    public async Task PostCheep(CheepDTO cheepDTO){
        await _cheepRepository.PostCheepAsync(cheepDTO);
    }
}
