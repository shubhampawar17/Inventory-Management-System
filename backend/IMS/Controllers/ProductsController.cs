using IMS.Contracts;
using IMS.Exceptions;
using IMS.Models;
using IMS.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _productRepository;

    public ProductsController(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductResponse>> GetAll()
    {
        var products = _productRepository.GetAllProducts()
            .Select(product => new ProductResponse(
                product.ProductId,
                product.Name,
                product.Description,
                product.Quantity,
                product.Price,
                product.InventoryId));

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public ActionResult<ProductResponse> GetById(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(new ProductResponse(
            product.ProductId,
            product.Name,
            product.Description,
            product.Quantity,
            product.Price,
            product.InventoryId));
    }

    [HttpPost]
    public ActionResult<ProductResponse> Create(ProductRequest request)
    {
        try
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Quantity = request.Quantity,
                Price = request.Price,
                InventoryId = request.InventoryId
            };

            _productRepository.AddProduct(product);

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new ProductResponse(
                product.ProductId,
                product.Name,
                product.Description,
                product.Quantity,
                product.Price,
                product.InventoryId));
        }
        catch (DuplicateProductException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, ProductRequest request)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        try
        {
            product.Quantity = request.Quantity;
            product.InventoryId = request.InventoryId;
            _productRepository.UpdateProduct(id, request.Name, request.Description, request.Price);
            return NoContent();
        }
        catch (DuplicateProductException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _productRepository.DeleteProduct(id);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProductDeleteConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
