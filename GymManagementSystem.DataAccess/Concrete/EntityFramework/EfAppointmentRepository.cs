using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfAppointmentRepository:EfGenericRepository<Appointment>,IAppointmentRepository
{
    public EfAppointmentRepository(GymContext context) : base(context)
    {
    }
}
