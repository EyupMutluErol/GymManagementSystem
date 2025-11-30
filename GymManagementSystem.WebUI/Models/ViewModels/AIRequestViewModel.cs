using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class AIRequestViewModel
{
    [Display(Name = "Yaşınız")]
    [Required(ErrorMessage = "Yaş bilgisi gereklidir.")]
    public int Age { get; set; }

    [Display(Name = "Kilonuz (kg)")]
    [Required(ErrorMessage = "Kilo bilgisi gereklidir.")]
    public double Weight { get; set; }

    [Display(Name = "Boyunuz (cm)")]
    [Required(ErrorMessage = "Boy bilgisi gereklidir.")]
    public double Height { get; set; }

    [Display(Name = "Cinsiyet")]
    public string Gender { get; set; } // "Erkek", "Kadın"

    [Display(Name = "Hedefiniz")]
    public string Goal { get; set; } // "Kilo Vermek", "Kas Yapmak", "Form Korumak"

    [Display(Name = "Aktivite Düzeyi")]
    public string ActivityLevel { get; set; } // "Hareketsiz", "Orta", "Aktif"

    // AI'dan gelen cevabı göstermek için
    public string? AIResponse { get; set; }
}
