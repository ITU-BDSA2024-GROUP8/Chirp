﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Models;
using Chirp.Web.Pages.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel : BaseCheepFormPage
{
    public UserTimelineModel(ICheepService service, UserManager<Author> userManager)
        : base(service, userManager) {}

    public async Task<ActionResult> OnGet(string author)
    {
        var pageQuery = Request.Query["page"];
        int page = int.TryParse(pageQuery, out page) ? Math.Abs(page) : 1;
        Cheeps = await _service.GetCheepsFromUserTimeline(page, author);

        if(User.Identity!.IsAuthenticated){
            await FilterFollowers();
        }

        return Page();
    }
}
