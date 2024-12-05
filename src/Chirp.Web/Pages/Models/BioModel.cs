using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class BioModel
{
    [StringLength(300)]
    public string? Bio { get; set; }
    public required string? RouteName { get; set; }
    public required bool IsMyBio { get; set; }
}