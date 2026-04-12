using IMS.Contracts;
using IMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private const int LowStockThreshold = 10;
    private readonly InventoryContext _context;

    public DashboardController(InventoryContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
    {
        var inventories = await _context.Inventories
            .AsNoTracking()
            .AsSplitQuery()
            .Include(inventory => inventory.Products)
            .Include(inventory => inventory.Suppliers)
            .ToListAsync();

        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();

        var transactions = await _context.Transactions
            .AsNoTracking()
            .OrderByDescending(transaction => transaction.Date)
            .ToListAsync();

        var inventoryLocations = inventories.ToDictionary(inventory => inventory.InventoryId, inventory => inventory.Location);
        var productNames = products.ToDictionary(product => product.ProductId, product => product.Name);
        var lowStockProducts = products
            .Where(product => product.Quantity <= LowStockThreshold)
            .OrderBy(product => product.Quantity)
            .ThenBy(product => product.Name)
            .Take(6)
            .Select(product => new LowStockProductResponse(
                product.ProductId,
                product.Name,
                inventoryLocations.GetValueOrDefault(product.InventoryId, $"Inventory #{product.InventoryId}"),
                product.InventoryId,
                product.Quantity))
            .ToList();

        var topProducts = products
            .OrderByDescending(product => product.Quantity * product.Price)
            .Take(5)
            .Select(product => new TopProductResponse(
                product.ProductId,
                product.Name,
                inventoryLocations.GetValueOrDefault(product.InventoryId, $"Inventory #{product.InventoryId}"),
                product.InventoryId,
                product.Quantity,
                product.Price,
                product.Quantity * product.Price))
            .ToList();

        var recentTransactions = transactions
            .Take(6)
            .Select(transaction => new RecentTransactionInsightResponse(
                transaction.TransactionId,
                transaction.ProductId,
                productNames.GetValueOrDefault(transaction.ProductId, $"Product #{transaction.ProductId}"),
                inventoryLocations.GetValueOrDefault(transaction.InventoryId, $"Inventory #{transaction.InventoryId}"),
                transaction.InventoryId,
                NormalizeTransactionType(transaction.Type),
                transaction.Quantity,
                transaction.Date))
            .ToList();

        var inventorySnapshots = inventories
            .OrderBy(inventory => inventory.InventoryId)
            .Select(inventory =>
            {
                var lowStockCount = inventory.Products.Count(product => product.Quantity <= LowStockThreshold);
                var healthLabel = inventory.Products.Count == 0
                    ? "Idle"
                    : lowStockCount == 0
                        ? "Stable"
                        : lowStockCount >= Math.Max(1, (int)Math.Ceiling(inventory.Products.Count / 2d))
                            ? "Attention"
                            : "Watch";

                return new InventorySnapshotResponse(
                    inventory.InventoryId,
                    inventory.Location,
                    inventory.Products.Count,
                    inventory.Suppliers.Count,
                    inventory.Products.Sum(product => product.Quantity),
                    inventory.Products.Sum(product => product.Quantity * product.Price),
                    healthLabel);
            })
            .ToList();

        var addTransactionCount = transactions.Count(transaction => IsAddTransaction(transaction.Type));
        var removeTransactionCount = transactions.Count(transaction => IsRemoveTransaction(transaction.Type));
        var supplierCoverage = inventories.Count == 0
            ? 0
            : (int)Math.Round(inventories.Count(inventory => inventory.Suppliers.Count > 0) * 100d / inventories.Count);
        var inStockRate = products.Count == 0
            ? 0
            : (int)Math.Round(products.Count(product => product.Quantity > LowStockThreshold) * 100d / products.Count);

        return Ok(new DashboardSummaryResponse(
            new DashboardKpiResponse(
                inventories.Count,
                products.Count,
                await _context.Suppliers.AsNoTracking().CountAsync(),
                transactions.Count,
                products.Sum(product => product.Quantity),
                products.Sum(product => product.Quantity * product.Price),
                inStockRate,
                supplierCoverage,
                addTransactionCount,
                removeTransactionCount,
                lowStockProducts.Count),
            inventorySnapshots,
            lowStockProducts,
            topProducts,
            recentTransactions));
    }

    private static string NormalizeTransactionType(string type) =>
        IsRemoveTransaction(type) ? "Remove" : "Add";

    private static bool IsAddTransaction(string type) =>
        type.Contains("add", StringComparison.OrdinalIgnoreCase);

    private static bool IsRemoveTransaction(string type) =>
        type.Contains("remove", StringComparison.OrdinalIgnoreCase);
}
