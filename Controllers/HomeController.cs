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
            var allBooks = await _context.Books
                                .AsNoTracking()
                                .OrderByDescending(b => b.Id)
                                .ToListAsync();
            return View(allBooks);
        }

        // === ACTION TÌM KIẾM (ĐÃ GIỮ LẠI) ===
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            // 1. Lưu từ khóa tìm kiếm để hiển thị lại trên View
            ViewData["SearchQuery"] = query;

            var queryableBooks = _context.Books.AsQueryable();

            // 2. Kiểm tra nếu query không rỗng
            if (!string.IsNullOrEmpty(query))
            {
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

            // 4. Trả về View "Search.cshtml"
            return View(books);
        }

        // === 8 ACTION MỚI CHO SÁCH, BANNER VÀ ICON ===

        // 1. ACTION CHO TRANG CHI TIẾT SÁCH
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // 2. ACTION CHO BANNER "SALE 11.11"
        public IActionResult Sale()
        {
            return View();
        }

        // 3. ACTION CHO BANNER SÁCH MỚI (TRONG CAROUSEL)
        public IActionResult BannerSachMoi()
        {
            return View();
        }

        // 4. ACTION CHO BANNER SIDEBAR 1 (CHIẾN THẦN NGỮ VĂN)
        public IActionResult BannerNguVan()
        {
            return View();
        }

        // 5. ACTION CHO BANNER SIDEBAR 2 (OXFORD)
        public IActionResult BannerOxford()
        {
            return View();
        }

        // 6. ACTION CHO ICON FLASH SALE
        public IActionResult FlashSale()
        {
            return View();
        }

        // 7. ACTION CHO ICON MANGA
        public IActionResult Manga()
        {
            return View();
        }

        // 8. ACTION CHO ICON QUÀ TẶNG
        public IActionResult QuaTang()
        {
            return View();
        }

        // === CÁC ACTION CÓ SẴN KHÁC ===
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