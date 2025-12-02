using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.WebUI.Controllers;

[Authorize(Roles = "Member")] // Sadece üyeler girebilir
public class AppointmentController : Controller
{
    private readonly IGymService _gymService;
    private readonly IServiceService _serviceService;
    private readonly ITrainerServiceService _trainerServiceService;
    private readonly IAppUserService _appUserService;
    private readonly IAppointmentService _appointmentService;
    private readonly UserManager<AppUser> _userManager;

    public AppointmentController(IGymService gymService,
                                 IServiceService serviceService,
                                 ITrainerServiceService trainerServiceService,
                                 IAppUserService appUserService,
                                 IAppointmentService appointmentService,
                                 UserManager<AppUser> userManager)
    {
        _gymService = gymService;
        _serviceService = serviceService;
        _trainerServiceService = trainerServiceService;
        _appUserService = appUserService;
        _appointmentService = appointmentService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetServicesByGym(int gymId)
    {
        var services = _serviceService.GetListByFilter(x => x.GymId == gymId);
        return Json(services.Select(x => new { x.Id, x.Name }));
    }

    // 2. Hizmet seçilince Hocaları getirir
    [HttpGet]
    public IActionResult GetTrainersByService(int serviceId)
    {
        var trainers = _trainerServiceService.GetTrainersByServiceId(serviceId);
        return Json(trainers.Select(x => new { x.Id, FullName = x.FirstName + " " + x.LastName }));
    }

    // 3. Hoca ve Tarih seçilince Saatleri getirir
    [HttpGet]
    public IActionResult GetAvailableHours(int trainerId, int serviceId, string date)
    {
        if (DateTime.TryParse(date, out DateTime parsedDate))
        {
            var slots = _appointmentService.GetAvailableSlots(trainerId, serviceId, parsedDate);
            return Json(slots);
        }
        return Json(new List<string>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        // 1. Tüm salonları çek
        var gyms = _gymService.GetList();

        // 2. Dropdown için hazırla
        ViewBag.Gyms = new SelectList(gyms, "Id", "Name");

        return View();
    }

    [HttpPost]
    public IActionResult Create(MakeAppointmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Saat bilgisini (string "09:00") TimeSpan'e çevir
            TimeSpan startTime = TimeSpan.Parse(model.TimeSlot);
            var service = _serviceService.GetById(model.ServiceId);

            var appointment = new Appointment
            {
                ServiceId = model.ServiceId,
                TrainerId = model.TrainerId,
                MemberId = int.Parse(_userManager.GetUserId(User)),
                Date = model.Date,
                StartTime = startTime,
                EndTime = startTime.Add(TimeSpan.FromMinutes(service.Duration)),
                Status = AppointmentStatus.Pending
            };

            _appointmentService.Insert(appointment);
            return RedirectToAction("Index", "Home"); 
        }

        // Hata varsa sayfayı tekrar doldur
        var gyms = _gymService.GetList();
        ViewBag.Gyms = new SelectList(gyms, "Id", "Name");
        return View(model);
    }

    public IActionResult Cancel(int id)
    {
        var appointment = _appointmentService.GetById(id);
        var currentUserId = int.Parse(_userManager.GetUserId(User));

        if (appointment != null && appointment.MemberId == currentUserId && appointment.Status == AppointmentStatus.Pending)
        {
            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentService.Update(appointment);
            TempData["SuccessMessage"] = "Randevunuz başarıyla iptal edildi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Bu randevuyu iptal edemezsiniz.";
        }

        return RedirectToAction("MyAppointments");
    }

    [HttpGet]
    public IActionResult MyAppointments()
    {
        // 1. Giriş yapan üyenin ID'sini bul
        var memberId = int.Parse(_userManager.GetUserId(User));

        // 2. Randevularını detaylı çek
        var appointments = _appointmentService.GetAppointmentsByMember(memberId);

        // 3. ViewModel'e çevir
        var modelList = new List<AppointmentListViewModel>();

        foreach (var item in appointments)
        {
            // Duruma göre renk belirle
            string color = "secondary";
            string statusText = item.Status.ToString();

            if (item.Status == AppointmentStatus.Pending) { color = "warning"; statusText = "Onay Bekliyor"; }
            else if (item.Status == AppointmentStatus.Confirmed) { color = "success"; statusText = "Onaylandı"; }
            else if (item.Status == AppointmentStatus.Cancelled) { color = "danger"; statusText = "İptal Edildi"; }

            modelList.Add(new AppointmentListViewModel
            {
                Id = item.Id,
                GymName = (item.Service != null && item.Service.Gym != null) ? item.Service.Gym.Name : "-",
                ServiceName = item.Service != null ? item.Service.Name : "-",
                TrainerName = item.Trainer != null ? item.Trainer.FirstName + " " + item.Trainer.LastName : "-",
                Date = item.Date,
                StartTime = item.StartTime.ToString(@"hh\:mm"),
                EndTime = item.EndTime.ToString(@"hh\:mm"),
                Status = statusText,
                StatusColor = color
            });
        }

        return View(modelList);
    }

}
