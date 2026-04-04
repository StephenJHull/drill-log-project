using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly DrillLogContext _context;

        public EditModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public SelectList JobTitleList { get; set; } = default!;
        public SelectList SupervisorList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            Employee = employee;
            await LoadDropdowns(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Employee.JobTitleNavigation");
            ModelState.Remove("Employee.SupervisorSsnNavigation");

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(Employee.Ssn);
                return Page();
            }

            _context.Attach(Employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Employees.AnyAsync(e => e.Ssn == Employee.Ssn))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = $"Employee {Employee.FirstName} {Employee.LastName} updated successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadDropdowns(int currentSsn)
        {
            var roles = await _context.EmployeeRoles.OrderBy(r => r.JobTitle).ToListAsync();
            JobTitleList = new SelectList(roles, "JobTitle", "JobTitle");

            var supervisors = await _context.Employees
                .Where(e => e.Ssn != currentSsn)
                .OrderBy(e => e.LastName)
                .ToListAsync();
            SupervisorList = new SelectList(
                supervisors.Select(s => new { s.Ssn, Display = $"{s.FirstName} {s.LastName} ({s.JobTitle})" }),
                "Ssn", "Display");
        }
    }
}
