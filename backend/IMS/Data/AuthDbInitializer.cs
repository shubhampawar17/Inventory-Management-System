using IMS.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IMS.Data
{
    public static class AuthDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

            await context.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS adminusers (
                    adminuserid integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                    username text NOT NULL UNIQUE,
                    passwordhash text NOT NULL,
                    passwordsalt text NOT NULL,
                    isactive boolean NOT NULL DEFAULT true,
                    createdat timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
                );
                """);

            /* 
            if (!ShouldSeedDefaultAdmin(configuration, environment))
            {
                return;
            }
            */
            Console.WriteLine("[DB] Forcing admin seeding (bypassing check for troubleshooting)...");

            var username = configuration["AdminSeed:Username"] ?? "admin";
            var password = configuration["AdminSeed:Password"] ?? "admin123";
            var (hash, salt) = PasswordHasher.HashPassword(password);

            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO adminusers (username, passwordhash, passwordsalt, isactive, createdat)
                VALUES (@username, @passwordhash, @passwordsalt, @isactive, @createdat)
                ON CONFLICT (username) 
                DO UPDATE SET 
                    passwordhash = EXCLUDED.passwordhash,
                    passwordsalt = EXCLUDED.passwordsalt,
                    isactive = EXCLUDED.isactive;
                """,
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@passwordhash", hash),
                new NpgsqlParameter("@passwordsalt", salt),
                new NpgsqlParameter("@isactive", true),
                new NpgsqlParameter("@createdat", DateTime.UtcNow));
            
            Console.WriteLine($"[DB] Admin user '{username}' seeded/updated successfully.");
        }

        private static bool ShouldSeedDefaultAdmin(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var configValue = configuration["AdminSeed:Enabled"];
            Console.WriteLine($"[DB] Configuration 'AdminSeed:Enabled' value: '{configValue}'");
            Console.WriteLine($"[DB] Environment: '{environment.EnvironmentName}'");

            if (bool.TryParse(configValue, out var enabled))
            {
                Console.WriteLine($"[DB] Parsed Enabled: {enabled}");
                return enabled;
            }

            var isDev = environment.IsDevelopment();
            Console.WriteLine($"[DB] Enabled not set or invalid. Falling back to IsDevelopment: {isDev}");
            return isDev;
        }
    }
}
