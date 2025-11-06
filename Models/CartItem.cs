namespace WebBanSach.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty; // Khởi tạo
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }

        public decimal Total
        {
            get { return Price * Quantity; }
        }

        // Constructor để dễ dàng tạo từ Book
        public CartItem(Book book)
        {
            BookId = book.Id;
            Title = book.Title;
            Price = book.Price;
            ImageUrl = book.ImageUrl;
            Quantity = 1;
        }

        public CartItem()
        {
        }
    }
}