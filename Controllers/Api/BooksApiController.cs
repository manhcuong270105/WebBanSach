using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSach.Data;
using WebBanSach.Models;

namespace WebBanSach.Controllers.Api
{
    [Route("api/[controller]")] // Đường dẫn: /api/BooksApi
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/BooksApi?filter=tên_sách
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string? filter)
        {
            var query = _context.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                // Chức năng Filtering: Tìm theo tên sách hoặc tác giả
                var lowerFilter = filter.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowerFilter) ||
                    b.Author.ToLower().Contains(lowerFilter)
                );
            }

            return await query.ToListAsync();
        }
    }
}