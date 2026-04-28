using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Drills
{
    [Authorize(Roles = "Admin,Manager,Driller")]
    public class IndexModel : PageModel
    {
        private readonly DrillLogContext _context;

        public IndexModel(DrillLogContext context)
        {
            _context = context;
        }

        public IList<Drill> Drills { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Drills = await _context.Drills
                .Include(d => d.ModelNameNavigation)
                .OrderBy(d => d.MachineId)
                .ToListAsync();
        }
    }
}
