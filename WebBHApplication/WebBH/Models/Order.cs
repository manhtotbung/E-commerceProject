using System.ComponentModel.DataAnnotations;
using WebBH.Areas.Data;

namespace WebBH.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Link tới user Identity
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

        // Snapshot thông tin giao nhận tại thời điểm đặt (không phụ thuộc hồ sơ user)
        public string FullName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Note { get; set; } = default!;


        // Thanh toán & trạng thái
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => Subtotal + ShippingFee - Discount;

        public string PaymentMethod { get; set; } = "COD";    // hoặc enum
        public string Status { get; set; } = "Pending";       // Pending/Paid/Shipped/Cancelled...

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
