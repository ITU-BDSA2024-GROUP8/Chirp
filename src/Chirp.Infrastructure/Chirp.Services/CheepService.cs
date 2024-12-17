using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Chirp.Services;
/// <summary>
/// ICheepService is for defining the methods for the CheepService
/// </summary>
public interface ICheepService
{
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsAsync(int page);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthorAsync(int page, string authorId);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimelineAsync(int page, string authorId);
    public Task PostCheepAsync(Cheep cheep);
}

/// <summary>
/// CheepService is for buissness logic regarding cheeps.
/// </summary>
public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }
    
    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsAsync(int page)
    {
        return await _cheepRepository.GetCheepsAsync(page);
    }

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthorAsync(int page, string authorId)
    {
        return await _cheepRepository.GetCheepsFromAuthorAsync(page, authorId);
    }

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimelineAsync(int page, string authorId)
    {
        return await _cheepRepository.GetCheepsFromUserTimelineAsync(page, authorId);
    }

    public async Task PostCheepAsync(Cheep cheep)
    {
        await _cheepRepository.PostCheepAsync(cheep);
    }
}