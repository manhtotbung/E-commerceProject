using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace WebBH.Models
{
    public class CheckOutVM
    {

        [Required]
        [Display(Name = "Full Name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PaymentMethod { get; set; } = "COD";
        public string? Note { get; set; }

        // NEW: gắn giỏ hàng vào luôn
        public List<CartItem> Items { get; set; } = new();
        public decimal Subtotal => Items.Sum(i => i.UnitPrice * i.Quantity);
        public decimal Total => Subtotal; // có thể + ship - discount sau
    }
}

