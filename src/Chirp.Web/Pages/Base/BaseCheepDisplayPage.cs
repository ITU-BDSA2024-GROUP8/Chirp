﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Base;

public class BaseCheepDisplayPage : PageModel
{
    public required List<CheepDTO> Cheeps { get; set; }
    public required int PageNumber { get; set; }
    public required int CheepCount { get; set; }
    protected readonly ICheepService _cheepService;
    protected readonly UserManager<Author> _userManager;

    public BaseCheepDisplayPage(ICheepService cheepService, UserManager<Author> userManager)
    {
        _cheepService = cheepService;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
        PageNumber = 1;
    }
}