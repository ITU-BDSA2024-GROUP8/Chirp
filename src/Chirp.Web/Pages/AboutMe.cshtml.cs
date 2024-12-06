using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Chirp.Web.Pages.Base;

namespace Chirp.Web.Pages
{
    public class AboutMeModel : BaseCheepFormPage
    {
        protected readonly IAchievementService _achievementService;
        public required List<Achievement> Achievements { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public AboutMeModel(ICheepService cheepService, IAchievementService achievementService, UserManager<Author> userManager)
            : base(cheepService, userManager)
        {
            _achievementService = achievementService;
            Cheeps = new List<CheepDTO>();
            Achievements = new List<Achievement>();
        }

        public async Task<ActionResult> OnGet()
        {
            var pageQuery = Request.Query["page"];
            PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;

            if (User.Identity!.IsAuthenticated)
            {
                var currentAuthor = await _userManager.GetUserAsync(User);
                Name = currentAuthor!.Name;
                Email = currentAuthor.Email!;
                (Cheeps, CheepCount) = await _service.GetCheepsFromAuthor(PageNumber, Name);
                Achievements = await _achievementService.GetAuthorAchievements(currentAuthor.Id);
                await LoadFollowersAndFollowing(currentAuthor.Id);
            }
            return Page();
        }

        public async Task<ActionResult> OnPostForgetMe()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Page();
            }

            var currentAuthor = await _userManager.GetUserAsync(User);

            await _service.DeleteCheepsByAuthor(currentAuthor!.Id);
            await _service.DeleteFollowersAndFollowing(currentAuthor.Id);
            await _achievementService.DeleteAuthorAchievements(currentAuthor.Id);
            await _userManager.DeleteAsync(currentAuthor);

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("/Public");
        }
    }
}