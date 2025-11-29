using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.DataAccess.Abstract;

public interface IAppointmentRepository:IGenericRepository<Appointment>
{
    List<Appointment> GetAppointmentsByMember(int memberId);
    List<Appointment> GetAppointmentsByTrainer(int trainerId);
    List<Appointment> GetAllAppointmentsWithDetails();
}
