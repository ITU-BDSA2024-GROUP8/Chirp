using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel : BaseCheepFormPage
{
    public PublicModel(ICheepService service, UserManager<Author> userManager) 
        : base(service, userManager) {}

    public async Task<ActionResult> OnGet()
    {
        var pageQuery = Request.Query["page"];
        int page = int.TryParse(pageQuery, out page) ? Math.Abs(page) : 1;
        Cheeps = await _service.GetCheeps(page);

        var currentAuthor = await _userManager.GetUserAsync(User);
        var currentAuthorName = currentAuthor!.Name;

        foreach (var cheep in Cheeps)
        {
            var targetAuthorName = cheep.Author;
            if(Follows.ContainsKey(targetAuthorName)){
                continue;
            }
            var isFollowing = await _service.IsFollowing(currentAuthorName, targetAuthorName);
            Follows[targetAuthorName] = isFollowing;
        }

        return Page();
    }
}
