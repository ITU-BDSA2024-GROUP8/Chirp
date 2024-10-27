using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Models;

public class Author : IdentityUser
{
    [Required]
    public required ICollection<Cheep> Cheeps { get; set; }
}