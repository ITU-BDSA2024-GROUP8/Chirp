using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class CheepFormModel
{
    [Required]
    [StringLength(160)]
    public string Message { get; set; }
}