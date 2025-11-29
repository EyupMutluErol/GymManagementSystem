using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class TrainerServiceManager:GenericManager<TrainerService>,ITrainerServiceService
{
    private readonly ITrainerServiceRepository _trainerServiceRepository;
    private readonly IAppUserRepository _appUserRepository;

    public TrainerServiceManager(ITrainerServiceRepository trainerServiceRepository, IAppUserRepository appUserRepository) : base(trainerServiceRepository)
    {
        _trainerServiceRepository = trainerServiceRepository;
        _appUserRepository = appUserRepository;
    }

    public List<AppUser> GetTrainersByServiceId(int serviceId)
    {
        // 1. Bu hizmeti veren kayıtları bul
        var relations = _trainerServiceRepository.GetListByFilter(x => x.ServiceId == serviceId);

        // 2. O kayıtların Hoca ID'lerini al
        var trainerIds = relations.Select(x => x.AppUserId).ToList();

        // 3. Hoca bilgilerini getir
        var trainers = new List<AppUser>();
        foreach (var id in trainerIds)
        {
            var trainer = _appUserRepository.GetById(id);
            if (trainer != null)
            {
                trainers.Add(trainer);
            }
        }
        return trainers;
    }
}
