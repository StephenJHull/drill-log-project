using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Drills
{
    [Authorize(Roles = "Admin,Manager")]
    public class CreateModel : PageModel
    {
        private readonly DrillLogContext _context;

        public CreateModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Drill Drill { get; set; } = default!;

        public SelectList ModelList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
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

            _context.Drills.Add(Drill);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Drill (Machine {Drill.MachineId}) created successfully.";
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
