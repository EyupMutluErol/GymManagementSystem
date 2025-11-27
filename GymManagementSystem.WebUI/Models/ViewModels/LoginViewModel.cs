using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class LoginViewModel
{
    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz.")]
    [EmailAddress]
    public string Email { get; set; }

    [Display(Name = "Şifre")]
    [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
