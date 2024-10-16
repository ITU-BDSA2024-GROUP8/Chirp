using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Repositories;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int page, string author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };
    
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

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
