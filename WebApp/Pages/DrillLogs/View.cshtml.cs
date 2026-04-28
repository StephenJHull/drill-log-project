using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    public class ViewModel : PageModel
    {
        private readonly DrillLogContext _context;

        public ViewModel(DrillLogContext context)
        {
            _context = context;
        }

        public DrillLog? DrillLog { get; set; }
        public DrillPattern? DrillPattern { get; set; }
        public List<DrillHole> DrillHoles { get; set; } = new();

        public string? DrillerName { get; set; }
        public string? BlasterName { get; set; }
        public string? MachineDisplay { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DrillLog = await _context.DrillLogs
                .Include(d => d.DrillerSsnNavigation)
                .Include(d => d.QuarryNameNavigation)
                .FirstOrDefaultAsync(d => d.ShotId == id.Value);

            if (DrillLog == null)
            {
                return NotFound();
            }

            DrillPattern = await _context.DrillPatterns.FirstOrDefaultAsync(p => p.ShotId == id.Value);

            DrillHoles = await _context.DrillHoles
                .Where(h => h.ShotId == id.Value)
                .OrderBy(h => h.HoleNo)
                .ToListAsync();

            if (DrillLog.DrillerSsnNavigation != null)
            {
                DrillerName = $"{DrillLog.DrillerSsnNavigation.FirstName} {DrillLog.DrillerSsnNavigation.LastName}";
            }

            if (DrillPattern != null && DrillPattern.BlasterSsn != 0)
            {
                var blaster = await _context.Employees.FindAsync(DrillPattern.BlasterSsn);
                if (blaster != null)
                {
                    BlasterName = $"{blaster.FirstName} {blaster.LastName}";
                }
            }

            if (DrillHoles.Any())
            {
                var machineId = DrillHoles.First().MachineId;
                var machine = await _context.Drills
                    .Include(d => d.ModelNameNavigation)
                    .FirstOrDefaultAsync(d => d.MachineId == machineId);
                if (machine != null)
                {
                    MachineDisplay = $"{machine.ModelNameNavigation?.Manufacture} {machine.ModelName} (ID: {machine.MachineId})";
                }
            }

            return Page();
        }
    }
}
