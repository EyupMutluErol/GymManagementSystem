using GymManagementSystem.Entities.Abstract;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Entities.Concrete;

public class AppUser:IdentityUser<int>,IEntity
{
    [Display(Name = "Ad")]
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; }

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; }

    [Display(Name = "Profil Fotoğrafı")]
    public string? PhotoUrl { get; set; } 


    public int? GymId { get; set; }
    public Gym? Gym { get; set; }

    [Display(Name = "Mesai Başlangıç")]
    [DataType(DataType.Time)]
    public TimeSpan? ShiftStart { get; set; }

    [Display(Name = "Mesai Bitiş")]
    [DataType(DataType.Time)]
    public TimeSpan? ShiftEnd { get; set; }


    

    [Display(Name = "Doğum Tarihi")]
    [DataType(DataType.Date)] 
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Cinsiyet")]
    [StringLength(10)] // "Erkek" veya "Kadın" yazacağız.
    public string? Gender { get; set; }

    [Display(Name = "Kilo (kg)")]
    [Range(30, 200, ErrorMessage = "Geçerli bir kilo giriniz (30-200).")] // Mantıksız değerleri engeller
    public double? Weight { get; set; }

    [Display(Name = "Boy (cm)")]
    [Range(100, 250, ErrorMessage = "Geçerli bir boy giriniz (100-250).")] // Mantıksız değerleri engeller
    public double? Height { get; set; }

    public List<TrainerService> TrainerServices { get; set; }
}
