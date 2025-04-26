using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCantabra1.Data;
using WebCantabra1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebCantabra1.Pages.Orders
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
        public Order Order { get; set; }
        public SelectList CustomerList { get; set; }
        public SelectList ProductList { get; set; }

        public void OnGet()
        {
            CustomerList = new SelectList(_context.Customers, "CustomerId", "Name");
            ProductList = new SelectList(_context.Products, "ProductId", "Name");
        }

        public async Task<IActionResult> OnPostAsync(int[] ProductIds, int[] Quantities)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Verificar que el cliente existe
                var customer = await _context.Customers.FindAsync(Order.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError("Order.CustomerId", "Cliente no encontrado");
                    return Page();
                }

                Order.OrderDate = DateTime.Now;
                Order.Status = "Pendiente";
                Order.OrderItems = new List<OrderItem>();

                for (int i = 0; i < ProductIds.Length; i++)
                {
                    var product = await _context.Products.FindAsync(ProductIds[i]);
                    if (product != null && Quantities[i] > 0)
                    {
                        Order.OrderItems.Add(new OrderItem
                        {
                            ProductId = ProductIds[i],
                            Quantity = Quantities[i],
                            UnitPrice = product.Price
                        });
                    }
                }

                Order.TotalAmount = Order.OrderItems.Sum(item => item.UnitPrice * item.Quantity);

                _context.Orders.Add(Order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Pedido creado con ID: {Order.OrderId}");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el pedido: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al crear el pedido");
                return Page();
            }
        }
    }
}