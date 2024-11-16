using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class AuthorFollower
{
    [Required]
    public required string FollowerId { get; set; }
    
    [Required]
    public Author Follower { get; set; }
    
    [Required]
    public required string FollowingId { get; set; }
    
    [Required]
    public Author Following { get; set; }
}