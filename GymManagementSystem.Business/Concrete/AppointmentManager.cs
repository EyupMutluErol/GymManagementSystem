using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class AppointmentManager:GenericManager<Appointment>,IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentManager(IAppointmentRepository appointmentRepository) : base(appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
}
