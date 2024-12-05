using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Chirp.Infrastructure.Chirp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages
{
    public class UserTimelineModel : BaseCheepFormPage
    {
        public UserTimelineModel(ICheepService service, UserManager<Author> userManager) : base(service, userManager) { }

        public async Task<ActionResult> OnGet(string author)
        {
            var pageQuery = Request.Query["page"];
            PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;

            if (User.Identity!.IsAuthenticated)
            {
                var currentAuthor = await _userManager.GetUserAsync(User);
                await LoadFollowersAndFollowing(currentAuthor.Id);
                var currentAuthorName = currentAuthor!.Name;
                if (currentAuthorName == author)
                {
                    (Cheeps, CheepCount) = await _service.GetCheepsFromUserTimeline(PageNumber, author);
                }
                else
                {
                    (Cheeps, CheepCount) = await _service.GetCheepsFromAuthor(PageNumber, author);
                }
                await PopulateFollows();
            }
            else
            {
                (Cheeps, CheepCount) = await _service.GetCheepsFromAuthor(PageNumber, author);
            }

            return Page();
        }
    }
}