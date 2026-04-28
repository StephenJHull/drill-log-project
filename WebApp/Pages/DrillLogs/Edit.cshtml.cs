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
        public SelectList ShotTypeList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var drillLog = await _context.DrillLogs.FindAsync(id);
            if (drillLog == null)
            {
                return NotFound();
            }

            // Load drill log header
            ShotId = drillLog.ShotId;
            DrillerSsn = drillLog.DrillerSsn;
            QuarryName = drillLog.QuarryName;

            // Load pattern
            var pattern = await _context.DrillPatterns.FirstOrDefaultAsync(p => p.ShotId == id);
            if (pattern != null)
            {
                BlasterSsn = pattern.BlasterSsn;
                ShotType = pattern.ShotType ?? string.Empty;
                HoleDiameter = pattern.HoleDiameter;
                Burden = pattern.Burden;
                Spacing = pattern.Spacing;
                FaceHeight = pattern.FaceHeight;
                SubDrill = pattern.SubDrill;
                NumberOfHoles = pattern.NoHoles;
                DateLaidOut = pattern.DesignDate;
                DateOfShot = pattern.ShotDate;
                MachineId = pattern.NoHoles > 0 ? 0 : 0; // preserve default; machine stored on holes
            }

            // Clear any stale modelstate so the form helpers reflect the freshly loaded model values
            ModelState.Clear();

            // Load holes
            var holes = await _context.DrillHoles
                .Where(h => h.ShotId == id)
                .OrderBy(h => h.HoleNo)
                .ToListAsync();

            if (holes.Any())
            {
                Holes = holes.Select(h => new HoleEntry
                {
                    HoleNo = h.HoleNo,
                    Degrees = h.Degrees,
                    BrokenMaterial = h.BrokenMaterial,
                    CompetentRock = h.CompetentRock ?? 0,
                    TotalDepth = h.TotalDepth,
                    WaterDepth = h.WaterDepth,
                    Notes = h.Notes,
                }).ToList();

                // set MachineId from first hole (assumes same machine for all holes)
                MachineId = holes.First().MachineId;
            }

            await LoadDropdowns();

            // Ensure the ShotTypeList includes the current value (in case DB contains a value not in defaults)
            var defaultTypes = new List<string> { "Production", "Controlled", "Pre-split", "Trim" };
            if (!string.IsNullOrWhiteSpace(ShotType) && !defaultTypes.Contains(ShotType))
            {
                defaultTypes.Insert(0, ShotType);
            }
            ShotTypeList = new SelectList(defaultTypes, ShotType);

            // (debug entry removed)

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Basic validation
            if (ShotId == 0)
            {
                ModelState.AddModelError("ShotId", "ShotId is required.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            // Update DrillLog
            var drillLog = await _context.DrillLogs.FindAsync(ShotId);
            if (drillLog == null)
            {
                return NotFound();
            }

            drillLog.DrillerSsn = DrillerSsn;
            drillLog.QuarryName = QuarryName;
            drillLog.TimeLastSaved = DateTime.Now;

            // Update or create DrillPattern
            var pattern = await _context.DrillPatterns.FirstOrDefaultAsync(p => p.ShotId == ShotId);
            if (pattern == null)
            {
                var maxPatternId = await _context.DrillPatterns.MaxAsync(p => (int?)p.PatternId) ?? 0;
                pattern = new DrillPattern
                {
                    PatternId = maxPatternId + 1,
                    ShotId = ShotId
                };
                _context.DrillPatterns.Add(pattern);
            }

            pattern.ShotNo = ShotId;
            pattern.HoleDiameter = HoleDiameter;
            pattern.Burden = Burden;
            pattern.Spacing = Spacing;
            pattern.FaceHeight = FaceHeight;
            pattern.SubDrill = SubDrill;
            // ShotType may sometimes not bind correctly in Model binding; fall back to the raw form value
            var postedShotType = ShotType;
            if (string.IsNullOrWhiteSpace(postedShotType))
            {
                postedShotType = Request.Form["ShotType"].FirstOrDefault();
            }

            // If still not provided, preserve existing DB value (if any)
            if (!string.IsNullOrWhiteSpace(postedShotType))
            {
                pattern.ShotType = postedShotType;
            }
            pattern.NoHoles = NumberOfHoles;
            pattern.DesignDate = DateLaidOut;
            pattern.ShotDate = DateOfShot;
            pattern.BlasterSsn = BlasterSsn;

            // Incremental hole updates: update existing holes by HoleNo, add new holes, preserve others
            var existingHoles = await _context.DrillHoles.Where(h => h.ShotId == ShotId).ToListAsync();
            var existingByHoleNo = existingHoles.ToDictionary(h => h.HoleNo);

            var maxHoleId = await _context.DrillHoles.MaxAsync(h => (int?)h.HoleId) ?? 0;

            if (Holes != null && Holes.Count > 0)
            {
                for (int i = 0; i < Holes.Count; i++)
                {
                    var h = Holes[i];

                    if (existingByHoleNo.TryGetValue(h.HoleNo, out var existing))
                    {
                        // update existing hole
                        existing.Degrees = h.Degrees;
                        existing.BrokenMaterial = h.BrokenMaterial;
                        existing.CompetentRock = h.CompetentRock;
                        existing.TotalDepth = h.TotalDepth;
                        existing.WaterDepth = h.WaterDepth;
                        existing.Notes = h.Notes;
                        existing.MachineId = MachineId;
                        // keep existing start/end times
                    }
                    else
                    {
                        // add new hole
                        maxHoleId++;
                        var drillHole = new DrillHole
                        {
                            HoleId = maxHoleId,
                            HoleNo = h.HoleNo,
                            Easting = 0,
                            Northing = 0,
                            Elevation = 0,
                            Degrees = h.Degrees,
                            BrokenMaterial = h.BrokenMaterial,
                            CompetentRock = h.CompetentRock,
                            TotalDepth = h.TotalDepth,
                            WaterDepth = h.WaterDepth,
                            Notes = h.Notes,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now,
                            MachineId = MachineId,
                            ShotId = ShotId
                        };

                        _context.DrillHoles.Add(drillHole);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Drill Log (Shot {ShotId}) updated successfully.";
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

            ShotTypeList = new SelectList(new[] { "Production", "Controlled", "Pre-split", "Trim" });
        }
    }

}
