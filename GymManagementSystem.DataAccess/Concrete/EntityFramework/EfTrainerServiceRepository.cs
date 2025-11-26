using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfTrainerServiceRepository:EfGenericRepository<TrainerService>,ITrainerServiceRepository
{
    public EfTrainerServiceRepository(GymContext context) : base(context)
    {
    }
}
