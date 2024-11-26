using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    protected readonly ICheepService _service;
    protected readonly UserManager<Author> _userManager;
    public required int PageNumber { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    public List<string> Follows { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    

    public AboutMeModel(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    public async Task<ActionResult> OnGet()
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;

        if(User.Identity!.IsAuthenticated){
            var currentAuthor = await _userManager.GetUserAsync(User);
            Name = currentAuthor!.Name;
            Email = currentAuthor.Email!;
            Cheeps = await _service.GetCheepsFromAuthor(PageNumber, Name);
            Follows = await _service.GetFollowing(currentAuthor.Id);
        }
        return Page();
    }
}
