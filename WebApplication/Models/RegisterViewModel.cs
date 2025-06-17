using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [Display(Name = "Email")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(255, ErrorMessage = "Пароль должен содержать минимум {2} символов.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 