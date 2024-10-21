using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Models;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]

public class Author
{
    [Key]
    public int AuthorId { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required ICollection<Cheep> Cheeps { get; set; }
}