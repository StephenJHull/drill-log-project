using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    [Authorize (Roles = "Admin,Driller,Blaster")]
    public class CreateModel : PageModel
    {
        private readonly DrillLogContext _context;

        public CreateModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DrillLog DrillLog { get; set; } = default!;

        public SelectList QuarryList { get; set; } = default!;
        public SelectList DrillerList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("DrillLog.DrillerSsnNavigation");
            ModelState.Remove("DrillLog.QuarryNameNavigation");

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            _context.DrillLogs.Add(DrillLog);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Drill Log (Shot {DrillLog.ShotId}) created successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadDropdowns()
        {
            var quarries = await _context.Quarries.OrderBy(q => q.Name).ToListAsync();
            QuarryList = new SelectList(quarries, "Name", "Name");

            var drillers = await _context.Employees
                .Where(e => e.JobTitleNavigation != null && e.JobTitleNavigation.Role == "Driller")
                .OrderBy(e => e.LastName)
                .ToListAsync();
            DrillerList = new SelectList(
                drillers.Select(d => new { d.Ssn, Display = $"{d.FirstName} {d.LastName} ({d.JobTitle})" }),
                "Ssn", "Display");
        }
    }
}
