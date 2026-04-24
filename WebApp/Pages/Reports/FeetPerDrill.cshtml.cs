using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Reports;

public class FeetPerDrillModel : PageModel
{
    private readonly DrillLogContext _context;

    public FeetPerDrillModel(DrillLogContext context)
    {
        _context = context;
    }

    public DateTime ReportStartDate { get; private set; }

    public DateTime ReportEndDate { get; private set; }

    public IList<DrillFeetStat> DrillFeetStats { get; private set; } = new List<DrillFeetStat>();

    public async Task OnGetAsync()
    {
        ReportEndDate = DateTime.UtcNow.Date;
        ReportStartDate = ReportEndDate.AddYears(-1);

        // Pull raw rows from SQL first, then compute report stats in memory.
        var drillHoles = await _context.DrillHoles
            .Where(h => h.StartTime >= ReportStartDate && h.StartTime <= ReportEndDate)
            .Include(h => h.Machine)
            .AsNoTracking()
            .ToListAsync();

        DrillFeetStats = drillHoles
            .GroupBy(h => new { h.MachineId, h.Machine.ModelName })
            .Select(g => new DrillFeetStat
            {
                MachineId = g.Key.MachineId,
                ModelName = g.Key.ModelName.Trim(),
                HolesDrilled = g.Count(),
                TotalFeetDrilled = g.Sum(x => x.TotalDepth)
            })
            .OrderByDescending(x => x.TotalFeetDrilled)
            .ThenBy(x => x.MachineId)
            .ToList();
    }

    public sealed class DrillFeetStat
    {
        public int MachineId { get; set; }

        public string ModelName { get; set; } = string.Empty;

        public int HolesDrilled { get; set; }

        public decimal TotalFeetDrilled { get; set; }
    }
}
