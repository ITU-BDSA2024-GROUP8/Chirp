using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        var pageQuery = Request.Query["page"];
        int page = int.TryParse(pageQuery, out page) ? Math.Abs(page) : 1;
        Cheeps = _service.GetCheeps(page);
        return Page();
    }
}
