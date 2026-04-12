using IMS.Data;
using IMS.Models;
using IMS.Security;
using Microsoft.EntityFrameworkCore;

namespace IMS.Repository
{
    public class AdminUserRepository
    {
        private readonly InventoryContext _context;

        public AdminUserRepository(InventoryContext context)
        {
            _context = context;
        }

        public async Task<AdminUser?> ValidateCredentialsAsync(string username, string password)
        {
            var normalizedUsername = username.Trim().ToLower();
            var user = await _context.AdminUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(adminUser => adminUser.Username.ToLower() == normalizedUsername && adminUser.IsActive);

            if (user == null)
            {
                return null;
            }

            return PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt) ? user : null;
        }
    }
}
