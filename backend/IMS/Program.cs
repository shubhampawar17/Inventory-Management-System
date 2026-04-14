using IMS.Data;
using IMS.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var corsSection = builder.Configuration.GetSection("Cors:AllowedOrigins");
var allowedOrigins = corsSection.Get<string[]>() ?? new string[] { "http://localhost:4200" };

// Ensure origins are strictly cleaned: remove newlines, carriage returns, trailing slashes, and spaces
allowedOrigins = allowedOrigins
    .Select(o => o.Replace("\r", "").Replace("\n", "").Trim().TrimEnd('/'))
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .ToArray();

Console.WriteLine($"[CORS] Allowed Origins (Cleaned): {string.Join(" | ", allowedOrigins)}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InventoryDb")));
builder.Services.AddScoped<AdminUserRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<SupplierRepository>();
builder.Services.AddScoped<TransactionRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

try 
{
    await AuthDbInitializer.InitializeAsync(app.Services, app.Configuration, app.Environment);
    Console.WriteLine("[DB] Database initialization successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"[DB] Error during database initialization: {ex.Message}");
    // Do not rethrow if you want the app to still start (so we can at least get CORS headers on error responses)
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AngularClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
