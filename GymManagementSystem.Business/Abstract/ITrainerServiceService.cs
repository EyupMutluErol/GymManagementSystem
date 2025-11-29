using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Abstract;

public interface ITrainerServiceService:IGenericService<TrainerService>
{
    List<AppUser> GetTrainersByServiceId(int serviceId);
}
