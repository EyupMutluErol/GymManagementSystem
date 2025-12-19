using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class AppointmentManager:GenericManager<Appointment>,IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppUserRepository _appUserRepository;
    private readonly IServiceRepository _serviceRepository;

    public AppointmentManager(IAppointmentRepository appointmentRepository, IAppUserRepository appUserRepository,
                                  IServiceRepository serviceRepository) : base(appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
        _appUserRepository = appUserRepository;
        _serviceRepository = serviceRepository;
    }

    public List<string> GetAvailableSlots(int trainerId, int serviceId, DateTime date)
    {
        var slots = new List<string>();

        if (date.Date < DateTime.Now.Date)
        {
            return slots;
        }

        var trainer = _appUserRepository.GetById(trainerId);
        var service = _serviceRepository.GetById(serviceId);

        var existingAppointments = _appointmentRepository.GetListByFilter(x => x.TrainerId == trainerId && x.Date.Date == date.Date);

        if (trainer == null || service == null || trainer.ShiftStart == null || trainer.ShiftEnd == null)
            return slots;


        TimeSpan currentSlot = trainer.ShiftStart.Value;
        TimeSpan shiftEnd = trainer.ShiftEnd.Value;
        int duration = service.Duration;

        while (currentSlot.Add(TimeSpan.FromMinutes(duration)) <= shiftEnd)
        {
            TimeSpan slotEnd = currentSlot.Add(TimeSpan.FromMinutes(duration));
            bool isAvailable = true;

            if (date.Date == DateTime.Now.Date)
            {
                // Eğer slotun başlama saati, şu anki saatten küçükse (geçmişse)
                // Bu saati listeye eklemeden bir sonraki tura geçiyoruz.
                if (currentSlot < DateTime.Now.TimeOfDay)
                {
                    currentSlot = currentSlot.Add(TimeSpan.FromMinutes(duration));
                    continue;
                }
            }

            foreach (var app in existingAppointments)
            {
                if ((currentSlot < app.EndTime) && (slotEnd > app.StartTime))
                {
                    isAvailable = false;
                    break;
                }
            }

            if (isAvailable)
            {
                slots.Add(currentSlot.ToString(@"hh\:mm"));
            }

            currentSlot = currentSlot.Add(TimeSpan.FromMinutes(duration));
        }

        return slots;
    }
    public List<Appointment> GetAppointmentsByMember(int memberId)
    {
        return _appointmentRepository.GetAppointmentsByMember(memberId);
    }
    public List<Appointment> GetAppointmentsByTrainer(int trainerId)
    {
        return _appointmentRepository.GetAppointmentsByTrainer(trainerId);
    }
    public List<Appointment> GetAllAppointmentsWithDetails()
    {
        return _appointmentRepository.GetAllAppointmentsWithDetails();
    }
}
