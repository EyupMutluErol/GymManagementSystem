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
            var allUsers = await _appUserService.GetUserListWithRolesAsync();

            // 2. ViewModel listesine dönüştür (Mapping)
            var memberList = new List<UserListViewModel>();

            foreach (var item in allUsers)
            {
                if(item.RoleName == "Member")
                {
                    memberList.Add(new UserListViewModel
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email,
                        RoleName = item.RoleName
                    });
                }
            }

            // 3. View'a gönder
            return View(memberList);
        }

        [HttpGet]
        public async Task<IActionResult> TrainerList()
        {
            var allUsers = await _appUserService.GetUserListWithRolesAsync();

            var trainerList = new List<UserListViewModel>();

            foreach (var item in allUsers)
            {
                if (item.RoleName == "Trainer") // Sadece antrenörler
                {
                    trainerList.Add(new UserListViewModel
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email,
                        RoleName = item.RoleName
                    });
                }
            }

            return View(trainerList);
        }
        public async Task<IActionResult> ChangeRole(int id, string role)
        {
            // Servise "Bu ID'li kullanıcının rolünü şununla değiştir" diyoruz
            await _appUserService.ChangeUserRoleAsync(id, role);

            // İşlem bitince listeye geri dönüyoruz (Sayfa yenilenmiş oluyor)
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            await _appUserService.DeleteUserAsync(id);
            return RedirectToAction("UserList");
        }


        // SALON YÖNETİMİ
        [HttpGet]
        public IActionResult GymList()
        {
            // 1. Servisten tüm salonları çek (Generic metot yeterli)
            var gyms = _gymService.GetList();

            // 2. Entity listesini ViewModel listesine çevir
            var modelList = new List<GymViewModel>();

            foreach (var item in gyms)
            {
                modelList.Add(new GymViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Address = item.Address,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime
                });
            }

            return View(modelList);
        }

        [HttpGet]
        public IActionResult AddGym()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddGym(GymViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. ViewModel'den gelen veriyi Entity'e çevir
                var gymEntity = new Gym
                {
                    Name = model.Name,
                    Address = model.Address,
                    OpenTime = model.OpenTime,
                    CloseTime = model.CloseTime
                };

                // 2. Servis aracılığıyla veritabanına kaydet
                _gymService.Insert(gymEntity);

                // 3. Listeye geri dön
                return RedirectToAction("GymList");
            }

            // Hata varsa formu tekrar göster
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> AddTrainer()
        {
            // Sadece "Member" (Normal Üye) olanları getir.
            // Çünkü zaten Trainer olanı tekrar Trainer yapmaya gerek yok.
            var members = await _appUserService.GetUsersByRoleAsync("Member");

            var modelList = new List<UserListViewModel>();

            foreach (var item in members)
            {
                modelList.Add(new UserListViewModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    RoleName = "Member" // Zaten member olduklarını biliyoruz
                });
            }

            return View(modelList);
        }

        [HttpPost]
        public async Task<IActionResult> MakeTrainer(int userId)
        {
            // Seçilen kullanıcıyı "Trainer" yap
            await _appUserService.ChangeUserRoleAsync(userId, "Trainer");

            // İşlem bitince listeye geri dön veya dashboarda git
            return RedirectToAction("AddTrainer");
        }
    }
}
