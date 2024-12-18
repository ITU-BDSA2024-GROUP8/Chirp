using Chirp.Core.Models;

namespace Chirp.Core.Repositories;

public interface IAchievementRepository
{
    public Task AddNewAuthorAchievementAsync(string authorId, int achievementId);
    public Task<Achievement?> GetAuthorNewestAchievementAsync(string authorId);
    public Task<List<Achievement>> GetAuthorAchievementsAsync(string authorId);
}