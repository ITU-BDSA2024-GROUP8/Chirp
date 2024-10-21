using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DTOs;

public class CheepDTO
{
    [Required]
    public required string Author { get; set; }
    [Required]
    public required string Message { get; set; }
    [Required]
    public required string Timestamp { get; set; }
}