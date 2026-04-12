using IMS.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IMS.Data
{
    public static class AuthDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
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

            var (hash, salt) = PasswordHasher.HashPassword("admin123");

            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO adminusers (username, passwordhash, passwordsalt, isactive, createdat)
                VALUES (@username, @passwordhash, @passwordsalt, @isactive, @createdat)
                ON CONFLICT (username) DO NOTHING;
                """,
                new NpgsqlParameter("@username", "admin"),
                new NpgsqlParameter("@passwordhash", hash),
                new NpgsqlParameter("@passwordsalt", salt),
                new NpgsqlParameter("@isactive", true),
                new NpgsqlParameter("@createdat", DateTime.Now));
        }
    }
}
