using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Chirp.Services;

public interface IAchievementService
{
    public Task AddNewAuthorAchievement(string authorId, int achievementId);
    public Task<Achievement?> GetAuthorNewestAchievement(string authorId);
    public Task<List<Achievement>> GetAuthorAchievements(string authorId);
    public Task DeleteAuthorAchievements(string authorId);
}

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepository;

    public AchievementService(IAchievementRepository achievementRepository)
    {
        _achievementRepository = achievementRepository;
    }
    
    public async Task AddNewAuthorAchievement(string authorId, int achievementId)
    {
        await _achievementRepository.AddNewAuthorAchievementAsync(authorId, achievementId);
    }
    
    public async Task<Achievement?> GetAuthorNewestAchievement(string authorId)
    {
        return await _achievementRepository.GetAuthorNewestAchievementAsync(authorId);
    }

    public async Task<List<Achievement>> GetAuthorAchievements(string authorId)
    {
        return await _achievementRepository.GetAuthorAchievementsAsync(authorId);
    }
    
    public async Task DeleteAuthorAchievements(string authorId)
    {
        await _achievementRepository.DeleteAuthorAchievementsAsync(authorId);
    }
}