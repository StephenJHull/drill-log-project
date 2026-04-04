using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    public class DeleteModel : PageModel
    {
        private readonly DrillLogContext _context;

        public DeleteModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DrillLog DrillLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var drillLog = await _context.DrillLogs
                .Include(d => d.DrillerSsnNavigation)
                .FirstOrDefaultAsync(d => d.ShotId == id);

            if (drillLog == null)
            {
                return NotFound();
            }

            DrillLog = drillLog;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var drillLog = await _context.DrillLogs.FindAsync(DrillLog.ShotId);
            if (drillLog != null)
            {
                // Remove dependent drill holes and patterns first
                var holes = await _context.DrillHoles.Where(h => h.ShotId == drillLog.ShotId).ToListAsync();
                _context.DrillHoles.RemoveRange(holes);

                var patterns = await _context.DrillPatterns.Where(p => p.ShotId == drillLog.ShotId).ToListAsync();
                _context.DrillPatterns.RemoveRange(patterns);

                _context.DrillLogs.Remove(drillLog);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Drill Log (Shot {drillLog.ShotId}) deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
