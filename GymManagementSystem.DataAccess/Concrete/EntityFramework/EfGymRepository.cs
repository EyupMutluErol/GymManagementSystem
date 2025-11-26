using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfGymRepository:EfGenericRepository<Gym>,IGymRepository
{
    public EfGymRepository(GymContext context) : base(context)
    {
    }
}
