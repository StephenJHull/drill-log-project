using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.DrillLogs
{
    public class NewDrillLogModel : PageModel
    {
        private readonly DrillLogContext _context;

        public NewDrillLogModel(DrillLogContext context)
        {
            _context = context;
        }

        // === Shot Layout Fields ===
        [BindProperty]
        public int ShotId { get; set; }

        [BindProperty]
        public string QuarryName { get; set; } = null!;

        [BindProperty]
        public int DrillerSsn { get; set; }

        [BindProperty]
        public int BlasterSsn { get; set; }

        [BindProperty]
        public string ShotType { get; set; } = null!;

        [BindProperty]
        public DateTime? DateLaidOut { get; set; }

        [BindProperty]
        public DateTime? DateOfShot { get; set; }

        [BindProperty]
        public decimal HoleDiameter { get; set; }

        [BindProperty]
        public decimal Burden { get; set; }

        [BindProperty]
        public decimal Spacing { get; set; }

        [BindProperty]
        public decimal FaceHeight { get; set; }

        [BindProperty]
        public decimal SubDrill { get; set; }

        [BindProperty]
        public int NumberOfHoles { get; set; }

        [BindProperty]
        public int MachineId { get; set; }

        // === Hole Data ===
        [BindProperty]
        public List<HoleEntry> Holes { get; set; } = new();

        // === Dropdowns ===
        public SelectList QuarryList { get; set; } = default!;
        public SelectList DrillerList { get; set; } = default!;
        public SelectList BlasterList { get; set; } = default!;
        public SelectList MachineList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // We skip full model validation since we have custom binding
            var now = DateTime.Now;

            // 1. Create the DrillLog record
            var drillLog = new DrillLog
            {
                ShotId = ShotId,
                TimeBegan = now,
                TimeLastSaved = now,
                TimeSubmitted = now,
                DrillerSsn = DrillerSsn,
                QuarryName = QuarryName
            };

            _context.DrillLogs.Add(drillLog);

            // 2. Create the DrillPattern record
            // Generate a PatternId
            var maxPatternId = await _context.DrillPatterns.MaxAsync(p => (int?)p.PatternId) ?? 0;
            var drillPattern = new DrillPattern
            {
                PatternId = maxPatternId + 1,
                ShotNo = ShotId,
                HoleDiameter = HoleDiameter,
                Burden = Burden,
                Spacing = Spacing,
                FaceHeight = FaceHeight,
                SubDrill = SubDrill,
                ShotType = ShotType,
                NoHoles = NumberOfHoles,
                DesignDate = DateLaidOut,
                ShotDate = DateOfShot,
                BlasterSsn = BlasterSsn,
                ShotId = ShotId
            };

            _context.DrillPatterns.Add(drillPattern);

            // 3. Create DrillHole records
            var maxHoleId = await _context.DrillHoles.MaxAsync(h => (int?)h.HoleId) ?? 0;
            var holeStartId = maxHoleId + 1;

            if (Holes != null && Holes.Count > 0)
            {
                for (int i = 0; i < Holes.Count; i++)
                {
                    var h = Holes[i];
                    var drillHole = new DrillHole
                    {
                        HoleId = holeStartId + i,
                        HoleNo = h.HoleNo,
                        Easting = 0, // No GPS data from paper log
                        Northing = 0,
                        Elevation = 0,
                        Degrees = h.Degrees,
                        BrokenMaterial = h.BrokenMaterial,
                        CompetentRock = h.CompetentRock,
                        TotalDepth = h.TotalDepth,
                        WaterDepth = h.WaterDepth,
                        Notes = h.Notes,
                        StartTime = now,
                        EndTime = now,
                        MachineId = MachineId,
                        ShotId = ShotId
                    };

                    _context.DrillHoles.Add(drillHole);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Drill Log (Shot {ShotId}) with {Holes?.Count ?? 0} holes saved successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving drill log: {ex.InnerException?.Message ?? ex.Message}");
                await LoadDropdowns();
                return Page();
            }
        }

        private async Task LoadDropdowns()
        {
            var quarries = await _context.Quarries.OrderBy(q => q.Name).ToListAsync();
            QuarryList = new SelectList(quarries, "Name", "Name");

            var drillers = await _context.Employees
                .Include(e => e.JobTitleNavigation)
                .Where(e => e.JobTitleNavigation != null && e.JobTitleNavigation.Role == "Driller")
                .OrderBy(e => e.LastName)
                .ToListAsync();
            DrillerList = new SelectList(
                drillers.Select(d => new { d.Ssn, Display = $"{d.FirstName} {d.LastName}" }),
                "Ssn", "Display");

            var blasters = await _context.Employees
                .Include(e => e.JobTitleNavigation)
                .Where(e => e.JobTitleNavigation != null && e.JobTitleNavigation.Role == "Blaster")
                .OrderBy(e => e.LastName)
                .ToListAsync();
            BlasterList = new SelectList(
                blasters.Select(b => new { b.Ssn, Display = $"{b.FirstName} {b.LastName}" }),
                "Ssn", "Display");

            var machines = await _context.Drills
                .Include(d => d.ModelNameNavigation)
                .OrderBy(d => d.MachineId)
                .ToListAsync();
            MachineList = new SelectList(
                machines.Select(m => new { m.MachineId, Display = $"{m.ModelNameNavigation?.Manufacture} {m.ModelName} (ID: {m.MachineId})" }),
                "MachineId", "Display");
        }
    }

    // Simple DTO for binding hole entries from the form
    public class HoleEntry
    {
        public int HoleNo { get; set; }
        public decimal Degrees { get; set; }
        public decimal BrokenMaterial { get; set; }
        public decimal? CompetentRock { get; set; }
        public decimal TotalDepth { get; set; }
        public int? WaterDepth { get; set; }
        public string? Notes { get; set; }
    }
}
