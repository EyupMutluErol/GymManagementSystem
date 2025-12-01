using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceService _serviceService; // Yeni Eklendi
        private readonly IGymService _gymService;         // Yeni Eklendi

        public ReportsController(IAppUserService appUserService,
                                 IAppointmentService appointmentService,
                                 IServiceService serviceService,
                                 IGymService gymService)
        {
            _appUserService = appUserService;
            _appointmentService = appointmentService;
            _serviceService = serviceService;
            _gymService = gymService;
        }

        // 1. TÜM ANTRENÖRLERİ LİSTELEME
        // GET: /api/reports/trainers
        [HttpGet("trainers")]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _appUserService.GetUsersByRoleAsync("Trainer");

            var result = trainers.Select(t => new
            {
                Id = t.Id,
                AdSoyad = t.FirstName + " " + t.LastName,
                Email = t.Email,
                Mesai = $"{t.ShiftStart} - {t.ShiftEnd}",
                // Burada kaç tane uzmanlığı olduğunu da saydırabiliriz (Eğer Include ile çekildiyse)
            });

            return Ok(result);
        }

        // 2. TARİHE GÖRE RANDEVU DURUMU
        // GET: /api/reports/appointments?date=2025-12-01
        [HttpGet("appointments")]
        public IActionResult GetAppointmentsByDate(DateTime? date)
        {
            if (date == null) return BadRequest("Tarih parametresi zorunludur.");

            var appointments = _appointmentService.GetListByFilter(x => x.Date.Date == date.Value.Date);

            var stats = new
            {
                Tarih = date.Value.ToString("dd.MM.yyyy"),
                Toplam = appointments.Count,
                Onayli = appointments.Count(x => x.Status == AppointmentStatus.Confirmed),
                Bekleyen = appointments.Count(x => x.Status == AppointmentStatus.Pending),
                Iptal = appointments.Count(x => x.Status == AppointmentStatus.Cancelled)
            };

            return Ok(stats);
        }

        // 3. EĞİTMENİN DERS PROGRAMI (Gelecek Dersler)
        // GET: /api/reports/trainer/5/schedule
        [HttpGet("trainer/{trainerId}/schedule")]
        public IActionResult GetTrainerSchedule(int trainerId)
        {
            // Detaylı randevu listesini çekiyoruz (Service, Member dolu gelsin diye)
            var allAppointments = _appointmentService.GetAllAppointmentsWithDetails();

            // LINQ Filtreleme: Sadece bu hoca + Gelecek Tarih + Onaylanmış
            var schedule = allAppointments
                .Where(x => x.TrainerId == trainerId &&
                            x.Date >= DateTime.Today &&
                            x.Status == AppointmentStatus.Confirmed)
                .OrderBy(x => x.Date).ThenBy(x => x.StartTime)
                .Select(x => new
                {
                    Tarih = x.Date.ToString("dd.MM.yyyy"),
                    Saat = $"{x.StartTime} - {x.EndTime}",
                    Ders = x.Service.Name,
                    Ogrenci = x.Member.FirstName + " " + x.Member.LastName,
                    Salon = x.Service.Gym.Name
                })
                .ToList();

            if (!schedule.Any()) return NotFound("Bu eğitmen için planlanmış aktif ders bulunamadı.");

            return Ok(schedule);
        }

        // 4. AYLIK TAHMİNİ GELİR RAPORU (LINQ Aggregation)
        // GET: /api/reports/stats/income?month=12&year=2025
        [HttpGet("stats/income")]
        public IActionResult GetMonthlyIncome(int month, int year)
        {
            var allAppointments = _appointmentService.GetAllAppointmentsWithDetails();

            // İlgili ay ve yıldaki ONAYLI randevuları al
            var monthlyApps = allAppointments
                .Where(x => x.Date.Month == month &&
                            x.Date.Year == year &&
                            x.Status == AppointmentStatus.Confirmed)
                .ToList();

            // LINQ Sum: Fiyatları topla
            decimal totalIncome = monthlyApps.Sum(x => x.Service.Price);

            // LINQ GroupBy: En çok tercih edilen dersi bul
            var popularService = monthlyApps
                .GroupBy(x => x.Service.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => new { DersAdi = g.Key, Adet = g.Count() })
                .FirstOrDefault();

            var report = new
            {
                Donem = $"{month}/{year}",
                ToplamRandevu = monthlyApps.Count,
                TahminiCiro = totalIncome + " TL",
                FavoriDers = popularService
            };

            return Ok(report);
        }

        // 5. SALONLARA GÖRE HİZMET SAYILARI
        // GET: /api/reports/gyms/stats
        [HttpGet("gyms/stats")]
        public IActionResult GetGymStats()
        {
            var gyms = _gymService.GetList();
            var services = _serviceService.GetList();

            // Hangi salonda kaç hizmet var?
            var stats = gyms.Select(g => new
            {
                SalonAdi = g.Name,
                HizmetSayisi = services.Count(s => s.GymId == g.Id),
                CalismaSaatleri = $"{g.OpenTime} - {g.CloseTime}"
            }).ToList();

            return Ok(stats);
        }
    }
}