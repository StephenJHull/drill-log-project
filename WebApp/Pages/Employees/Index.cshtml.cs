using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DrillLogContext _context;

        public IndexModel(DrillLogContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Employees = await _context.Employees
                .Include(e => e.SupervisorSsnNavigation)
                .OrderBy(e => e.Ssn)
                .ToListAsync();
        }
    }
}
