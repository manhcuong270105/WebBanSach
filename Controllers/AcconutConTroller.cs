using Microsoft.AspNetCore.Mvc;
using WebBanSach.Models;
using WebBanSach.Data; // Thêm
using System.Linq; // Thêm
using BCrypt.Net; // Thêm

namespace WebBanSach.Controllers
{
    public class AccountController : Controller
    {
        // =============================================
        // === KẾT NỐI DATABASE ===
        // =============================================
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        // === PHẦN ĐĂNG NHẬP (GET) ===
        // =============================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // =============================================
        // === PHẦN ĐĂNG NHẬP (POST) - NÂNG CẤP ===
        // =============================================
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Bước 1: Tìm người dùng trong Database bằng Email (Username)
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Username);

                // Bước 2: Kiểm tra User tồn tại VÀ Mật khẩu (đã mã hóa) có khớp không
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // ĐĂNG NHẬP THÀNH CÔNG

                    // Bước 3: Lưu thông tin thật vào Session
                    HttpContext.Session.SetString("Username", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Bước 4: Chuyển hướng dựa trên vai trò
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Nếu sai mật khẩu hoặc không tìm thấy user
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // Nếu nhập sai (ví dụ: bỏ trống), ở lại trang Login
            return View(model);
        }

        // =============================================
        // === PHẦN ĐĂNG KÝ (GET) ===
        // =============================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // =============================================
        // === PHẦN ĐĂNG KÝ (POST) - NÂNG CẤP ===
        // =============================================
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Email đã tồn tại trong Database chưa
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng.");
                    return View(model);
                }

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Tạo User mới
                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Role = "User" // Mặc định tất cả là "User"
                };

                // Kiểm tra tài khoản admin đặc biệt (bạn có thể đổi email này)
                if (model.Email.ToLower() == "admin@gmail.com")
                {
                    user.Role = "Admin";
                }

                // Lưu User mới vào Database
                _context.Users.Add(user);
                _context.SaveChanges();

                // Chuyển về trang Đăng nhập để họ đăng nhập
                return RedirectToAction("Login");
            }

            // Nếu model không hợp lệ (ví dụ: mật khẩu không khớp), ở lại trang
            return View(model);
        }

        // =============================================
        // === PHẦN ĐĂNG XUẤT (GET) ===
        // =============================================
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa toàn bộ Session (Username và UserRole)
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

