using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfServiceRepository:EfGenericRepository<Service>,IServiceRepository
{
    public EfServiceRepository(GymContext context) : base(context)
    {
    }
}
