using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    [BindProperty]
    public CheepFormModel FormData { get; set; }
    
    private readonly ICheepService _service;
    private readonly UserManager<Author> _userManager;
    public required List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        var pageQuery = Request.Query["page"];
        int page = int.TryParse(pageQuery, out page) ? Math.Abs(page) : 1;
        Cheeps = await _service.GetCheepsFromAuthor(page, author);
        return Page();
    }
    
    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        var author = await _userManager.GetUserAsync(User);
        
        await _service.PostCheep(new Cheep
        {
            Author = author,
            AuthorId = author.Id,
            Text = FormData.Message,
            TimeStamp = DateTime.Now
        });

        return RedirectToPage();
    }
}
