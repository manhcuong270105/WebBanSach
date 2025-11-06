namespace WebBanSach.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; // Báo cho compiler là EF sẽ lo

        public int BookId { get; set; }
        public Book Book { get; set; } = null!; // Báo cho compiler là EF sẽ lo

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Lưu lại giá tại thời điểm mua
    }
}