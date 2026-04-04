using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Entities;

namespace WebApp.Pages.Quarries
{
    public class CreateModel : PageModel
    {
        private readonly DrillLogContext _context;

        public CreateModel(DrillLogContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Quarry Quarry { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Quarries.Add(Quarry);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Quarry '{Quarry.Name}' created successfully.";
            return RedirectToPage("./Index");
        }
    }
}
