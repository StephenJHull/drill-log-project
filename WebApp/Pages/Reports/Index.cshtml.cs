using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Reports;

[Authorize(Roles = "Admin,Manager")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
