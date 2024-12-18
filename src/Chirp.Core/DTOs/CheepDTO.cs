namespace Chirp.Core.DTOs;

public class CheepDTO
{
    public required string AuthorId { get; set; }
    public required string AuthorName { get; set; }
    public required string Message { get; set; }
    public required DateTime Timestamp { get; set; }
}