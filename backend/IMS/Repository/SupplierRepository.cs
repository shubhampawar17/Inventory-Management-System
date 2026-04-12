using IMS.Data;
using IMS.Exceptions;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Repository
{
    public class SupplierRepository
    {
        private readonly InventoryContext _context;

        public SupplierRepository(InventoryContext context)
        {
            _context = context;
        }

        // Add a new supplier if no duplicate name exists
        public void AddSupplier(Supplier supplier)
        {
            if (_context.Suppliers.Any(s => s.Name == supplier.Name))
            {
                throw new DuplicateSupplierException("Supplier with this name already exists.");
            }
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }

        // Update an existing supplier's details with duplicate name check
        public void UpdateSupplier(int supplierId, string newName, string newContactInfo)
        {
            var supplier = GetSupplierById(supplierId);

            if (supplier == null)
            {
                throw new SupplierNotFoundException("Supplier not found.");
            }

            if (_context.Suppliers.Any(s => s.Name == newName && s.SupplierId != supplierId))
            {
                throw new DuplicateSupplierException("Supplier with this name already exists.");
            }

            supplier.Name = newName;
            supplier.ContactInformation = newContactInfo;

            _context.SaveChanges();
        }

        // Delete an existing supplier by ID
        public void DeleteSupplier(int supplierId)
        {
            var supplier = GetSupplierById(supplierId);

            if (supplier == null)
            {
                throw new SupplierNotFoundException("Supplier not found.");
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
        }

        // Retrieve supplier details by ID
        public Supplier? GetSupplierById(int supplierId)
        {
            return _context.Suppliers.Find(supplierId);
        }

        // Retrieve supplier details by name
        public Supplier? GetSupplierByName(string supplierName)
        {
            return _context.Suppliers.FirstOrDefault(s => s.Name == supplierName);
        }

        // Retrieve all suppliers
        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers.ToList();
        }
    }
}
