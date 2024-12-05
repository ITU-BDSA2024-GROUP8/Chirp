using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class BioModel
{
    [Required]
    public required string? RouteName { get; set; }
    [Required]
    public required bool IsMyBio { get; set; }
    public BioText? BioText { get; set; }
}

public class BioText
{
    [StringLength(300)]
    public string? Bio { get; set; }
}