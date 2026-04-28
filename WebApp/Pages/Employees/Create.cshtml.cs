using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly DrillLogContext _context;

        public CreateModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public SelectList JobTitleList { get; set; } = default!;
        public SelectList SupervisorList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove navigation properties from validation
            ModelState.Remove("Employee.JobTitleNavigation");
            ModelState.Remove("Employee.SupervisorSsnNavigation");

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Employee {Employee.FirstName} {Employee.LastName} created successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadDropdowns()
        {
            var roles = await _context.EmployeeRoles.OrderBy(r => r.JobTitle).ToListAsync();
            JobTitleList = new SelectList(roles, "JobTitle", "JobTitle");

            var supervisors = await _context.Employees
                .Where(e => e.JobTitle != null && (e.JobTitle.Contains("Manager") || e.JobTitle.Contains("Lead") || e.JobTitle.Contains("Senior")))
                .OrderBy(e => e.LastName)
                .ToListAsync();
            SupervisorList = new SelectList(
                supervisors.Select(s => new { s.Ssn, Display = $"{s.FirstName} {s.LastName} ({s.JobTitle})" }),
                "Ssn", "Display");
        }
    }
}
