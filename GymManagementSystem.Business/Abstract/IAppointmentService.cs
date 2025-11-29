using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Abstract;

public interface IAppointmentService:IGenericService<Appointment>
{
    List<string> GetAvailableSlots(int trainerId, int serviceId, DateTime date);
    List<Appointment> GetAppointmentsByMember(int memberId);
    List<Appointment> GetAppointmentsByTrainer(int trainerId);
    List<Appointment> GetAllAppointmentsWithDetails();
}
