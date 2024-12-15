using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    protected readonly ICheepService _cheepService;
    protected readonly IAuthorService _authorService;
    protected readonly IAchievementService _achievementService;
    protected readonly UserManager<Author> _userManager;
    public required int PageNumber { get; set; }
    public required int CheepCount { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    public required List<string> Follows { get; set; }
    public required List<Achievement> Achievements { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    

    public AboutMeModel(ICheepService cheepService, IAuthorService authorService, IAchievementService achievementService, UserManager<Author> userManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _achievementService = achievementService;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
        Follows = new List<string>();
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
            (Cheeps, CheepCount) = await _cheepService.GetCheepsFromAuthorAsync(PageNumber, Name);
            Follows = await _authorService.GetFollowingAsync(currentAuthor.Id);
            Achievements = await _achievementService.GetAuthorAchievementsAsync(currentAuthor.Id);
        }
        return Page();
    }

    public async Task<ActionResult> OnPostForgetMe(){
        if(!User.Identity!.IsAuthenticated){
            return Page();
        }

        var currentAuthor = await _userManager.GetUserAsync(User);

        await _cheepService.DeleteCheepsByAuthorAsync(currentAuthor!.Id);
        await _authorService.DeleteFollowersAndFollowingAsync(currentAuthor.Id);
        await _achievementService.DeleteAuthorAchievementsAsync(currentAuthor.Id);
        await _userManager.DeleteAsync(currentAuthor);

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToPage("/Public");
    }
}