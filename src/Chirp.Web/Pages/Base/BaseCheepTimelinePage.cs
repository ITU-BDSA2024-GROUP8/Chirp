using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages.Base;

public class BaseCheepTimelinePage : BaseCheepDisplayPage
{
    [BindProperty]
    public CheepFormModel? FormData { get; set; }
    public Dictionary<string, bool> Follows { get; set; }
    protected readonly IAuthorService _authorService;
    

    public BaseCheepTimelinePage(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager) : base(cheepService, userManager)
    {
        _authorService = authorService;
        Follows = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnPost()
    {
        if (FormData == null || FormData.Message.Length > 300 || FormData.Message == null)
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