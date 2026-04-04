using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Quarries
{
    public class IndexModel : PageModel
    {
        private readonly DrillLogContext _context;

        public IndexModel(DrillLogContext context)
        {
            _context = context;
        }

        public IList<Quarry> Quarries { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Quarries = await _context.Quarries.OrderBy(q => q.Name).ToListAsync();
        }
    }
}
