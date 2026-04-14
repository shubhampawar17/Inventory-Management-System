using IMS.Models;
using Microsoft.EntityFrameworkCore;

namespace IMS.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext()
        {
        }

        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(product => product.ProductId);
                entity.Property(product => product.ProductId).HasColumnName("productid");
                entity.Property(product => product.Name).HasColumnName("name");
                entity.Property(product => product.Description).HasColumnName("description");
                entity.Property(product => product.Quantity).HasColumnName("quantity");
                entity.Property(product => product.Price).HasColumnName("price");
                entity.Property(product => product.InventoryId).HasColumnName("inventoryid");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventories");
                entity.HasKey(inventory => inventory.InventoryId);
                entity.Property(inventory => inventory.InventoryId).HasColumnName("inventoryid");
                entity.Property(inventory => inventory.ProductId).HasColumnName("productid");
                entity.Property(inventory => inventory.Location).HasColumnName("location");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");
                entity.HasKey(supplier => supplier.SupplierId);
                entity.Property(supplier => supplier.SupplierId).HasColumnName("supplierid");
                entity.Property(supplier => supplier.Name).HasColumnName("name");
                entity.Property(supplier => supplier.ContactInformation).HasColumnName("contactinformation");
                entity.Property(supplier => supplier.InventoryId).HasColumnName("inventoryid");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");
                entity.HasKey(transaction => transaction.TransactionId);
                entity.Property(transaction => transaction.TransactionId).HasColumnName("transactionid");
                entity.Property(transaction => transaction.ProductId).HasColumnName("productid");
                entity.Property(transaction => transaction.Type).HasColumnName("type");
                entity.Property(transaction => transaction.Quantity).HasColumnName("quantity");
                entity.Property(transaction => transaction.Date)
                      .HasColumnName("date")
                      .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                entity.Property(transaction => transaction.InventoryId).HasColumnName("inventoryid");
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("adminusers");
                entity.HasKey(adminUser => adminUser.AdminUserId);
                entity.Property(adminUser => adminUser.AdminUserId).HasColumnName("adminuserid");
                entity.Property(adminUser => adminUser.Username).HasColumnName("username");
                entity.Property(adminUser => adminUser.PasswordHash).HasColumnName("passwordhash");
                entity.Property(adminUser => adminUser.PasswordSalt).HasColumnName("passwordsalt");
                entity.Property(adminUser => adminUser.IsActive).HasColumnName("isactive");
                entity.Property(adminUser => adminUser.CreatedAt)
                      .HasColumnName("createdat")
                      .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            });
        }
    }
}
