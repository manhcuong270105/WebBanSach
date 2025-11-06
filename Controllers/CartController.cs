using Microsoft.AspNetCore.Mvc;
using WebBanSach.Data;
using WebBanSach.Models;
using Newtonsoft.Json;
using System.Linq; // Cần thêm

namespace WebBanSach.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CART_KEY = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === THAY ĐỔI: Dùng List<CartItem> ===
        private List<CartItem> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString(CART_KEY);
            if (sessionCart == null)
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(sessionCart) ?? new List<CartItem>();
        }

        // === THAY ĐỔI: Dùng List<CartItem> ===
        private void SaveCartItems(List<CartItem> cartItems)
        {
            var sessionCart = JsonConvert.SerializeObject(cartItems);
            HttpContext.Session.SetString(CART_KEY, sessionCart);
        }

        // Trang xem giỏ hàng
        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            ViewBag.TotalAmount = cartItems.Sum(item => item.Total);
            return View(cartItems); // Truyền List<CartItem> sang View
        }

        // === THAY ĐỔI: Logic thêm số lượng ===
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.BookId == id);

            if (cartItem != null)
            {
                // Nếu đã có, tăng số lượng
                cartItem.Quantity++;
            }
            else
            {
                // Nếu chưa có, thêm mới
                cartItem = new CartItem(book);
                cart.Add(cartItem);
            }

            SaveCartItems(cart);
            return RedirectToAction("Index"); // Về trang giỏ hàng
        }

        // === THAY ĐỔI: Xóa theo BookId ===
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.BookId == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            SaveCartItems(cart);
            return RedirectToAction("Index");
        }

        // === THÊM MỚI: Action hiển thị form Checkout ===
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // Key của session vẫn là "Username" (dựa theo _Layout.cshtml)
            // nhưng giá trị của nó lưu Email
            var userEmail = HttpContext.Session.GetString("Username");
            if (userEmail != null)
            {
                // Sửa u.Username thành u.Email
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    var order = new Order
                    {
                        // Sửa user.Username thành user.Email
                        CustomerName = user.Email,
                        Email = user.Email,
                        Phone = ""
                    };
                    return View(order);
                }
            }

            return View(new Order());
        }

        // === THÊM MỚI: Action xử lý đặt hàng ===
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang rỗng!");
            }

            if (ModelState.IsValid)
            {
                // 1. Lưu thông tin Order
                order.TotalAmount = cart.Sum(item => item.Total);
                order.OrderDate = DateTime.Now;
                order.OrderDetails = new List<OrderDetail>();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu để lấy Order.Id

                // 2. Lưu chi tiết Order (OrderDetail)
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        Price = item.Price // Lưu giá tại thời điểm mua
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();

                // 3. Xóa giỏ hàng
                SaveCartItems(new List<CartItem>());

                // 4. Chuyển đến trang Cảm ơn
                return RedirectToAction("OrderConfirmation");
            }

            return View(order); // Hiển thị lại form nếu có lỗi
        }

        // === THÊM MỚI: Trang xác nhận đặt hàng thành công ===
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}