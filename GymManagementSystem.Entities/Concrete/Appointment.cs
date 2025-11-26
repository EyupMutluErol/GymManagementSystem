using GymManagementSystem.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Entities.Concrete;

public class Appointment:IEntity
{
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }
    public AppUser Member { get; set; }

    [Required]
    public int TrainerId { get; set; }
    public AppUser Trainer { get; set; }

    [Required]
    public int ServiceId { get; set; }
    public Service Service { get; set; }

    [Display(Name = "Randevu Tarihi")]
    [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Display(Name = "Başlangıç Saati")]
    [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Display(Name = "Bitiş Saati")]
    [Required] // Kullanıcı seçmez, sistem hesaplar ama veritabanı için zorunludur.
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; }
}

public enum AppointmentStatus
{
    [Display(Name = "Bekliyor")]
    Pending,

    [Display(Name = "Onaylandı")]
    Confirmed,

    [Display(Name = "İptal Edildi")]
    Cancelled
}
