using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string Username { get; set; } = string.Empty; // Khởi tạo

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Khởi tạo

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}