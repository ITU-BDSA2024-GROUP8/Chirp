using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DTOs;
/// <summary>
/// CheepDTO encapsulates data and minimizes unnecessary data exposure.
/// </summary>
public class CheepDTO
{
    [Required]
    public required string AuthorId { get; set; }
    
    [Required]
    public required string AuthorName { get; set; }
    
    [Required]
    public required string Message { get; set; }
    
    [Required]
    public required DateTime Timestamp { get; set; }
}