using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; } = string.Empty; // "Add" or "Remove"
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int InventoryId { get; set; }

        public Product? Product { get; set; }
        public Inventory? Inventory { get; set; }
    }
}
