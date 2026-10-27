using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyCloakExample.Web.Pages;

public class IndexModel : PageModel
{
    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public string UserName =>
        User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "неизвестный";

    public void OnGet()
    {
        ViewData["Title"] = "KeyCloakExample.Web";
    }
}
