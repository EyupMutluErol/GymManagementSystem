using GymManagementSystem.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Entities.Concrete;

public class TrainerService:IEntity
{
    public int Id { get; set; }

    [Required]
    public int AppUserId { get; set; } // Trainer
    public AppUser AppUser { get; set; }

    [Required]
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}
