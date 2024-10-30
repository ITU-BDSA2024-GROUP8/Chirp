using Chirp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class CheepBoxPartialModel : PageModel
{
    private readonly ICheepService _service;

    [BindProperty]
    public required string Text { get; set; }

    public CheepBoxPartialModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _service.PostCheep(new CheepDTO
        {
            Message = Text,
            Author = User.Identity!.Name!,
            Timestamp = DateTime.Now
        });

        return RedirectToPage();
    }
}