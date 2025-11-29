using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using GymManagementSystem.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfAppointmentRepository:EfGenericRepository<Appointment>,IAppointmentRepository
{
    public EfAppointmentRepository(GymContext context) : base(context)
    {
    }
    public List<Appointment> GetAppointmentsByMember(int memberId)
    {
        return _context.Appointments
                .Include(x => x.Service)       
                .ThenInclude(s => s.Gym)   
                .Include(x => x.Trainer)      
                .Where(x => x.MemberId == memberId)
                .OrderByDescending(x => x.Date)
                .ToList();
    }
    public List<Appointment> GetAppointmentsByTrainer(int trainerId)
    {
        return _context.Appointments
            .Include(x => x.Service)
            .Include(x => x.Member) 
            .Where(x => x.TrainerId == trainerId)
            .OrderBy(x => x.Date).ThenBy(x => x.StartTime) 
            .ToList();
    }
    public List<Appointment> GetAllAppointmentsWithDetails()
    {
        return _context.Appointments
            .Include(x => x.Member)
            .Include(x => x.Trainer)
            .Include(x => x.Service)
                .ThenInclude(s => s.Gym)
            .OrderByDescending(x => x.Date)
            .ToList();
    }
}
