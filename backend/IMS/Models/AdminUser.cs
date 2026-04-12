using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class AdminUser
    {
        [Key]
        public int AdminUserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
