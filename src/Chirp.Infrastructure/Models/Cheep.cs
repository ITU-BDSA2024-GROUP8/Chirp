﻿using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Cheep
{
    [Key]
    public int CheepId { get; set; }

    [Required]
    [StringLength(160)]
    public required string Text { get; set; }

    [Required]
    public required DateTime TimeStamp { get; set; }

    [Required]
    public required string AuthorId { get; set; }
    
    [Required]
    public Author Author { get; set; }
}