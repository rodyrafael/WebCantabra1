using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebCantabra1.Data;
using WebCantabra1.Models;

namespace WebCantabra1.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TodayOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PopularProducts { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            var today = DateTime.UtcNow.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Convertir las fechas a UTC
            TodayOrders = await _context.Orders
                .CountAsync(o => o.OrderDate.Date == today);

            PendingOrders = await _context.Orders
                .CountAsync(o => o.Status == "Pendiente");

            MonthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate >= firstDayOfMonth)
                .SumAsync(o => o.TotalAmount);

            PopularProducts = await _context.Products
                .Include(p => p.OrderItems)
                .Where(p => p.OrderItems.Count > 5)
                .CountAsync();

            RecentOrders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
        }
    }
}