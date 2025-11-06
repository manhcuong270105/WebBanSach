using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanSach.Models;
using WebBanSach.Data; // Cần
using Microsoft.EntityFrameworkCore; // Cần
using System.Threading.Tasks; // Cần
using System.Linq; // Cần

namespace WebBanSach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action Index này sẽ gửi TẤT CẢ sách ra View
        public async Task<IActionResult> Index()
        {
            // Lấy TẤT CẢ sách (không phân trang)
            // Sắp xếp theo ID giảm dần (để sách mới nhất lên đầu)
            var allBooks = await _context.Books
                                .AsNoTracking()
                                .OrderByDescending(b => b.Id)
                                .ToListAsync();

            // Gửi toàn bộ danh sách ra View
            return View(allBooks);
        }
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            // 1. Lưu từ khóa tìm kiếm để hiển thị lại trên View
            ViewData["SearchQuery"] = query;

            var queryableBooks = _context.Books.AsQueryable();

            // 2. Kiểm tra nếu query không rỗng
            if (!string.IsNullOrEmpty(query))
            {
                // Logic tìm kiếm (tương tự file BooksApiController của bạn)
                var lowerQuery = query.ToLower();
                queryableBooks = queryableBooks.Where(b =>
                    b.Title.ToLower().Contains(lowerQuery) ||
                    b.Author.ToLower().Contains(lowerQuery)
                );
            }

            // 3. Lấy kết quả và gửi sang View
            var books = await queryableBooks
                            .AsNoTracking()
                            .OrderByDescending(b => b.Id)
                            .ToListAsync();

            // 4. Trả về View "Search.cshtml" (sẽ tạo ở Bước 2)
            return View(books);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

