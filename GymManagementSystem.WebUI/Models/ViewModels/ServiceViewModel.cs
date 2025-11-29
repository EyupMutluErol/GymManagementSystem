using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class ServiceViewModel
{
    public int Id { get; set; }

    [Display(Name = "Hizmet Adı")]
    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    public string Name { get; set; }

    [Display(Name = "Süre (Dakika)")]
    [Required(ErrorMessage = "Süre zorunludur.")]
    [Range(10, 240, ErrorMessage = "Lütfen 10 ile 240 dakika arasında geçerli bir süre giriniz.")]
    public int Duration { get; set; }

    [Display(Name = "Ücret")]
    [Required(ErrorMessage = "Ücret zorunludur.")]
    [Range(0, 100000, ErrorMessage = "Lütfen 0 ile 1000 arasında geçerli bir ücret giriniz.")]
    public decimal Price { get; set; }

    [Display(Name = "Salon")]
    [Required(ErrorMessage = "Lütfen bir salon seçiniz.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lütfen listeden bir salon seçiniz.")]
    public int GymId { get; set; }

    public string? GymName { get; set; }
}
