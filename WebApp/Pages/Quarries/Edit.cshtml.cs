using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Pages.Quarries
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditModel : PageModel
    {
        private readonly DrillLogContext _context;

        public EditModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Quarry Quarry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var quarry = await _context.Quarries.FindAsync(id);
            if (quarry == null)
            {
                return NotFound();
            }

            Quarry = quarry;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Quarry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Quarries.AnyAsync(q => q.Name == Quarry.Name))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = $"Quarry '{Quarry.Name}' updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}
