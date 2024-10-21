using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Cheep
{
    [Required]
    public required int CheepId { get; set; }

    [Required]
    [StringLength(500)]
    public required string Text { get; set; }

    [Required]
    public required DateTime TimeStamp { get; set; }

    [Required]
    public required int AuthorId { get; set; }
    
    [Required]
    public required Author Author { get; set; }
}