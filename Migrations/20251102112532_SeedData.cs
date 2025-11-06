using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanSach.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. THÊM DỮ LIỆU CATEGORY (PHẢI CHÈN TRƯỚC)
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Văn học" },
                    { 2, "Khoa học" }
                });

            // Định nghĩa dữ liệu sách (15 cuốn khác nhau)
            var seedBooks = new[]
            {
                new { Title = "Nhà Giả Kim", Author = "Paulo Coelho", Price = 79000M },
                new { Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", Price = 115000M },
                new { Title = "Cuộc đời lớn", Author = "Ha Min-jin", Price = 95500M },
                new { Title = "Tư duy nhanh và chậm", Author = "Daniel Kahneman", Price = 168000M },
                new { Title = "Cây Cam Ngọt Của Tôi", Author = "José Mauro de Vasconcelos", Price = 85000M },
                new { Title = "Hai Số Phận", Author = "Jeffrey Archer", Price = 120000M },
                new { Title = "Bảy Thói Quen Của Người Thành Đạt", Author = "Stephen Covey", Price = 150000M },
                new { Title = "Vật Lý Của Tương Lai", Author = "Michio Kaku", Price = 210000M },
                new { Title = "Suối Nguồn", Author = "Ayn Rand", Price = 250000M },
                new { Title = "Giết Con Chim Nhại", Author = "Harper Lee", Price = 105000M },
                new { Title = "Bên Kia Sông Rhine", Author = "Lê Thu", Price = 65000M },
                new { Title = "Hoàng Tử Bé", Author = "Antoine de Saint-Exupéry", Price = 55000M },
                new { Title = "Mật Mã Da Vinci", Author = "Dan Brown", Price = 110000M },
                new { Title = "Sức Mạnh Của Hiện Tại", Author = "Eckhart Tolle", Price = 99000M },
                new { Title = "1984", Author = "George Orwell", Price = 88000M },
            };

            // 2. THÊM DỮ LIỆU BOOK
            for (int i = 0; i < seedBooks.Length; i++)
            {
                // Sử dụng ảnh book_1.jpg đến book_15.jpg (Kiểm tra lại đuôi file trên máy của bạn)
                string imageUrl = $"/images/books/book_{i + 1}.jpg";

                migrationBuilder.InsertData(
                    table: "Books",
                    columns: new[] { "Id", "Title", "Author", "Price", "ImageUrl", "CategoryId" },
                    values: new object[]
                    {
                        i + 1, // ID bắt đầu từ 1
                        seedBooks[i].Title,
                        seedBooks[i].Author,
                        seedBooks[i].Price,
                        imageUrl,
                        1 // Gán cho CategoryId = 1
                    });
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa dữ liệu mẫu khi rollback
            for (int i = 1; i <= 15; i++)
            {
                migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: i);
            }
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValues: new object[] { 1, 2 });
        }
    }
}