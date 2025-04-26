using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebCantabra1.Data;
using WebCantabra1.Models;

namespace WebCantabra1.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.Customers.Add(Customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cliente creado exitosamente: {Customer.Name}");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el cliente: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al guardar el cliente. Por favor, inténtelo de nuevo.");
                return Page();
            }
        }
    }
}