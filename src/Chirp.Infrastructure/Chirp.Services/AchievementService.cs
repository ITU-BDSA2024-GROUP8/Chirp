using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Chirp.Services;

/// <summary>
/// IAchievementService interface is for defining the methods for the AchievementService
/// </summary>
public interface IAchievementService
{
    public Task AddNewAuthorAchievementAsync(string authorId, int achievementId);
    public Task<Achievement?> GetAuthorNewestAchievementAsync(string authorId);
    public Task<List<Achievement>> GetAuthorAchievementsAsync(string authorId);
}

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepository;

    public AchievementService(IAchievementRepository achievementRepository)
    {
        _achievementRepository = achievementRepository;
    }
    
    public async Task AddNewAuthorAchievementAsync(string authorId, int achievementId)
    {
        await _achievementRepository.AddNewAuthorAchievementAsync(authorId, achievementId);
    }
    
    public async Task<Achievement?> GetAuthorNewestAchievementAsync(string authorId)
    {
        return await _achievementRepository.GetAuthorNewestAchievementAsync(authorId);
    }

    public async Task<List<Achievement>> GetAuthorAchievementsAsync(string authorId)
    {
        return await _achievementRepository.GetAuthorAchievementsAsync(authorId);
    }
}