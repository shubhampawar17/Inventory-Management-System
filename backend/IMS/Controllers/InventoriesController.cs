using IMS.Contracts;
using IMS.Data;
using IMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoriesController : ControllerBase
{
    private readonly InventoryContext _context;

    public InventoriesController(InventoryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryResponse>>> GetAll()
    {
        var inventories = await _context.Inventories
            .OrderBy(inventory => inventory.InventoryId)
            .Select(inventory => new InventoryResponse(inventory.InventoryId, inventory.Location))
            .ToListAsync();

        return Ok(inventories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryResponse>> GetById(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        return Ok(new InventoryResponse(inventory.InventoryId, inventory.Location));
    }

    [HttpPost]
    public async Task<ActionResult<InventoryResponse>> Create(InventoryRequest request)
    {
        var inventory = new Inventory
        {
            Location = request.Location
        };

        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = inventory.InventoryId }, new InventoryResponse(
            inventory.InventoryId,
            inventory.Location));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, InventoryRequest request)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return NotFound(new { message = "Inventory not found." });
        }

        inventory.Location = request.Location;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return NotFound(new { message = "Inventory not found." });
        }

        var productCount = await _context.Products.CountAsync(product => product.InventoryId == id);
        var supplierCount = await _context.Suppliers.CountAsync(supplier => supplier.InventoryId == id);
        var transactionCount = await _context.Transactions.CountAsync(transaction => transaction.InventoryId == id);

        if (productCount > 0 || supplierCount > 0 || transactionCount > 0)
        {
            return Conflict(new
            {
                message = $"Cannot delete inventory '{inventory.Location}' because it still has {productCount} products, {supplierCount} suppliers, and {transactionCount} transactions."
            });
        }

        _context.Inventories.Remove(inventory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
