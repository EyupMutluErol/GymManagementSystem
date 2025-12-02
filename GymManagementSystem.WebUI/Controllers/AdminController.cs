using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Business.Dtos;
using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IGymService _gymService;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceService _serviceService;

        public AdminController(IAppUserService appUserService, IGymService gymService, IAppointmentService appointmentService,IServiceService serviceService)
        {
            _appUserService = appUserService;
            _gymService = gymService;
            _appointmentService = appointmentService;
            _serviceService = serviceService;
        }

        // ADMİN DASHBOARD
        public async Task<IActionResult> Index()
        {
            var members = await _appUserService.GetUsersByRoleAsync("Member");
            var trainers = await _appUserService.GetUsersByRoleAsync("Trainer");

            var pendingApps = _appointmentService.GetListByFilter(x => x.Status == AppointmentStatus.Pending);

            var gyms = _gymService.GetList();
            var services = _serviceService.GetList();

            var model = new DashboardViewModel
            {
                TotalMemberCount = members.Count,
                ActiveTrainerCount = trainers.Count,
                PendingAppointmentCount = pendingApps.Count,
                TotalGymCount = gyms.Count,
                TotalServiceCount = services.Count
            };

            return View(model);
        }

        // KULLANICI YÖNETİMİ
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

        public async Task<IActionResult> DeleteUser(int id)
        {
            await _appUserService.DeleteUserAsync(id);
            return RedirectToAction("UserList");
        }


        // ANTRENÖR YÖNETİMİ
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

        [HttpGet]
        public async Task<IActionResult> EditTrainer(int id)
        {
            // Servisten veriyi çek
            var dto = await _appUserService.GetTrainerDetailsAsync(id);
            if (dto == null) return NotFound();

            // Salon listesini ViewBag'e koy (Dropdown için)
            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name", dto.GymId);

            return View(dto);
        }

        

        [HttpPost]
        public async Task<IActionResult> EditTrainer(TrainerDetailDto model, int[] selectedServices)
        {
            model.ServiceList = new List<ServiceCheckBoxDto>();
            if (selectedServices != null)
            {
                foreach (var serviceId in selectedServices)
                {
                    model.ServiceList.Add(new ServiceCheckBoxDto { ServiceId = serviceId, IsSelected = true });
                }
            }

            try
            {
                await _appUserService.UpdateTrainerDetailsAsync(model);
                return RedirectToAction("TrainerList");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            // 1. Salon Dropdown'ını doldur
            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name", model.GymId);

            // 2. Hizmet Listesini İSİMLERİYLE birlikte yeniden oluştur
            if (model.GymId.HasValue)
            {
                // Seçilen salondaki tüm hizmetleri veritabanından çek
                var allGymServices = _serviceService.GetListByFilter(x => x.GymId == model.GymId.Value);

                // Listeyi sıfırla ve baştan düzgünce doldur
                model.ServiceList = new List<ServiceCheckBoxDto>();

                foreach (var service in allGymServices)
                {
                    model.ServiceList.Add(new ServiceCheckBoxDto
                    {
                        ServiceId = service.Id,
                        ServiceName = service.Name, // İsim bilgisini burası getirir
                        IsSelected = selectedServices != null && selectedServices.Contains(service.Id)
                    });
                }
            }

            return View(model);
        }


        // SALON YÖNETİMİ
        [HttpGet]
        public IActionResult GymList()
        {
            // 1. Servisten tüm salonları çek 
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
        public IActionResult EditGym(int id)
        {
            // 1. Düzenlenecek salonu bul
            var gym = _gymService.GetById(id);

            if (gym == null)
            {
                return NotFound();
            }

            // 2. Entity'yi ViewModel'e çevir
            var model = new GymViewModel
            {
                Id = gym.Id,
                Name = gym.Name,
                Address = gym.Address,
                OpenTime = gym.OpenTime,
                CloseTime = gym.CloseTime
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditGym(GymViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Veritabanındaki orijinal kaydı bul
                var gymEntity = _gymService.GetById(model.Id);

                if (gymEntity == null)
                {
                    return NotFound();
                }

                // 2. Yeni değerleri aktar
                gymEntity.Name = model.Name;
                gymEntity.Address = model.Address;
                gymEntity.OpenTime = model.OpenTime;
                gymEntity.CloseTime = model.CloseTime;

                // 3. Güncelle
                _gymService.Update(gymEntity);

                return RedirectToAction("GymList");
            }

            return View(model);
        }

        public IActionResult DeleteGym(int id)
        {
            try
            {
                var gym = _gymService.GetById(id);
                if (gym != null)
                {
                    _gymService.Delete(gym);
                    TempData["SuccessMessage"] = "Salon başarıyla silindi.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Bu salon silinemez! Çünkü içinde kayıtlı antrenörler veya hizmetler var. Önce onları silmeli veya başka salona taşımalısınız.";
            }

            return RedirectToAction("GymList");
        }

        // SERVICE YÖNETİMİ

        [HttpGet]
        public IActionResult ServiceList()
        {
            var services = _serviceService.GetList();
            var gyms = _gymService.GetList();

            var modelList = new List<ServiceViewModel>();

            foreach (var item in services)
            {
                var gym = gyms.FirstOrDefault(x => x.Id == item.GymId);
                modelList.Add(new ServiceViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Duration = item.Duration,
                    Price = item.Price,
                    GymName = gym != null ? gym.Name : "Belirtilmemiş"
                });
            }

            return View(modelList);
        }

        [HttpGet]
        public IActionResult AddService()
        {
            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult AddService(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    Duration = model.Duration,
                    Price = model.Price,
                    GymId = model.GymId
                };

                _serviceService.Insert(service);
                return RedirectToAction("ServiceList");
            }

            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _serviceService.GetById(id);
            if (service == null)
            {
                return NotFound();
            }

            var model = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Duration = service.Duration,
                Price = service.Price,
                GymId = service.GymId
            };

            // Dropdown için salonları getir ve mevcut salonu seçili yap
            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name", service.GymId);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditService(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = _serviceService.GetById(model.Id);
                if (service == null)
                {
                    return NotFound();
                }

                // Güncelleme
                service.Name = model.Name;
                service.Duration = model.Duration;
                service.Price = model.Price;
                service.GymId = model.GymId;

                _serviceService.Update(service);
                return RedirectToAction("ServiceList");
            }

            // Hata varsa dropdown'ı tekrar doldur
            var gyms = _gymService.GetList();
            ViewBag.Gyms = new SelectList(gyms, "Id", "Name", model.GymId);

            return View(model);
        }

        public IActionResult DeleteService(int id)
        {
            try
            {
                var service = _serviceService.GetById(id);
                if (service != null)
                {
                    _serviceService.Delete(service);
                    TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Bu hizmet silinemez! Çünkü bu hizmete ait alınmış randevular mevcut.";
            }

            return RedirectToAction("ServiceList");
        }



        // RANDEVU YÖNETİMİ

        [HttpGet]
        public IActionResult AppointmentList()
        {
            var appointments = _appointmentService.GetAllAppointmentsWithDetails();
            var modelList = new List<AppointmentListViewModel>();

            foreach (var item in appointments)
            {
                string color = "secondary";
                string statusText = "";

                if (item.Status == AppointmentStatus.Pending) { color = "warning"; statusText = "Onay Bekliyor"; }
                else if (item.Status == AppointmentStatus.Confirmed) { color = "success"; statusText = "Onaylandı"; }
                else if (item.Status == AppointmentStatus.Cancelled) { color = "danger"; statusText = "Reddedildi / İptal"; }

                modelList.Add(new AppointmentListViewModel
                {
                    Id = item.Id,
                    GymName = item.Service.Gym.Name,
                    ServiceName = item.Service.Name,
                    TrainerName = item.Trainer.FirstName + " " + item.Trainer.LastName,
                    MemberName = item.Member.FirstName + " " + item.Member.LastName,
                    Date = item.Date,
                    StartTime = item.StartTime.ToString(@"hh\:mm"),
                    EndTime = item.EndTime.ToString(@"hh\:mm"),
                    Status = statusText,
                    StatusColor = color
                });
            }

            return View(modelList);
        }

        public IActionResult ApproveAppointment(int id)
        {
            var appointment = _appointmentService.GetById(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                _appointmentService.Update(appointment);
            }
            return RedirectToAction("AppointmentList");
        }

        public IActionResult RejectAppointment(int id)
        {
            var appointment = _appointmentService.GetById(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                _appointmentService.Update(appointment);
            }
            return RedirectToAction("AppointmentList");
        }

        // EKSTRA METHODLAR
        public async Task<IActionResult> ChangeRole(int id, string role)
        {
            await _appUserService.ChangeUserRoleAsync(id, role);
            return RedirectToAction("UserList");
        }

        [HttpGet]
        public IActionResult GetServicesByGym(int gymId)
        {
            // Salona ait hizmetleri çek
            var services = _serviceService.GetListByFilter(x => x.GymId == gymId);
            var jsonResult = services.Select(s => new { s.Id, s.Name }).ToList();

            return Json(jsonResult);
        }
    }
}
