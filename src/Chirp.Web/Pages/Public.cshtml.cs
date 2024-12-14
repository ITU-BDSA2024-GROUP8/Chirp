using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel : BaseCheepFormPage
{
    public PublicModel(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager) 
        : base(cheepService, authorService, userManager) {}

    public async Task<ActionResult> OnGet()
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;
        (Cheeps, CheepCount) = await _cheepService.GetCheeps(PageNumber);

        if(User.Identity!.IsAuthenticated){
            await PopulateFollows();
        }
        
        return Page();
    }
}