using IMS.Contracts;
using IMS.Exceptions;
using IMS.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionRepository _transactionRepository;
    private readonly ProductRepository _productRepository;

    public TransactionsController(TransactionRepository transactionRepository, ProductRepository productRepository)
    {
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TransactionResponse>> GetAll()
    {
        var transactions = _transactionRepository.GetAllTransactions()
            .Select(transaction => new TransactionResponse(
                transaction.TransactionId,
                transaction.ProductId,
                transaction.Type,
                transaction.Quantity,
                transaction.Date,
                transaction.InventoryId));

        return Ok(transactions);
    }

    [HttpGet("product/{productId:int}")]
    public ActionResult<IEnumerable<TransactionResponse>> GetByProduct(int productId)
    {
        try
        {
            var transactions = _transactionRepository.GetTransactionsByProductId(productId)
                .Select(transaction => new TransactionResponse(
                    transaction.TransactionId,
                    transaction.ProductId,
                    transaction.Type,
                    transaction.Quantity,
                    transaction.Date,
                    transaction.InventoryId));

            return Ok(transactions);
        }
        catch (TransactionNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public ActionResult<TransactionResponse> Create(TransactionRequest request)
    {
        var product = _productRepository.GetProductById(request.ProductId);
        if (product == null)
        {
            return NotFound(new { message = "Product not found." });
        }

        var inventory = _productRepository.GetInventoryByProductId(request.ProductId);
        if (inventory == null)
        {
            return BadRequest(new { message = "Inventory not found for the selected product." });
        }

        try
        {
            if (string.Equals(request.Type, "Add", StringComparison.OrdinalIgnoreCase))
            {
                _productRepository.AddStock(request.ProductId, request.Quantity, inventory.InventoryId);
            }
            else if (string.Equals(request.Type, "Remove", StringComparison.OrdinalIgnoreCase))
            {
                _productRepository.RemoveStock(request.ProductId, request.Quantity, inventory.InventoryId);
            }
            else
            {
                return BadRequest(new { message = "Type must be either 'Add' or 'Remove'." });
            }

            // More robust way to get the created transaction
            var latestTransaction = _transactionRepository.GetAllTransactions()
                .Where(t => t.ProductId == request.ProductId)
                .OrderByDescending(t => t.Date)
                .FirstOrDefault();

            if (latestTransaction == null)
            {
                return StatusCode(500, new { message = "Transaction was recorded but could not be retrieved." });
            }

            return CreatedAtAction(nameof(GetAll), new TransactionResponse(
                latestTransaction.TransactionId,
                latestTransaction.ProductId,
                latestTransaction.Type,
                latestTransaction.Quantity,
                latestTransaction.Date,
                latestTransaction.InventoryId));
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Transaction creation failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[ERROR] Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { message = "Internal server error occurred.", detail = ex.Message });
        }
    }
}
