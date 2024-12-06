using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Chirp.Web.Pages.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel : BaseCheepFormPage
{
    [BindProperty]
    public BioText BioText { get; set; }

    public UserTimelineModel(ICheepService service, UserManager<Author> userManager)
        : base(service, userManager)
    {
        BioText = new BioText();
    }

    public async Task<ActionResult> OnGet(string author)
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;
        (Cheeps, CheepCount) = await _service.GetCheeps(PageNumber);

        var authorRequest = await _service.GetAuthorByName(author);

        if (authorRequest == null){
            return Page();
        }
        
        BioText.Bio = authorRequest.Bio;

        if(User.Identity!.IsAuthenticated){
            var currentAuthor = await _userManager.GetUserAsync(User);
            var currentAuthorName = currentAuthor!.Name;
            if(currentAuthorName == author){
                (Cheeps, CheepCount) = await _service.GetCheepsFromUserTimeline(PageNumber, authorRequest.Id);
            } else {
                (Cheeps, CheepCount) = await _service.GetCheepsFromAuthor(PageNumber, authorRequest.Id);
            }
            await PopulateFollows();
        } else {
            (Cheeps, CheepCount) = await _service.GetCheepsFromAuthor(PageNumber, authorRequest.Id);
        }

        return Page();
    }
    
    public async Task<ActionResult> OnPostUpdateBio(string? newBio)
    {
        if (User.Identity?.IsAuthenticated != true) return Page();

        if (newBio?.Length > 300)
        {
            return Page();
        }
        
        var currentAuthor = await _userManager.GetUserAsync(User);

        await _service.UpdateBio(currentAuthor!, newBio);
        
        return RedirectToPage();
    }
}
