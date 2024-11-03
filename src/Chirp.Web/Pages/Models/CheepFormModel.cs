using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Models;

public class CheepFormModel
{
    [Required]
    public string Message { get; set; }
}