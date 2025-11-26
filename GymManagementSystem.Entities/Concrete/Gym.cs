using GymManagementSystem.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Entities.Concrete;

public class Gym:IEntity
{
    public int Id { get; set; }

    [Display(Name = "Salon Adı")]
    [Required(ErrorMessage = "Salon adı zorunludur.")]
    [StringLength(100, ErrorMessage = "Salon adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; }

    [Display(Name = "Adres")]
    [Required(ErrorMessage = "Adres bilgisi zorunludur.")]
    [StringLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir.")]
    public string Address { get; set; }

    [Display(Name = "Açılış Saati")]
    [Required(ErrorMessage = "Açılış saati zorunludur.")]
    [DataType(DataType.Time)]
    public TimeSpan OpenTime { get; set; }

    [Display(Name = "Kapanış Saati")]
    [Required(ErrorMessage = "Kapanış saati zorunludur.")]
    [DataType(DataType.Time)]
    public TimeSpan CloseTime { get; set; }

    public List<AppUser> Trainers { get; set; }
    public List<Service> Services { get; set; }
}
