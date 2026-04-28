using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    [Authorize(Roles = "Admin,Driller,Blaster")]
    public class EditModel : PageModel
    {
        private readonly DrillLogContext _context;

        public EditModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DrillLog DrillLog { get; set; } = default!;

        public SelectList QuarryList { get; set; } = default!;
        public SelectList DrillerList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var drillLog = await _context.DrillLogs.FindAsync(id);
            if (drillLog == null)
            {
                return NotFound();
            }

            DrillLog = drillLog;
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

            _context.Attach(DrillLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DrillLogs.AnyAsync(d => d.ShotId == DrillLog.ShotId))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = $"Drill Log (Shot {DrillLog.ShotId}) updated successfully.";
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
