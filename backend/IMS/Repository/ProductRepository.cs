using IMS.Data;
using IMS.Exceptions;
using IMS.Models;
using Microsoft.EntityFrameworkCore;

namespace IMS.Repository
{
    public class ProductRepository
    {
        private readonly InventoryContext _context;

        public ProductRepository(InventoryContext context)
        {
            _context = context;
        }

        public void AddProduct(Product product)
        {
            if (_context.Products.Any(p => p.Name == product.Name))
            {
                throw new DuplicateProductException("Product with this name already exists.");
            }

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(int productId, string newName, string newDescription, double newPrice)
        {
            var product = GetProductById(productId);

            if (product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            if (_context.Products.Any(p => p.Name == newName && p.ProductId != productId))
            {
                throw new DuplicateProductException("Product with this name already exists.");
            }

            product.Name = newName;
            product.Description = newDescription;
            product.Price = newPrice;

            _context.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            var product = GetProductById(productId);

            if (product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            var transactionCount = _context.Transactions.Count(transaction => transaction.ProductId == productId);

            if (transactionCount > 0)
            {
                throw new ProductDeleteConflictException(
                    $"Cannot delete product '{product.Name}' because it still has {transactionCount} transaction{(transactionCount == 1 ? string.Empty : "s")}."
                );
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public Product? GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(product => product.ProductId == productId);
        }

        public Inventory? GetInventoryByProductId(int productId)
        {
            return _context.Products
                .AsNoTracking()
                .Where(product => product.ProductId == productId)
                .Select(product => product.Inventory)
                .FirstOrDefault();
        }

        public Product? GetProductByName(string productName)
        {
            return _context.Products.FirstOrDefault(product => product.Name == productName);
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public void AddStock(int productId, int quantity, int inventoryId)
        {
            var product = GetProductById(productId);

            if (product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            product.Quantity += quantity;

            var transaction = new Transaction
            {
                ProductId = productId,
                Quantity = quantity,
                Type = "Add",
                Date = DateTime.Now,
                InventoryId = inventoryId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public void RemoveStock(int productId, int quantity, int inventoryId)
        {
            var product = GetProductById(productId);

            if (product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            if (product.Quantity < quantity)
            {
                throw new InsufficientStockException("Insufficient stock to remove the requested quantity.");
            }

            product.Quantity -= quantity;

            var transaction = new Transaction
            {
                ProductId = productId,
                Quantity = quantity,
                Type = "Remove",
                Date = DateTime.Now,
                InventoryId = inventoryId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}
