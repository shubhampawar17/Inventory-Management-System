using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }
        public int ProductId { get; internal set; }
        public string Location { get; set; } = string.Empty;
        
        public List<Product> Products { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
        
    }
}
