using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebCantabra1.Data;
using WebCantabra1.Models;
using Microsoft.Extensions.Logging;

namespace WebCantabra1.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            try
            {
                // Primero verificamos si hay pedidos sin incluir relaciones
                var orderCount = await _context.Orders.CountAsync();
                _logger.LogInformation($"Número total de pedidos en la base de datos: {orderCount}");

                // Ahora intentamos cargar los pedidos con sus relaciones
                Orders = await _context.Orders
                    .AsNoTracking() // Mejora el rendimiento para consultas de solo lectura
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                _logger.LogInformation($"Pedidos cargados con relaciones: {Orders.Count}");

                // Verificamos si hay algún pedido sin cliente asociado
                var ordersWithoutCustomer = Orders.Count(o => o.Customer == null);
                if (ordersWithoutCustomer > 0)
                {
                    _logger.LogWarning($"Hay {ordersWithoutCustomer} pedidos sin cliente asociado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar los pedidos: {ex.Message}");
                ModelState.AddModelError(string.Empty, 
                    "Error al cargar los pedidos. Por favor, contacte al administrador.");
            }
        }
    }
}