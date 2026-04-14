using IMS.Data;
using IMS.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var corsSection = builder.Configuration.GetSection("Cors:AllowedOrigins");
var allowedOrigins = corsSection.Get<string[]>() ?? new string[] { "http://localhost:4200" };

// Ensure origins don't have trailing slashes, as it can cause CORS to fail
allowedOrigins = allowedOrigins.Select(o => o.TrimEnd('/')).ToArray();

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
