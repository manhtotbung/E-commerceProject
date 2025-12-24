using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBH.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }   
        public string? ProductName { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }

        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; }
    }
}
