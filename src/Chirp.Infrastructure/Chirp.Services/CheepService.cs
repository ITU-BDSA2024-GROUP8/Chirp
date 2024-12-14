using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Chirp.Services;

public interface ICheepService
{
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheeps(int page);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthor(int page, string author);
    public Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimeline(int page, string author);
    public Task PostCheep(Cheep cheep);
    public Task DeleteCheepsByAuthor(string authorId);
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

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheeps(int page)
    {
        return await _cheepRepository.GetCheepsAsync(page);
    }

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromAuthor(int page, string author)
    {
        return await _cheepRepository.GetCheepsFromAuthorAsync(page, author);
    }

    public async Task<(List<CheepDTO> cheeps, int totalCheepCount)> GetCheepsFromUserTimeline(int page, string author)
    {
        return await _cheepRepository.GetCheepsFromUserTimelineAsync(page, author);
    }

    public async Task PostCheep(Cheep cheep)
    {
        await _cheepRepository.PostCheepAsync(cheep);
    }

    public async Task DeleteCheepsByAuthor(string authorId){
        await _authorRepository.DeleteCheepsByAuthorAsync(authorId);
    }
}