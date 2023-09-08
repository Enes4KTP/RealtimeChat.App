using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.MVC.Dtos.RegisterDto
{
    public class CreateNewUserDto
    {
        [Required(ErrorMessage ="Ad girilmesi zorunludur.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Soyad girilmesi zorunludur.")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı girilmesi zorunludur.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Mail girilmesi zorunludur.")]
        public string? Mail { get; set; }

        [Required(ErrorMessage = "Şifre girilmesi zorunludur.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Şifre girilmesi zorunludur.")]
        [Compare("Password",ErrorMessage ="Şifreler uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }
    }
}
