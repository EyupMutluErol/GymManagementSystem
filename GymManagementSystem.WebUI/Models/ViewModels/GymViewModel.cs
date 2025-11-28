using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class GymViewModel
{
    public int Id { get; set; }

    [Display(Name = "Salon Adı")]
    [Required(ErrorMessage = "Salon adı zorunludur.")]
    public string Name { get; set; }

    [Display(Name = "Adres")]
    [Required(ErrorMessage = "Adres bilgisi zorunludur.")]
    public string Address { get; set; }

    [Display(Name = "Açılış Saati")]
    [Required]
    [DataType(DataType.Time)]
    public TimeSpan OpenTime { get; set; }

    [Display(Name = "Kapanış Saati")]
    [Required]
    [DataType(DataType.Time)]
    public TimeSpan CloseTime { get; set; }
}
