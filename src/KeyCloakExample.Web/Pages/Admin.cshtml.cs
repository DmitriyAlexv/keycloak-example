using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyCloakExample.Web.Pages;

[Authorize(Roles = "demo-user")]
public class AdminModel : PageModel
{
    public string UserName =>
        User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "неизвестный";

    public void OnGet()
    {
        ViewData["Title"] = "Только для demo-user";
    }
}
