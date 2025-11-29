using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.WebUI.Models.ViewModels;

public class MakeAppointmentViewModel
{
    [Display(Name = "Salon")]
    [Required(ErrorMessage = "Lütfen bir salon seçiniz.")]
    public int GymId { get; set; }

    [Display(Name = "Hizmet")]
    [Required(ErrorMessage = "Lütfen bir hizmet seçiniz.")]
    public int ServiceId { get; set; }

    [Display(Name = "Antrenör")]
    [Required(ErrorMessage = "Lütfen bir antrenör seçiniz.")]
    public int TrainerId { get; set; }

    [Display(Name = "Randevu Tarihi")]
    [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Display(Name = "Saat")]
    [Required(ErrorMessage = "Lütfen bir saat seçiniz.")]
    public string TimeSlot { get; set; } 
}
