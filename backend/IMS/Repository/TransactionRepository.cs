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
    public class TransactionRepository
    {
        private readonly InventoryContext _context;

        public TransactionRepository(InventoryContext context)
        {
            _context = context;
        }

        // Add a transaction record
        public void AddTransaction(Transaction transaction)
        {
            // Check if InventoryId exists in Inventories table
            bool inventoryExists = _context.Inventories.Any(i => i.InventoryId == transaction.InventoryId);
            if (!inventoryExists)
            {
                Console.WriteLine($"Error: Inventory with ID {transaction.InventoryId} does not exist.");
                return;  // Skip saving if InventoryId is invalid
            }

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        //public void AddTransaction(Transaction transaction)
        //{
        //    _context.Transactions.Add(transaction);
        //    _context.SaveChanges();
        //}

        // Retrieve transaction history by product ID
        public List<Transaction> GetTransactionsByProductId(int productId)
        {
            var transactions = _context.Transactions.Where(t => t.ProductId == productId).ToList();

            if (transactions.Count == 0)
            {
                throw new TransactionNotFoundException("No transactions found for the given product ID.");
            }

            return transactions;
        }

        // Retrieve all transactions
        public List<Transaction> GetAllTransactions()
        {
            return _context.Transactions.ToList();
        }
    }
}
