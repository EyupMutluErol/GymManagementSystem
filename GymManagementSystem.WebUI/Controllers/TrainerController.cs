using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.WebUI.Controllers;

[Authorize(Roles = "Trainer")] // Sadece Eğitmenler girebilir
public class TrainerController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAppointmentService _appointmentService;

    public TrainerController(UserManager<AppUser> userManager, IAppointmentService appointmentService)
    {
        _userManager = userManager;
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> Index()
    {
        // Giriş yapan eğitmeni bul
        var trainer = await _userManager.GetUserAsync(User);

        // Eğitmene ait randevuları çek (Bunun için servise yeni bir metot ekleyeceğiz)
        var appointments = _appointmentService.GetAppointmentsByTrainer(trainer.Id);

        var modelList = new List<AppointmentListViewModel>();

        foreach (var item in appointments)
        {
            string color = "secondary";
            string statusText = "";

            if (item.Status == AppointmentStatus.Pending) { color = "warning"; statusText = "Onay Bekliyor"; }
            else if (item.Status == AppointmentStatus.Confirmed) { color = "success"; statusText = "Onaylandı"; }
            else if (item.Status == AppointmentStatus.Cancelled) { color = "danger"; statusText = "İptal"; }

            modelList.Add(new AppointmentListViewModel
            {
                Id = item.Id,
                // Öğrenci adını göster
                TrainerName = item.Member.FirstName + " " + item.Member.LastName,
                ServiceName = item.Service.Name,
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