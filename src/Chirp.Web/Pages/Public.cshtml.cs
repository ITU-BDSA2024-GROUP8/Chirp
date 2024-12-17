using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;
/// <summary>
/// PublicModel class is for displaying the public cheeps
/// Maximum of 32 cheeps per page
/// </summary>
public class PublicModel : BaseCheepTimelinePage
{
    public PublicModel(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager) 
        : base(cheepService, authorService, userManager) {}

    public async Task<ActionResult> OnGet()
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;
        (Cheeps, CheepCount) = await _cheepService.GetCheepsAsync(PageNumber);

        if(User.Identity!.IsAuthenticated){
            await PopulateFollows();
        }
        
        return Page();
    }
}