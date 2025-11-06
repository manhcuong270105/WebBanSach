using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Khởi tạo

        // Một Category có nhiều Book
        public List<Book> Books { get; set; } = new List<Book>();
    }
}