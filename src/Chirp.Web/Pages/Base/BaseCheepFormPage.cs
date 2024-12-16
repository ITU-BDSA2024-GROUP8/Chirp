using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Base;

public class BaseCheepFormPage : PageModel
{
    [BindProperty]
    public CheepFormModel? FormData { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    public required int PageNumber { get; set; }
    public required int CheepCount { get; set; }
    public Dictionary<string, bool> Follows { get; set; }
    protected readonly ICheepService _cheepService;
    protected readonly IAuthorService _authorService;
    protected readonly UserManager<Author> _userManager;

    public BaseCheepFormPage(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userManager = userManager;
        PageNumber = 1;
        Follows = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var author = await _userManager.GetUserAsync(User);

        if (author == null) return Page();
        
        await _cheepService.PostCheepAsync(new Cheep
        {
            AuthorId = author.Id,
            Text = FormData!.Message,
            TimeStamp = DateTime.Now
        });

        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostFollowAuthor(string targetAuthorId)
    {
        if (User.Identity?.IsAuthenticated != true) return Page();
        
        var currentAuthor = await _userManager.GetUserAsync(User);

        if (targetAuthorId == currentAuthor!.Id) return Page();

        await _authorService.FollowAuthorAsync(currentAuthor!.Id, targetAuthorId);

        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAuthor(string targetAuthorId)
    {
        if (User.Identity?.IsAuthenticated != true) return Page();
        
        var currentAuthor = await _userManager.GetUserAsync(User);
        
        if (targetAuthorId == currentAuthor!.Id) return Page();

        await _authorService.UnfollowAuthorAsync(currentAuthor!.Id, targetAuthorId);

        return RedirectToPage();
    }

    public async Task PopulateFollows(){
        var currentAuthor = await _userManager.GetUserAsync(User);
        var currentAuthorId = currentAuthor!.Id;

        foreach (var cheep in Cheeps)
        {
            var targetAuthorId = cheep.AuthorId;
            if(Follows.ContainsKey(targetAuthorId)){
                continue;
            }
            var isFollowing = await _authorService.IsFollowingAsync(currentAuthorId, targetAuthorId);
            Follows[targetAuthorId] = isFollowing;
        }
    }
}