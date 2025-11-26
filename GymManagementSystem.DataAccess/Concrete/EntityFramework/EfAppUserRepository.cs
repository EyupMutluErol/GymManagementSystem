using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfAppUserRepository:EfGenericRepository<AppUser>,IAppUserRepository
{
    public EfAppUserRepository(GymContext context) : base(context)
    {
    }
}
