using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class CheepFormModel
{
    [Required (ErrorMessage = "Cheep can't be empty")]
    [StringLength(160)]
    public string Message { get; set; }
}