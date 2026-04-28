using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Drills
{
    [Authorize(Roles = "Admin,Manager")]
    public class DeleteModel : PageModel
    {
        private readonly DrillLogContext _context;

        public DeleteModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Drill Drill { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var drill = await _context.Drills
                .Include(d => d.ModelNameNavigation)
                .FirstOrDefaultAsync(d => d.MachineId == id);

            if (drill == null)
            {
                return NotFound();
            }

            Drill = drill;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var drill = await _context.Drills.FindAsync(Drill.MachineId);
            if (drill != null)
            {
                // Remove dependent drill holes first
                var holes = await _context.DrillHoles.Where(h => h.MachineId == drill.MachineId).ToListAsync();
                _context.DrillHoles.RemoveRange(holes);

                // Remove dependent drilling instances
                var drillings = await _context.Drillings.Where(d => d.MachineId == drill.MachineId).ToListAsync();
                _context.Drillings.RemoveRange(drillings);

                _context.Drills.Remove(drill);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Drill (Machine {drill.MachineId}) deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
