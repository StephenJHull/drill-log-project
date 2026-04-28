using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Drills
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditModel : PageModel
    {
        private readonly DrillLogContext _context;

        public EditModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Drill Drill { get; set; } = default!;

        public SelectList ModelList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var drill = await _context.Drills.FindAsync(id);
            if (drill == null)
            {
                return NotFound();
            }

            Drill = drill;
            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Drill.ModelNameNavigation");

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            _context.Attach(Drill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Drills.AnyAsync(d => d.MachineId == Drill.MachineId))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = $"Drill (Machine {Drill.MachineId}) updated successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadDropdowns()
        {
            var types = await _context.DrillTypes.OrderBy(t => t.ModelName).ToListAsync();
            ModelList = new SelectList(
                types.Select(t => new { t.ModelName, Display = $"{t.ModelName} ({t.Manufacture})" }),
                "ModelName", "Display");
        }
    }
}
