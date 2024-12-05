using System.ComponentModel.DataAnnotations;
using Chirp.Core.DTOs;
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
    protected readonly ICheepService _service;
    protected readonly UserManager<Author> _userManager;

    public BaseCheepFormPage(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
        PageNumber = 1;
        Follows = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnPost()
    {
        ModelState.Remove("Bio");
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var author = await _userManager.GetUserAsync(User);

        if (author == null) return Page();
        
        await _service.PostCheep(new Cheep
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

        await _service.FollowAuthor(currentAuthor!.Id, targetAuthorId);

        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAuthor(string targetAuthorId)
    {
        if (User.Identity?.IsAuthenticated != true) return Page();
        
        var currentAuthor = await _userManager.GetUserAsync(User);
        
        if (targetAuthorId == currentAuthor!.Id) return Page();

        await _service.UnfollowAuthor(currentAuthor!.Id, targetAuthorId);

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
            var isFollowing = await _service.IsFollowing(currentAuthorId, targetAuthorId);
            Follows[targetAuthorId] = isFollowing;
        }
    }
}