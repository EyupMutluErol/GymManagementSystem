using GymManagementSystem.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Entities.Concrete;

public class Service:IEntity
{
    public int Id { get; set; }

    [Display(Name = "Hizmet Adı")]
    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Hizmet adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; }

    [Display(Name = "Süre (Dakika)")]
    [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
    [Range(10, 240, ErrorMessage = "Hizmet süresi 10 ile 240 dakika arasında olmalıdır.")]
    public int Duration { get; set; }

    [Display(Name = "Ücret")]
    [Required(ErrorMessage = "Ücret bilgisi zorunludur.")]
    [DataType(DataType.Currency)]
    [Range(0, 10000, ErrorMessage = "Ücret 0 ile 10.000 TL arasında olmalıdır.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // Bir hizmet mutlaka bir salona ait olmalıdır.
    [Required]
    public int GymId { get; set; }
    public Gym Gym { get; set; }

    public List<TrainerService> TrainerServices { get; set; }
}
