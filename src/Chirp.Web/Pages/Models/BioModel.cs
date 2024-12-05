using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class BioModel
{
    [Required (ErrorMessage = "Bio can be over 200 characters")]
    [StringLength(301)]
    public required string? RouteName { get; set; }
    public required bool IsMyBio { get; set; }
}