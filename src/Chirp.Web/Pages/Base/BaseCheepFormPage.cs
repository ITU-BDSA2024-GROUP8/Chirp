﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Base;

public class BaseCheepFormPage : PageModel
{
    [BindProperty]
    public CheepFormModel FormData { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    protected readonly ICheepService _service;
    protected readonly UserManager<Author> _userManager;

    public BaseCheepFormPage(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var author = await _userManager.GetUserAsync(User);
        
        await _service.PostCheep(new Cheep
        {
            Author = author,
            AuthorId = author.Id,
            Text = FormData.Message,
            TimeStamp = DateTime.Now
        });

        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostFollowAuthor(string targetAuthorName)
    {
        var currentAuthor = await _userManager.GetUserAsync(User);

        await _service.FollowAuthor(currentAuthor!.Name, targetAuthorName);

        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAuthor(string targetAuthorName)
    {
        var currentAuthor = await _userManager.GetUserAsync(User);

        await _service.UnfollowAuthor(currentAuthor!.Name, targetAuthorName);

        return RedirectToPage();
    }
}