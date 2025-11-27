using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Ad")]
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    public string FirstName { get; set; }

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    public string LastName { get; set; }

    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "E-Posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }

    [Display(Name = "Şifre")]
    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Şifre Tekrar")]
    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    public string ConfirmPassword { get; set; }
}
