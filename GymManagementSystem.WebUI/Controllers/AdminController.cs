using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IGymService _gymService;
        private readonly IAppointmentService _appointmentService;

        public AdminController(IAppUserService appUserService, IGymService gymService, IAppointmentService appointmentService)
        {
            _appUserService = appUserService;
            _gymService = gymService;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _appUserService.GetUsersByRoleAsync("Member");
            var trainers = await _appUserService.GetUsersByRoleAsync("Trainer");

            var pendingApps = _appointmentService.GetListByFilter(x => x.Status == AppointmentStatus.Pending);

            var gyms = _gymService.GetList();

            var model = new DashboardViewModel
            {
                TotalMemberCount = members.Count,
                ActiveTrainerCount = trainers.Count,
                PendingAppointmentCount = pendingApps.Count,
                TotalGymCount = gyms.Count
            };

            return View(model);
        }

        // --- KULLANICI YÖNETİMİ ---
        public async Task<IActionResult> UserList()
        {
            var dtoList = await _appUserService.GetUserListWithRolesAsync();

            // 2. ViewModel listesine dönüştür (Mapping)
            var viewModelList = new List<UserListViewModel>();

            foreach (var item in dtoList)
            {
                viewModelList.Add(new UserListViewModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    RoleName = item.RoleName
                });
            }

            // 3. View'a gönder
            return View(viewModelList);
        }
    }
}
