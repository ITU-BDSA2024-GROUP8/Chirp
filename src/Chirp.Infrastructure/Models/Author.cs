using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Author
{
    [Required]
    public required int AuthorId { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Email { get; set; }
    
    public ICollection<Cheep>? Cheeps { get; set; }
}