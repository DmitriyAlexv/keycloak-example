using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyCloakExample.Web.Pages;

[Authorize]
public class SecureModel : PageModel
{
    public string? AccessToken { get; private set; }

    public async Task OnGetAsync()
    {
        ViewData["Title"] = "Защищённая страница";
        AccessToken = await HttpContext.GetTokenAsync("access_token");
    }
}
