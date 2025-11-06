using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSach.Data;
using WebBanSach.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; // Cần thiết cho IWebHostEnvironment
using System.IO; // Cần thiết cho thao tác file

namespace WebBanSach.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // #1: Thêm biến môi trường

        // #2: Sửa Constructor để nhận IWebHostEnvironment
        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Gán giá trị
        }

        // Hàm kiểm tra Admin (giữ nguyên)
        private bool CheckAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // =============================================
        // === QUẢN LÝ SẢN PHẨM (NÂNG CẤP ASYNC) ===
        // =============================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            // #3: Truyền danh mục (Categories) để hiển thị dropdown trong form AddBook
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var allBooks = await _context.Books.ToListAsync();
            return View(allBooks);
        }

        // [POST] Xử lý việc Thêm sách mới
        // #4: Sửa signature để nhận Book và IFormFile (tên ImageFile phải khớp với Form trong View)
        [HttpPost]
        public async Task<IActionResult> AddBook(Book book, IFormFile ImageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Kiểm tra xem form gửi lên có hợp lệ không (Title, Author, Price, CategoryId)
            if (ModelState.IsValid)
            {
                // === XỬ LÝ UPLOAD FILE ẢNH ===
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Lấy đường dẫn thư mục wwwroot/images/books
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Tạo tên file duy nhất (Guid.NewGuid() để đảm bảo tên file không trùng)
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Lưu file vào thư mục
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật ImageUrl cho đối tượng Book (đường dẫn tương đối)
                    book.ImageUrl = "/images/books/" + uniqueFileName;
                }
                else
                {
                    // Nếu không có ảnh, dùng ảnh placeholder
                    book.ImageUrl = "/images/books/book-placeholder.png";
                }
                // === KẾT THÚC XỬ LÝ UPLOAD FILE ẢNH ===

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu form không hợp lệ, tải lại Categories và Books để trả về View
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var allBooks = await _context.Books.ToListAsync();
            return View("Index", allBooks);
        }

        // [POST] Xử lý việc Xóa sách (giữ nguyên)
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var bookToDelete = await _context.Books.FindAsync(id);

            if (bookToDelete != null)
            {
                // Tùy chọn: Thêm logic xóa file ảnh cũ khỏi wwwroot tại đây (chưa có)

                _context.Books.Remove(bookToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // =============================================
        // === QUẢN LÝ NGƯỜI DÙNG (CODE MỚI) ===
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // =============================================
        // === QUẢN LÝ ĐƠN HÀNG (CODE MỚI THÊM) ===
        // =============================================
        [HttpGet]
        public async Task<IActionResult> ManageOrders()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Lấy tất cả đơn hàng, sắp xếp đơn mới nhất lên đầu
            // Include(o => o.OrderDetails) để tải chi tiết đơn hàng (để đếm số lượng)
            var orders = await _context.Orders
                                    .Include(o => o.OrderDetails)
                                    .OrderByDescending(o => o.OrderDate)
                                    .ToListAsync();

            return View(orders);
        }

        // === THÊM MỚI: Trang Chi tiết Đơn hàng ===
        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Tìm đơn hàng theo Id, bao gồm cả OrderDetails VÀ thông tin Book
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Tải chi tiết đơn hàng
                    .ThenInclude(od => od.Book) // Từ chi tiết, tải thông tin Sách
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(); // Không tìm thấy đơn hàng
            }

            return View(order);
        }
    }
}