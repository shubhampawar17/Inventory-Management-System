using IMS.Contracts;
using IMS.Exceptions;
using IMS.Models;
using IMS.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly SupplierRepository _supplierRepository;

    public SuppliersController(SupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SupplierResponse>> GetAll()
    {
        var suppliers = _supplierRepository.GetAllSuppliers()
            .Select(supplier => new SupplierResponse(
                supplier.SupplierId,
                supplier.Name,
                supplier.ContactInformation,
                supplier.InventoryId));

        return Ok(suppliers);
    }

    [HttpGet("{id:int}")]
    public ActionResult<SupplierResponse> GetById(int id)
    {
        var supplier = _supplierRepository.GetSupplierById(id);
        if (supplier == null)
        {
            return NotFound();
        }

        return Ok(new SupplierResponse(
            supplier.SupplierId,
            supplier.Name,
            supplier.ContactInformation,
            supplier.InventoryId));
    }

    [HttpPost]
    public ActionResult<SupplierResponse> Create(SupplierRequest request)
    {
        try
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                ContactInformation = request.ContactInformation,
                InventoryId = request.InventoryId
            };

            _supplierRepository.AddSupplier(supplier);

            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, new SupplierResponse(
                supplier.SupplierId,
                supplier.Name,
                supplier.ContactInformation,
                supplier.InventoryId));
        }
        catch (DuplicateSupplierException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, SupplierRequest request)
    {
        var supplier = _supplierRepository.GetSupplierById(id);
        if (supplier == null)
        {
            return NotFound();
        }

        supplier.InventoryId = request.InventoryId;

        try
        {
            _supplierRepository.UpdateSupplier(id, request.Name, request.ContactInformation);
            return NoContent();
        }
        catch (DuplicateSupplierException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (SupplierNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _supplierRepository.DeleteSupplier(id);
            return NoContent();
        }
        catch (SupplierNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
