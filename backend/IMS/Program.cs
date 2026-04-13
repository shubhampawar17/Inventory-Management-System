using IMS.Data;
using IMS.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:4200"];

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

await AuthDbInitializer.InitializeAsync(app.Services, app.Configuration, app.Environment);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AngularClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
