using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class GymManager:GenericManager<Gym>,IGymService
{
    private readonly IGymRepository _gymRepository;

    public GymManager(IGymRepository gymRepository) : base(gymRepository)
    {
        _gymRepository = gymRepository;
    }
}
