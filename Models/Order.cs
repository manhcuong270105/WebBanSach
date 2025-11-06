using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Models
{
    public class Order
    {
        public int Id { get; set; }

        // public string? UserId { get; set; }
        // public User User { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string CustomerName { get; set; } = string.Empty; // Khởi tạo

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; } = string.Empty; // Khởi tạo

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = string.Empty; // Khởi tạo

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Khởi tạo

        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Khởi tạo List
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}