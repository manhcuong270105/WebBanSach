using Microsoft.EntityFrameworkCore;
using WebBanSach.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm dịch vụ kết nối DB (dùng Sqlite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// === SỬA LỖI CS8604: Thêm kiểm tra null cho connectionString ===
if (string.IsNullOrEmpty(connectionString))
{
    // Ném ra lỗi rõ ràng nếu không tìm thấy chuỗi kết nối
    throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DefaultConnection' trong appsettings.json.");
}
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)); // Tại đây, connectionString đã được đảm bảo là không null

// (Tùy chọn) Thêm Identity để quản lý Đăng nhập/Đăng ký
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// 2. Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Thêm dịch vụ Controllers và Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// (Tùy chọn) Kích hoạt xác thực nếu dùng Identity
// app.UseAuthentication();
app.UseAuthorization();

// 4. Kích hoạt Session
app.UseSession();

// ===================================================
// === SỬA LỖI: ĐƯA "DEFAULT" LÊN TRƯỚC "API" ===
// ===================================================

// Định tuyến mặc định (PHẢI ĐƯỢC ƯU TIÊN TRƯỚC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Định tuyến cho API (ĐẶT SAU)
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Home}/{action=Index}/{id?}");

// ===================================================

// (Tùy chọn) Kích hoạt trang Razor Pages nếu dùng Identity
// app.MapRazorPages();

app.Run();