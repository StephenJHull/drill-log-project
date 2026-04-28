using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DrillLogContext _context;

        public IndexModel(DrillLogContext context)
        {
            _context = context;
        }

        public IList<DrillLog> DrillLogs { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DrillLogs = await _context.DrillLogs
                .Include(d => d.DrillerSsnNavigation)
                .Include(d => d.QuarryNameNavigation)
                .OrderByDescending(d => d.TimeBegan)
                .ToListAsync();
        }
    }
}
