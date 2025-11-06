using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty; // Khởi tạo
        [Required(ErrorMessage = "Vui lòng nhập Tác giả")]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty; // Khởi tạo

        [Required]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // URL ảnh bìa

        // Khóa ngoại cho Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // Báo cho compiler là EF sẽ lo
    }
}