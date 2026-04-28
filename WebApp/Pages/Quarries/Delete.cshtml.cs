using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Entities;

namespace WebApp.Pages.Quarries
{
    [Authorize(Roles = "Admin,Manager")]
    public class DeleteModel : PageModel
    {
        private readonly DrillLogContext _context;

        public DeleteModel(DrillLogContext context)
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
            var quarry = await _context.Quarries.FindAsync(Quarry.Name);
            if (quarry != null)
            {
                _context.Quarries.Remove(quarry);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Quarry '{quarry.Name}' deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
