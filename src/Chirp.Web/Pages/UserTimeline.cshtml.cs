using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel : BaseCheepFormPage
{
    public required string Bio { get; set; }

    public UserTimelineModel(ICheepService service, UserManager<Author> userManager)
        : base(service, userManager) {}

    public async Task<ActionResult> OnGet(string author)
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;
        Cheeps = await _service.GetCheeps(PageNumber);

        var authorRequest = await _service.GetAuthorByName(author);

        if (authorRequest == null){
            return Page();
        }

        Bio = authorRequest.Bio;

        if(User.Identity!.IsAuthenticated){
            var currentAuthor = await _userManager.GetUserAsync(User);
            var currentAuthorName = currentAuthor!.Name;
            if(currentAuthorName == author){
                Cheeps = await _service.GetCheepsFromUserTimeline(PageNumber, authorRequest.Id);
            } else {
                Cheeps = await _service.GetCheepsFromAuthor(PageNumber, authorRequest.Id);
            }
            await PopulateFollows();
        } else {
            Cheeps = await _service.GetCheepsFromAuthor(PageNumber, authorRequest.Id);
        }

        return Page();
    }
}
