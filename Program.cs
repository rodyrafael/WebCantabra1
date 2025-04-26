using Microsoft.EntityFrameworkCore;
using WebCantabra1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configurar la conexión a la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Verificar la conexión a la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Intenta conectar a la base de datos
        context.Database.CanConnect();
        Console.WriteLine("✅ Conexión exitosa a la base de datos PostgreSQL");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar con la base de datos: {ex.Message}");
        throw; // Relanza la excepción para que la aplicación no inicie si no hay conexión
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
