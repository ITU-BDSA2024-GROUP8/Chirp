using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;
/// <summary>
/// AboutMeModel class is for displaying the current author's information
/// This class is inheriting from the BaseCheepDisplayPage class
/// This class is using the IAchievementService interface
/// </summary>
public class AboutMeModel : BaseCheepDisplayPage
{
    protected readonly IAchievementService _achievementService;
    public required List<Achievement> Achievements { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    

    public AboutMeModel(ICheepService cheepService, IAchievementService achievementService, UserManager<Author> userManager) : base(cheepService, userManager)
    {
        _achievementService = achievementService;
        Achievements = new List<Achievement>();
    }

    public async Task<ActionResult> OnGet()
    {
        var pageQuery = Request.Query["page"];
        PageNumber = int.TryParse(pageQuery, out var page) ? Math.Max(page, 1) : 1;

        if(User.Identity!.IsAuthenticated){
            var currentAuthor = await _userManager.GetUserAsync(User);
            Name = currentAuthor!.Name;
            Email = currentAuthor.Email!;
            (Cheeps, CheepCount) = await _cheepService.GetCheepsFromAuthorAsync(PageNumber, currentAuthor.Id);
            Achievements = await _achievementService.GetAuthorAchievementsAsync(currentAuthor.Id);
        }
        return Page();
    }

/// <summary>
/// This method is for deleting the current author and signing out
/// This method is not the same as the delete method in the build in the API
/// </summary>
    public async Task<ActionResult> OnPostForgetMe(){
        if(!User.Identity!.IsAuthenticated){
            return Page();
        }

        var currentAuthor = await _userManager.GetUserAsync(User);

        await _userManager.DeleteAsync(currentAuthor!);

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToPage("/Public");
    }
}