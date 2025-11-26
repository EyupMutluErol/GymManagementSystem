using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class TrainerServiceManager:GenericManager<TrainerService>,ITrainerServiceService
{
    private readonly ITrainerServiceRepository _trainerServiceRepository;

    public TrainerServiceManager(ITrainerServiceRepository trainerServiceRepository) : base(trainerServiceRepository)
    {
        _trainerServiceRepository = trainerServiceRepository;
    }
}
