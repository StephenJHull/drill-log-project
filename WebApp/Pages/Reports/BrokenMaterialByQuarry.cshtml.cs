using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Reports;

public class BrokenMaterialByQuarryModel : PageModel
{
    private readonly DrillLogContext _context;

    public BrokenMaterialByQuarryModel(DrillLogContext context)
    {
        _context = context;
    }

    public IList<QuarryBrokenMaterialStat> QuarryStats { get; private set; } = new List<QuarryBrokenMaterialStat>();

    public async Task OnGetAsync()
    {
        // Pull raw rows from SQL first, then compute report stats in memory.
        var drillHoles = await _context.DrillHoles
            .Include(h => h.Shot)
            .ThenInclude(s => s.QuarryNameNavigation)
            .AsNoTracking()
            .ToListAsync();

        QuarryStats = drillHoles
            .GroupBy(h => h.Shot.QuarryName.Trim())
            .Select(g => new QuarryBrokenMaterialStat
            {
                QuarryName = g.Key,
                HoleCount = g.Count(),
                AverageBrokenMaterialFeet = g.Average(x => x.BrokenMaterial)
            })
            .OrderByDescending(x => x.AverageBrokenMaterialFeet)
            .ThenBy(x => x.QuarryName)
            .ToList();
    }

    public sealed class QuarryBrokenMaterialStat
    {
        public string QuarryName { get; set; } = string.Empty;

        public int HoleCount { get; set; }

        public decimal AverageBrokenMaterialFeet { get; set; }
    }
}
